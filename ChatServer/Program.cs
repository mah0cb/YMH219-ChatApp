using ChatServerApp.Forms;

namespace ChatServerApp;

static class Program
{
    /// <summary>Sunucu uygulamasının giriş noktası.</summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new SunucuForm());
    }
}
