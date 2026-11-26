using System.Net.Sockets;
using System.Text;
using ChatCommon.Models;
using ChatCommon.Protocol;

namespace ChatClientApp.Services;

/// <summary>Sohbet sunucusuna bağlanan, mesaj gönderen ve gelen mesajları event ile bildiren istemci.</summary>
public class ChatClient
{
    private static readonly TimeSpan JoinTimeout = TimeSpan.FromSeconds(5);

    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private TcpClient? _tcpClient;
    private StreamWriter? _writer;
    private bool _closingByUser;

    /// <summary>Sunucudan mesaj geldiğinde tetiklenir (arka plan thread'inden).</summary>
    public event EventHandler<ChatMessage>? MessageReceived;

    /// <summary>Bağlantı kapandığında tetiklenir; argüman kapanma nedenidir.</summary>
    public event EventHandler<string>? Disconnected;

    public string Username { get; private set; } = string.Empty;

    public bool IsConnected { get; private set; }

    /// <summary>
    /// Sunucuya bağlanır, kullanıcı adıyla katılma isteği gönderir ve sunucunun onayını bekler.
    /// Hata durumunda SocketException, IOException veya ChatConnectionException fırlatır.
    /// </summary>
    public async Task ConnectAsync(string host, int port, string username)
    {
        if (IsConnected)
            throw new InvalidOperationException("Zaten bağlısınız.");

        var tcpClient = new TcpClient();
        try
        {
            await tcpClient.ConnectAsync(host, port);
            NetworkStream stream = tcpClient.GetStream();
            var reader = new StreamReader(stream, Encoding.UTF8);
            _writer = new StreamWriter(stream, new UTF8Encoding(false));

            await MessageSerializer.WriteAsync(_writer, new ChatMessage { Type = MessageType.Join, From = username });

            using var timeout = new CancellationTokenSource(JoinTimeout);
            ChatMessage? reply = MessageSerializer.Deserialize(await reader.ReadLineAsync(timeout.Token));
            if (reply == null || reply.Type != MessageType.Join || reply.From != username)
                throw new ChatConnectionException(reply?.Text is { Length: > 0 } text ? text : "Sunucu katılma isteğini kabul etmedi.");

            _tcpClient = tcpClient;
            Username = username;
            IsConnected = true;
            _closingByUser = false;
            MessageReceived?.Invoke(this, reply);
            _ = Task.Run(() => ReadLoopAsync(reader));
        }
        catch (OperationCanceledException)
        {
            tcpClient.Close();
            throw new ChatConnectionException("Sunucu zamanında yanıt vermedi.");
        }
        catch
        {
            tcpClient.Close();
            throw;
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
            reason = "Sunucu bağlantısı koptu.";
        }
        finally
        {
            IsConnected = false;
            _tcpClient?.Close();
            Disconnected?.Invoke(this, _closingByUser ? "Bağlantı kesildi." : reason);
        }
    }
}
