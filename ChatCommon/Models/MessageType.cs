namespace ChatCommon.Models;

/// <summary>Protokoldeki mesaj türleri.</summary>
public enum MessageType
{
    /// <summary>İstemcinin kullanıcı adıyla katılma isteği.</summary>
    Join,
    /// <summary>İstemcinin ayrılma bildirimi.</summary>
    Leave,
    /// <summary>Herkese giden sohbet mesajı.</summary>
    Chat,
    /// <summary>Tek kullanıcıya giden özel mesaj.</summary>
    Private,
    /// <summary>Çevrimiçi kullanıcı listesi (Text alanında virgülle ayrılmış).</summary>
    UserList,
    /// <summary>Sunucunun bilgi/hata mesajı.</summary>
    System
}
