using System.Net.Sockets;
using ChatClientApp.Services;
using ChatCommon.Models;

namespace ChatClientApp.Forms;

/// <summary>İstemci ana penceresi: bağlantı, mesaj listesi ve gönderme.</summary>
public partial class IstemciForm : Form
{
    private readonly ChatClient _client = new();

    public IstemciForm()
    {
        InitializeComponent();
        _client.MessageReceived += Client_MessageReceived;
        _client.Disconnected += Client_Disconnected;
    }

    private async void btnBaglan_Click(object sender, EventArgs e)
    {
        string username = txtKullaniciAdi.Text.Trim();
        if (username.Length == 0)
        {
            MessageBox.Show("Lütfen bir kullanıcı adı girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnBaglan.Enabled = false;
        lblDurum.Text = "Bağlanıyor...";
        try
        {
            await _client.ConnectAsync(txtSunucu.Text.Trim(), (int)nudPort.Value, username);
            SetConnected(true);
        }
        catch (Exception ex) when (ex is SocketException or IOException or ChatConnectionException)
        {
            SetConnected(false);
            MessageBox.Show($"Bağlanılamadı: {ex.Message}", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnBaglantiyiKes_Click(object sender, EventArgs e) => _client.Disconnect();

    private void IstemciForm_FormClosing(object sender, FormClosingEventArgs e) => _client.Disconnect();

    private async void btnGonder_Click(object sender, EventArgs e) => await SendCurrentMessageAsync();

    private async void txtMesaj_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
            return;
        e.SuppressKeyPress = true;
        await SendCurrentMessageAsync();
    }

    private async Task SendCurrentMessageAsync()
    {
        string text = txtMesaj.Text.Trim();
        if (text.Length == 0 || !_client.IsConnected)
            return;
        try
        {
            await _client.SendTextAsync(text);
            txtMesaj.Clear();
        }
        catch (Exception ex) when (ex is IOException or ObjectDisposedException or InvalidOperationException)
        {
            lblDurum.Text = $"Mesaj gönderilemedi: {ex.Message}";
        }
    }

    private void Client_MessageReceived(object? sender, ChatMessage message) => RunOnUi(() =>
    {
        lstMesajlar.Items.Add(message.ToString());
        lstMesajlar.TopIndex = lstMesajlar.Items.Count - 1;
    });

    private void Client_Disconnected(object? sender, string reason) => RunOnUi(() =>
    {
        SetConnected(false);
        lblDurum.Text = reason;
        lstMesajlar.Items.Add($"[{DateTime.Now:HH:mm:ss}] [Sistem] {reason}");
    });

    /// <summary>Arka plan thread'inden gelen olayları UI thread'ine aktarır.</summary>
    private void RunOnUi(Action action)
    {
        if (IsDisposed || !IsHandleCreated)
            return;
        BeginInvoke(action);
    }

    private void SetConnected(bool connected)
    {
        btnBaglan.Enabled = !connected;
        btnBaglantiyiKes.Enabled = connected;
        txtSunucu.Enabled = !connected;
        nudPort.Enabled = !connected;
        txtKullaniciAdi.Enabled = !connected;
        txtMesaj.Enabled = connected;
        btnGonder.Enabled = connected;
        lblDurum.Text = connected ? $"Bağlı: {_client.Username} @ {txtSunucu.Text}:{nudPort.Value}" : "Bağlı değil";
        if (connected)
            txtMesaj.Focus();
        else
            lstKullanicilar.Items.Clear();
    }
}
