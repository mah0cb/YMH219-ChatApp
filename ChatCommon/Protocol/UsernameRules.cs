namespace ChatCommon.Protocol;

/// <summary>Sunucu ve istemcinin ortak kullandığı kullanıcı adı kuralları.</summary>
public static class UsernameRules
{
    public const int MaxLength = 20;

    /// <summary>Kullanıcı adı geçerliyse null, değilse Türkçe hata metni döner.</summary>
    public static string? Validate(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return "Kullanıcı adı boş olamaz.";
        if (username.Length > MaxLength)
            return $"Kullanıcı adı en fazla {MaxLength} karakter olabilir.";
        // Boşluk "/w ad mesaj" komutunu, virgül kullanıcı listesi biçimini bozar.
        if (username.Any(ch => char.IsWhiteSpace(ch) || ch == ','))
            return "Kullanıcı adı boşluk veya virgül içeremez.";
        return null;
    }
}
