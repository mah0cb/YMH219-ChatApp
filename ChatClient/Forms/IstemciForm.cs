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
        btnBaglan.Enabled = false;
        lblDurum.Text = "Bağlanıyor...";
        try
        {
            await _client.ConnectAsync(txtSunucu.Text.Trim(), (int)nudPort.Value, txtKullaniciAdi.Text.Trim());
            SetConnected(true);
        }
        catch (ChatConnectionException ex)
        {
            SetConnected(false);
            lblDurum.Text = "Bağlanılamadı";
            MessageBox.Show(ex.Message, "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        catch (ArgumentException ex)
        {
            lblDurum.Text = ex.Message;
        }
        catch (Exception ex) when (ex is IOException or ObjectDisposedException or InvalidOperationException or SocketException)
        {
            // Bağlantı koptuysa okuma döngüsü Disconnected ile ayrıca bildirir.
            lblDurum.Text = "Mesaj gönderilemedi: sunucu bağlantısı kopmuş olabilir.";
        }
    }

    /// <summary>Listeden seçilen kullanıcıya özel mesaj için kutuya "/w ad " yazar.</summary>
    private void lstKullanicilar_DoubleClick(object sender, EventArgs e)
    {
        if (lstKullanicilar.SelectedItem is not string username || !_client.IsConnected)
            return;
        txtMesaj.Text = $"/w {username} ";
        txtMesaj.Focus();
        txtMesaj.SelectionStart = txtMesaj.Text.Length;
    }

    private void btnGecmisiKaydet_Click(object sender, EventArgs e)
    {
        if (lstMesajlar.Items.Count == 0)
        {
            MessageBox.Show("Kaydedilecek mesaj yok.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        sfdGecmis.FileName = $"sohbet_{DateTime.Now:yyyyMMdd_HHmm}.txt";
        if (sfdGecmis.ShowDialog(this) != DialogResult.OK)
            return;
        try
        {
            ChatHistoryExporter.Save(sfdGecmis.FileName, _client.Username, lstMesajlar.Items.Cast<string>());
            lblDurum.Text = $"Geçmiş kaydedildi: {sfdGecmis.FileName}";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show($"Geçmiş kaydedilemedi: {ex.Message}", "Dosya Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Client_MessageReceived(object? sender, ChatMessage message) => RunOnUi(() =>
    {
        if (message.Type == MessageType.UserList)
        {
            lstKullanicilar.Items.Clear();
            lstKullanicilar.Items.AddRange(message.Users);
            grpKullanicilar.Text = $"Çevrimiçi Kullanıcılar ({message.Users.Length})";
            return;
        }
        lstMesajlar.Items.Add(message.ToString());
        lstMesajlar.TopIndex = lstMesajlar.Items.Count - 1;
    });

    private void Client_Disconnected(object? sender, DisconnectedEventArgs e) => RunOnUi(() =>
    {
        SetConnected(false);
        lblDurum.Text = e.Reason;
        lstMesajlar.Items.Add($"[{DateTime.Now:HH:mm:ss}] [Sistem] {e.Reason}");
        if (!e.ByUser)
            MessageBox.Show($"{e.Reason} Sunucu tekrar açıldığında yeniden bağlanabilirsiniz.", "Bağlantı Koptu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
