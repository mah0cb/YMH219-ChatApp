using System.Text.Json;
using System.Text.Json.Serialization;
using ChatCommon.Models;

namespace ChatCommon.Protocol;

/// <summary>
/// Satır tabanlı protokol: her mesaj tek satırlık bir JSON'dur.
/// JSON metnindeki satır sonları kaçışlı yazıldığı için bir satır her zaman tek mesajdır.
/// </summary>
public static class MessageSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>Mesajı tek satırlık JSON'a çevirir.</summary>
    public static string Serialize(ChatMessage message) => JsonSerializer.Serialize(message, Options);

    /// <summary>Satırı mesaja çevirir; bozuk veya boş satırda null döner.</summary>
    public static ChatMessage? Deserialize(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;
        try
        {
            return JsonSerializer.Deserialize<ChatMessage>(line, Options);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>Mesajı yazıcıya tek satır olarak yazar ve gönderir.</summary>
    public static async Task WriteAsync(StreamWriter writer, ChatMessage message)
    {
        await writer.WriteLineAsync(Serialize(message));
        await writer.FlushAsync();
    }
}
