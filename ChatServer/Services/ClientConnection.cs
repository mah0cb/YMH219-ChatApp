using System.Net.Sockets;
using System.Text;
using ChatCommon.Models;
using ChatCommon.Protocol;

namespace ChatServerApp.Services;

/// <summary>Sunucuya bağlı tek bir istemcinin bağlantısı ve okuma döngüsü.</summary>
public class ClientConnection
{
    private readonly TcpClient _tcpClient;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public ClientConnection(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
        NetworkStream stream = tcpClient.GetStream();
        _reader = new StreamReader(stream, Encoding.UTF8);
        _writer = new StreamWriter(stream, new UTF8Encoding(false));
        RemoteEndPoint = tcpClient.Client.RemoteEndPoint?.ToString() ?? "bilinmiyor";
        ConnectedAt = DateTime.Now;
    }

    /// <summary>Katılma mesajı kabul edildikten sonra atanan kullanıcı adı.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>İstemcinin IP:port bilgisi.</summary>
    public string RemoteEndPoint { get; }

    public DateTime ConnectedAt { get; }

    /// <summary>Tek bir mesaj okur; bağlantı kapandıysa null döner, bozuk satırları atlar.</summary>
    public async Task<ChatMessage?> ReadAsync(CancellationToken token)
    {
        string? line;
        while ((line = await _reader.ReadLineAsync(token)) != null)
        {
            ChatMessage? message = MessageSerializer.Deserialize(line);
            if (message != null)
                return message;
        }
        return null;
    }

    /// <summary>Bağlantı kapanana veya Leave gelene kadar mesajları okuyup işleyiciye verir.</summary>
    public async Task ReadLoopAsync(Func<ChatMessage, Task> handleMessage, CancellationToken token)
    {
        ChatMessage? message;
        while ((message = await ReadAsync(token)) != null && message.Type != MessageType.Leave)
        {
            await handleMessage(message);
        }
    }

    /// <summary>İstemciye mesaj gönderir; aynı anda tek yazma yapılması için kilit kullanır.</summary>
    public async Task SendAsync(ChatMessage message)
    {
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

    /// <summary>Bağlantıyı kapatır; okuma döngüsü bu sayede sonlanır.</summary>
    public void Close() => _tcpClient.Close();
}
