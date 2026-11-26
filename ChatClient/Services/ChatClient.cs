using System.Net.Sockets;
using System.Text;
using ChatCommon.Models;
using ChatCommon.Protocol;

namespace ChatClientApp.Services;

/// <summary>Sohbet sunucusuna bağlanan, mesaj gönderen ve gelen mesajları event ile bildiren istemci.</summary>
public class ChatClient
{
    private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(5);

    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private TcpClient? _tcpClient;
    private StreamWriter? _writer;
    private bool _closingByUser;

    /// <summary>Sunucudan mesaj geldiğinde tetiklenir (arka plan thread'inden).</summary>
    public event EventHandler<ChatMessage>? MessageReceived;

    /// <summary>Bağlantı kapandığında tetiklenir (arka plan thread'inden).</summary>
    public event EventHandler<DisconnectedEventArgs>? Disconnected;

    public string Username { get; private set; } = string.Empty;

    public bool IsConnected { get; private set; }

    /// <summary>
    /// Sunucuya bağlanır, kullanıcı adıyla katılma isteği gönderir ve sunucunun onayını bekler.
    /// Her türlü bağlantı hatası Türkçe açıklamalı ChatConnectionException olarak fırlatılır.
    /// </summary>
    public async Task ConnectAsync(string host, int port, string username)
    {
        if (IsConnected)
            throw new InvalidOperationException("Zaten bağlısınız.");
        string? error = UsernameRules.Validate(username);
        if (error != null)
            throw new ChatConnectionException(error);
        if (string.IsNullOrWhiteSpace(host))
            throw new ChatConnectionException("Sunucu adresi boş olamaz.");

        var tcpClient = new TcpClient();
        try
        {
            using var timeout = new CancellationTokenSource(ConnectTimeout);
            await tcpClient.ConnectAsync(host, port, timeout.Token);
            NetworkStream stream = tcpClient.GetStream();
            var reader = new StreamReader(stream, Encoding.UTF8);
            _writer = new StreamWriter(stream, new UTF8Encoding(false));

            await MessageSerializer.WriteAsync(_writer, new ChatMessage { Type = MessageType.Join, From = username });
            ChatMessage reply = await WaitForJoinReplyAsync(reader, username, timeout.Token);

            _tcpClient = tcpClient;
            Username = username;
            IsConnected = true;
            _closingByUser = false;
            MessageReceived?.Invoke(this, reply);
            _ = Task.Run(() => ReadLoopAsync(reader));
        }
        catch (Exception ex)
        {
            tcpClient.Close();
            throw ex switch
            {
                ChatConnectionException => ex,
                OperationCanceledException => new ChatConnectionException("Sunucu zamanında yanıt vermedi (zaman aşımı). Adres ve portu kontrol edin."),
                SocketException { SocketErrorCode: SocketError.ConnectionRefused } =>
                    new ChatConnectionException($"{host}:{port} adresinde çalışan bir sunucu yok. Sunucu kapalı olabilir."),
                SocketException { SocketErrorCode: SocketError.HostNotFound or SocketError.NoData } =>
                    new ChatConnectionException($"'{host}' sunucu adresi bulunamadı."),
                SocketException or IOException => new ChatConnectionException($"Bağlantı kurulamadı: {ex.Message}"),
                _ => ex
            };
        }
    }

    /// <summary>
    /// Sunucu kendi Join mesajımızı yayınlayana kadar bekler (kabul). System mesajı gelirse ret demektir.
    /// Kabulden önce başkalarına ait yayınlar gelebileceği için onlar atlanır.
    /// </summary>
    private static async Task<ChatMessage> WaitForJoinReplyAsync(StreamReader reader, string username, CancellationToken token)
    {
        while (true)
        {
            string? line = await reader.ReadLineAsync(token);
            if (line == null)
                throw new ChatConnectionException("Sunucu bağlantıyı kapattı.");

            ChatMessage? message = MessageSerializer.Deserialize(line);
            if (message?.Type == MessageType.System)
                throw new ChatConnectionException(message.Text);
            if (message?.Type == MessageType.Join && message.From == username)
                return message;
        }
    }

    /// <summary>Herkese gidecek sohbet mesajı gönderir.</summary>
    public Task SendTextAsync(string text) =>
        SendAsync(new ChatMessage { Type = MessageType.Chat, From = Username, Text = text });

    /// <summary>Mesajı sunucuya tek satır JSON olarak gönderir.</summary>
    public async Task SendAsync(ChatMessage message)
    {
        if (!IsConnected || _writer == null)
            throw new InvalidOperationException("Sunucuya bağlı değilsiniz.");

        await _writeLock.WaitAsync();
        try
        {
            await MessageSerializer.WriteAsync(_writer, message);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    /// <summary>Sunucuya ayrılma bildirimi gönderir ve bağlantıyı kapatır.</summary>
    public void Disconnect()
    {
        if (!IsConnected)
            return;
        _closingByUser = true;
        try
        {
            _writeLock.Wait();
            try
            {
                _writer?.WriteLine(MessageSerializer.Serialize(new ChatMessage { Type = MessageType.Leave, From = Username }));
                _writer?.Flush();
            }
            finally
            {
                _writeLock.Release();
            }
        }
        catch (Exception ex) when (ex is IOException or ObjectDisposedException)
        {
            // Bağlantı zaten kopmuş; yine de kapatıyoruz.
        }
        _tcpClient?.Close();
    }

    /// <summary>Bağlantı kapanana kadar sunucudan gelen satırları okur.</summary>
    private async Task ReadLoopAsync(StreamReader reader)
    {
        string reason = "Sunucu bağlantıyı kapattı.";
        try
        {
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                ChatMessage? message = MessageSerializer.Deserialize(line);
                if (message != null)
                    MessageReceived?.Invoke(this, message);
            }
        }
        catch (Exception ex) when (ex is IOException or ObjectDisposedException or SocketException)
        {
            reason = "Sunucuyla bağlantı koptu.";
        }
        finally
        {
            IsConnected = false;
            _tcpClient?.Close();
            Disconnected?.Invoke(this, _closingByUser
                ? new DisconnectedEventArgs("Bağlantı kesildi.", true)
                : new DisconnectedEventArgs(reason, false));
        }
    }
}
