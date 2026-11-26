using System.Text;

namespace ChatClientApp.Services;

/// <summary>Ekrandaki mesaj geçmişini .txt dosyasına yazar.</summary>
public static class ChatHistoryExporter
{
    /// <summary>Başlık satırıyla birlikte mesajları UTF-8 olarak kaydeder; dosya hatalarını çağırana bırakır.</summary>
    public static void Save(string path, string username, IEnumerable<string> lines)
    {
        var header = new[]
        {
            $"Sohbet geçmişi - kullanıcı: {(username.Length > 0 ? username : "-")} - kayıt: {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
            new string('-', 60)
        };
        File.WriteAllLines(path, header.Concat(lines), Encoding.UTF8);
    }
}
