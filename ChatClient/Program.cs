using ChatClientApp.Forms;

namespace ChatClientApp;

static class Program
{
    /// <summary>İstemci uygulamasının giriş noktası.</summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new IstemciForm());
    }
}
