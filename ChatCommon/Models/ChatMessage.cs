using System.Text.Json.Serialization;

namespace ChatCommon.Models;

/// <summary>Sunucu ile istemci arasında taşınan tek mesaj.</summary>
public class ChatMessage
{
    private string _text = string.Empty;

    public MessageType Type { get; set; }

    /// <summary>Gönderen kullanıcı adı (sunucu mesajlarında boş).</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>Özel mesajın alıcısı; diğer türlerde boş.</summary>
    public string To { get; set; } = string.Empty;

    public string Text
    {
        get => _text;
        set => _text = value ?? string.Empty;
    }

    /// <summary>Mesajın zamanı; sunucu yayınlarken kendi saatiyle doldurur.</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>UserList mesajındaki kullanıcı adlarını dizi olarak döner.</summary>
    [JsonIgnore]
    public string[] Users => Text.Split(',', StringSplitOptions.RemoveEmptyEntries);

    /// <summary>Listede gösterilecek "[SS:dd:ss] Gönderen: metin" biçimi.</summary>
    public override string ToString()
    {
        string zaman = $"[{Timestamp:HH:mm:ss}]";
        return Type switch
        {
            MessageType.Chat => $"{zaman} {From}: {Text}",
            MessageType.Private => $"{zaman} (özel) {From} → {To}: {Text}",
            MessageType.Join => $"{zaman} * {From} sohbete katıldı",
            MessageType.Leave => $"{zaman} * {From} sohbetten ayrıldı",
            _ => $"{zaman} [Sistem] {Text}"
        };
    }
}
