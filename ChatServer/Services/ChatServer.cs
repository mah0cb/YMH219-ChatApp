using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using ChatCommon.Models;

namespace ChatServerApp.Services;

/// <summary>TCP sohbet sunucusu: istemcileri kabul eder, kullanıcı adıyla kaydeder ve mesajları dağıtır.</summary>
public class ChatServer
{
    private readonly ConcurrentDictionary<string, ClientConnection> _clients = new(StringComparer.OrdinalIgnoreCase);
    private TcpListener? _listener;
    private CancellationTokenSource? _cancellation;

    /// <summary>Bir istemci kullanıcı adıyla kaydolduğunda tetiklenir. Tüm event'ler arka plan thread'inden gelir.</summary>
    public event EventHandler<ClientConnection>? ClientConnected;

    /// <summary>Kayıtlı bir istemcinin bağlantısı kapandığında tetiklenir.</summary>
    public event EventHandler<ClientConnection>? ClientDisconnected;

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

    /// <summary>Dinlemeyi durdurur ve tüm istemci bağlantılarını kapatır.</summary>
    public void Stop()
    {
        _cancellation?.Cancel();
        _listener?.Stop();
        _listener = null;
        foreach (ClientConnection client in _clients.Values)
            client.Close();
    }

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
        catch (OperationCanceledException)
        {
            // Stop() çağrıldı, normal kapanış.
        }
        catch (ObjectDisposedException)
        {
            // Dinleyici kapatıldı, normal kapanış.
        }
    }

    /// <summary>Tek istemcinin yaşam döngüsü: katılma, mesaj okuma, ayrılma.</summary>
    private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken token)
    {
        var connection = new ClientConnection(tcpClient);
        try
        {
            ChatMessage? join = await connection.ReadAsync(token);
            if (join?.Type != MessageType.Join || string.IsNullOrWhiteSpace(join.From))
                return;

            string username = join.From.Trim();
            if (!_clients.TryAdd(username, connection))
                return;

            connection.Username = username;
            ClientConnected?.Invoke(this, connection);
            await connection.SendAsync(new ChatMessage { Type = MessageType.Join, From = username, Timestamp = DateTime.Now });

            await connection.ReadLoopAsync(message => HandleMessageAsync(connection, message), token);
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException or OperationCanceledException)
        {
            // Bağlantı koptu veya sunucu durduruldu; temizlik finally'de.
        }
        finally
        {
            connection.Close();
            if (_clients.TryRemove(new KeyValuePair<string, ClientConnection>(connection.Username, connection)))
                ClientDisconnected?.Invoke(this, connection);
        }
    }

    /// <summary>İstemciden gelen mesajı sunucu bilgileriyle damgalayıp işler.</summary>
    private async Task HandleMessageAsync(ClientConnection sender, ChatMessage message)
    {
        message.From = sender.Username;
        message.Timestamp = DateTime.Now;
        if (message.Type == MessageType.Chat)
        {
            MessageReceived?.Invoke(this, message);
            await sender.SendAsync(message);
        }
    }
}
