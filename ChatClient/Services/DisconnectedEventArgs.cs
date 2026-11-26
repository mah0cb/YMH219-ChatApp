namespace ChatClientApp.Services;

/// <summary>Bağlantı kapanışının nedeni ve kullanıcının kendisinin mi kapattığı.</summary>
public class DisconnectedEventArgs : EventArgs
{
    public DisconnectedEventArgs(string reason, bool byUser)
    {
        Reason = reason;
        ByUser = byUser;
    }

    public string Reason { get; }

    public bool ByUser { get; }
}
