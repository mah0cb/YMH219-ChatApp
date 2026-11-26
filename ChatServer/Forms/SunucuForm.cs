using System.Net.Sockets;
using ChatCommon.Models;
using ChatServerApp.Services;

namespace ChatServerApp.Forms;

/// <summary>Sunucu ana penceresi: başlat/durdur, bağlı istemciler ve günlük.</summary>
public partial class SunucuForm : Form
{
    private readonly ChatServer _server = new();

    public SunucuForm()
    {
        InitializeComponent();
        _server.ClientConnected += Server_ClientConnected;
        _server.ClientDisconnected += Server_ClientDisconnected;
        _server.MessageReceived += Server_MessageReceived;
    }

    private async void btnBaslat_Click(object sender, EventArgs e)
    {
        int port = (int)nudPort.Value;
        Task serverTask;
        try
        {
            serverTask = _server.StartAsync(port);
        }
        catch (SocketException ex)
        {
            MessageBox.Show($"Sunucu başlatılamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        SetRunning(true);
        AddLog($"Sunucu {port} portunda dinlemeye başladı.");
        await serverTask;

        if (IsDisposed)
            return;
        SetRunning(false);
        lvIstemciler.Items.Clear();
        AddLog("Sunucu durduruldu.");
    }

    private void btnDurdur_Click(object sender, EventArgs e) => _server.Stop();

    private void SunucuForm_FormClosing(object sender, FormClosingEventArgs e) => _server.Stop();

    private void Server_ClientConnected(object? sender, ClientConnection client) => RunOnUi(() =>
    {
        var item = new ListViewItem(client.Username) { Name = client.Username, Tag = client };
        item.SubItems.Add(client.RemoteEndPoint);
        item.SubItems.Add(client.ConnectedAt.ToString("dd.MM.yyyy HH:mm:ss"));
        lvIstemciler.Items.Add(item);
        AddLog($"{client.Username} bağlandı ({client.RemoteEndPoint}).");
    });

    private void Server_ClientDisconnected(object? sender, ClientConnection client) => RunOnUi(() =>
    {
        foreach (ListViewItem item in lvIstemciler.Items)
        {
            if (item.Tag == client)
            {
                lvIstemciler.Items.Remove(item);
                break;
            }
        }
        AddLog($"{client.Username} ayrıldı ({client.RemoteEndPoint}).");
    });

    private void Server_MessageReceived(object? sender, ChatMessage message) => RunOnUi(() => AddLog(message.ToString()));

    /// <summary>Arka plan thread'inden gelen olayları UI thread'ine aktarır.</summary>
    private void RunOnUi(Action action)
    {
        if (IsDisposed || !IsHandleCreated)
            return;
        BeginInvoke(action);
    }

    private void SetRunning(bool running)
    {
        btnBaslat.Enabled = !running;
        nudPort.Enabled = !running;
        btnDurdur.Enabled = running;
        lblDurum.Text = running ? $"Durum: Çalışıyor (port {nudPort.Value})" : "Durum: Durduruldu";
    }

    private void AddLog(string text)
    {
        string line = text.StartsWith('[') ? text : $"[{DateTime.Now:HH:mm:ss}] {text}";
        lstGunluk.Items.Add(line);
        lstGunluk.TopIndex = lstGunluk.Items.Count - 1;
    }
}
