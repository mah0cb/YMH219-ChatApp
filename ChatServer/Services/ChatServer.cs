using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using ChatCommon.Models;
using ChatCommon.Protocol;

namespace ChatServerApp.Services;

/// <summary>TCP sohbet sunucusu: istemcileri kabul eder, kullanıcı adıyla kaydeder ve mesajları dağıtır.</summary>
public class ChatServer
{
    private static readonly TimeSpan JoinTimeout = TimeSpan.FromSeconds(10);

    private readonly ConcurrentDictionary<string, ClientConnection> _clients = new(StringComparer.OrdinalIgnoreCase);
    private TcpListener? _listener;
    private CancellationTokenSource? _cancellation;

    /// <summary>Bir istemci kullanıcı adıyla kaydolduğunda tetiklenir. Tüm event'ler arka plan thread'inden gelir.</summary>
    public event EventHandler<ClientConnection>? ClientConnected;

    /// <summary>Kayıtlı bir istemcinin bağlantısı kapandığında tetiklenir.</summary>
    public event EventHandler<ClientConnection>? ClientDisconnected;

    /// <summary>Bir bağlantı reddedildiğinde (aynı/geçersiz kullanıcı adı) nedeniyle birlikte tetiklenir.</summary>
    public event EventHandler<string>? ClientRejected;

    /// <summary>Bir istemciden sohbet mesajı geldiğinde tetiklenir.</summary>
    public event EventHandler<ChatMessage>? MessageReceived;

    public bool IsRunning => _listener != null;

    /// <summary>
    /// Dinlemeyi başlatır ve kabul döngüsünün Task'ını döner; bu Task sunucu durunca tamamlanır.
    /// Port meşgulse SocketException hemen (await'ten önce) fırlatılır.
    /// </summary>
    public Task StartAsync(int port)
    {
        if (IsRunning)
            throw new InvalidOperationException("Sunucu zaten çalışıyor.");

        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        _listener = listener;
        _cancellation = new CancellationTokenSource();
        CancellationToken token = _cancellation.Token;
        return Task.Run(() => AcceptLoopAsync(listener, token));
    }

    /// <summary>Dinlemeyi durdurur ve tüm istemci bağlantılarını kapatır. Birden çok kez çağrılabilir.</summary>
    public void Stop()
    {
        _cancellation?.Cancel();
        _listener?.Stop();
        _listener = null;
        foreach (ClientConnection client in _clients.Values)
            client.Close();
    }

    /// <summary>Mesajı tüm bağlı istemcilere paralel gönderir.</summary>
    public Task BroadcastAsync(ChatMessage message) =>
        Task.WhenAll(_clients.Values.Select(client => SendSafeAsync(client, message)));

    private async Task AcceptLoopAsync(TcpListener listener, CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync(token);
                _ = HandleClientAsync(tcpClient, token);
            }
        }
        catch (Exception ex) when (ex is OperationCanceledException or ObjectDisposedException or SocketException)
        {
            // Stop() çağrıldı veya dinleyici kapandı; döngü biter, Task tamamlanır.
        }
    }

    /// <summary>Tek istemcinin yaşam döngüsü: katılma, mesaj okuma, ayrılma.</summary>
    private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken token)
    {
        var connection = new ClientConnection(tcpClient);
        try
        {
            ChatMessage? join;
            using (var joinTimeout = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                joinTimeout.CancelAfter(JoinTimeout);
                join = await connection.ReadAsync(joinTimeout.Token);
            }
            if (join?.Type != MessageType.Join)
                return;

            string username = join.From.Trim();
            string? error = UsernameRules.Validate(username);
            if (error == null && !_clients.TryAdd(username, connection))
                error = $"'{username}' kullanıcı adı zaten kullanımda. Başka bir ad seçin.";
            if (error != null)
            {
                ClientRejected?.Invoke(this, $"{connection.RemoteEndPoint} reddedildi: {error}");
                await SendSafeAsync(connection, new ChatMessage { Type = MessageType.System, Text = error });
                return;
            }

            connection.Username = username;
            ClientConnected?.Invoke(this, connection);
            await BroadcastAsync(new ChatMessage { Type = MessageType.Join, From = username, Timestamp = DateTime.Now });
            await BroadcastUserListAsync();

            await connection.ReadLoopAsync(message => HandleMessageAsync(connection, message), token);
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException or OperationCanceledException)
        {
            // Bağlantı koptu, katılma zaman aşımına uğradı veya sunucu durduruldu; temizlik finally'de.
        }
        finally
        {
            connection.Close();
            if (_clients.TryRemove(new KeyValuePair<string, ClientConnection>(connection.Username, connection)))
            {
                ClientDisconnected?.Invoke(this, connection);
                await BroadcastAsync(new ChatMessage { Type = MessageType.Leave, From = connection.Username, Timestamp = DateTime.Now });
                await BroadcastUserListAsync();
            }
        }
    }

    /// <summary>İstemciden gelen mesajı sunucu bilgileriyle damgalayıp işler.</summary>
    private async Task HandleMessageAsync(ClientConnection sender, ChatMessage message)
    {
        message.From = sender.Username;
        message.Timestamp = DateTime.Now;
        if (message.Type == MessageType.Chat && !string.IsNullOrWhiteSpace(message.Text))
        {
            MessageReceived?.Invoke(this, message);
            await BroadcastAsync(message);
        }
    }

    /// <summary>Güncel çevrimiçi kullanıcı listesini herkese gönderir.</summary>
    private Task BroadcastUserListAsync() =>
        BroadcastAsync(new ChatMessage { Type = MessageType.UserList, Text = string.Join(",", _clients.Keys.Order()) });

    /// <summary>Gönderim hatasında istemciyi kapatır; kapanış onun okuma döngüsünde temizlenir.</summary>
    private static async Task SendSafeAsync(ClientConnection client, ChatMessage message)
    {
        try
        {
            await client.SendAsync(message);
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException)
        {
            client.Close();
        }
    }
}
