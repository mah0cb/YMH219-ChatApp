namespace ChatClientApp.Services;

/// <summary>Sunucunun bağlantıyı reddettiği veya yanıt vermediği durumlar için hata.</summary>
public class ChatConnectionException : Exception
{
    public ChatConnectionException(string message) : base(message)
    {
    }
}
