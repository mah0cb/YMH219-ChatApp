namespace ChatClientApp.Forms;

partial class IstemciForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        lblSunucu = new Label();
        txtSunucu = new TextBox();
        lblPort = new Label();
        nudPort = new NumericUpDown();
        lblKullaniciAdi = new Label();
        txtKullaniciAdi = new TextBox();
        btnBaglan = new Button();
        btnBaglantiyiKes = new Button();
        lstMesajlar = new ListBox();
        grpKullanicilar = new GroupBox();
        lstKullanicilar = new ListBox();
        txtMesaj = new TextBox();
        btnGonder = new Button();
        btnGecmisiKaydet = new Button();
        sfdGecmis = new SaveFileDialog();
        stsDurum = new StatusStrip();
        lblDurum = new ToolStripStatusLabel();
        ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
        grpKullanicilar.SuspendLayout();
        stsDurum.SuspendLayout();
        SuspendLayout();
        //
        // lblSunucu
        //
        lblSunucu.AutoSize = true;
        lblSunucu.Location = new Point(12, 17);
        lblSunucu.Name = "lblSunucu";
        lblSunucu.Size = new Size(61, 20);
        lblSunucu.TabIndex = 0;
        lblSunucu.Text = "Sunucu:";
        //
        // txtSunucu
        //
        txtSunucu.Location = new Point(79, 14);
        txtSunucu.Name = "txtSunucu";
        txtSunucu.Size = new Size(130, 27);
        txtSunucu.TabIndex = 1;
        txtSunucu.Text = "127.0.0.1";
        //
        // lblPort
        //
        lblPort.AutoSize = true;
        lblPort.Location = new Point(220, 17);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(38, 20);
        lblPort.TabIndex = 2;
        lblPort.Text = "Port:";
        //
        // nudPort
        //
        nudPort.Location = new Point(262, 14);
        nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        nudPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudPort.Name = "nudPort";
        nudPort.Size = new Size(80, 27);
        nudPort.TabIndex = 3;
        nudPort.Value = new decimal(new int[] { 5000, 0, 0, 0 });
        //
        // lblKullaniciAdi
        //
        lblKullaniciAdi.AutoSize = true;
        lblKullaniciAdi.Location = new Point(352, 17);
        lblKullaniciAdi.Name = "lblKullaniciAdi";
        lblKullaniciAdi.Size = new Size(101, 20);
        lblKullaniciAdi.TabIndex = 4;
        lblKullaniciAdi.Text = "Kullanıcı adı:";
        //
        // txtKullaniciAdi
        //
        txtKullaniciAdi.Location = new Point(459, 14);
        txtKullaniciAdi.MaxLength = 20;
        txtKullaniciAdi.Name = "txtKullaniciAdi";
        txtKullaniciAdi.Size = new Size(130, 27);
        txtKullaniciAdi.TabIndex = 5;
        //
        // btnBaglan
        //
        btnBaglan.Location = new Point(600, 12);
        btnBaglan.Name = "btnBaglan";
        btnBaglan.Size = new Size(100, 30);
        btnBaglan.TabIndex = 6;
        btnBaglan.Text = "Bağlan";
        btnBaglan.UseVisualStyleBackColor = true;
        btnBaglan.Click += btnBaglan_Click;
        //
        // btnBaglantiyiKes
        //
        btnBaglantiyiKes.Enabled = false;
        btnBaglantiyiKes.Location = new Point(706, 12);
        btnBaglantiyiKes.Name = "btnBaglantiyiKes";
        btnBaglantiyiKes.Size = new Size(130, 30);
        btnBaglantiyiKes.TabIndex = 7;
        btnBaglantiyiKes.Text = "Bağlantıyı Kes";
        btnBaglantiyiKes.UseVisualStyleBackColor = true;
        btnBaglantiyiKes.Click += btnBaglantiyiKes_Click;
        //
        // lstMesajlar
        //
        lstMesajlar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lstMesajlar.FormattingEnabled = true;
        lstMesajlar.HorizontalScrollbar = true;
        lstMesajlar.Location = new Point(12, 54);
        lstMesajlar.Name = "lstMesajlar";
        lstMesajlar.Size = new Size(640, 384);
        lstMesajlar.TabIndex = 8;
        //
        // grpKullanicilar
        //
        grpKullanicilar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        grpKullanicilar.Controls.Add(lstKullanicilar);
        grpKullanicilar.Location = new Point(660, 48);
        grpKullanicilar.Name = "grpKullanicilar";
        grpKullanicilar.Size = new Size(228, 394);
        grpKullanicilar.TabIndex = 9;
        grpKullanicilar.TabStop = false;
        grpKullanicilar.Text = "Çevrimiçi Kullanıcılar";
        //
        // lstKullanicilar
        //
        lstKullanicilar.Dock = DockStyle.Fill;
        lstKullanicilar.FormattingEnabled = true;
        lstKullanicilar.Location = new Point(3, 23);
        lstKullanicilar.Name = "lstKullanicilar";
        lstKullanicilar.Size = new Size(222, 368);
        lstKullanicilar.TabIndex = 0;
        lstKullanicilar.DoubleClick += lstKullanicilar_DoubleClick;
        //
        // txtMesaj
        //
        txtMesaj.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtMesaj.Enabled = false;
        txtMesaj.Location = new Point(12, 452);
        txtMesaj.MaxLength = 1000;
        txtMesaj.Name = "txtMesaj";
        txtMesaj.PlaceholderText = "Mesaj yazın (özel mesaj: /w kullanıcı mesaj)";
        txtMesaj.Size = new Size(410, 27);
        txtMesaj.TabIndex = 10;
        txtMesaj.KeyDown += txtMesaj_KeyDown;
        //
        // btnGonder
        //
        btnGonder.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnGonder.Enabled = false;
        btnGonder.Location = new Point(428, 450);
        btnGonder.Name = "btnGonder";
        btnGonder.Size = new Size(100, 30);
        btnGonder.TabIndex = 11;
        btnGonder.Text = "Gönder";
        btnGonder.UseVisualStyleBackColor = true;
        btnGonder.Click += btnGonder_Click;
        //
        // btnGecmisiKaydet
        //
        btnGecmisiKaydet.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnGecmisiKaydet.Location = new Point(534, 450);
        btnGecmisiKaydet.Name = "btnGecmisiKaydet";
        btnGecmisiKaydet.Size = new Size(118, 30);
        btnGecmisiKaydet.TabIndex = 13;
        btnGecmisiKaydet.Text = "Geçmişi Kaydet";
        btnGecmisiKaydet.UseVisualStyleBackColor = true;
        btnGecmisiKaydet.Click += btnGecmisiKaydet_Click;
        //
        // sfdGecmis
        //
        sfdGecmis.DefaultExt = "txt";
        sfdGecmis.Filter = "Metin dosyası (*.txt)|*.txt";
        sfdGecmis.Title = "Sohbet geçmişini kaydet";
        //
        // stsDurum
        //
        stsDurum.ImageScalingSize = new Size(20, 20);
        stsDurum.Items.AddRange(new ToolStripItem[] { lblDurum });
        stsDurum.Location = new Point(0, 494);
        stsDurum.Name = "stsDurum";
        stsDurum.Size = new Size(900, 26);
        stsDurum.TabIndex = 12;
        //
        // lblDurum
        //
        lblDurum.Name = "lblDurum";
        lblDurum.Size = new Size(85, 20);
        lblDurum.Text = "Bağlı değil";
        //
        // IstemciForm
        //
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 520);
        Controls.Add(stsDurum);
        Controls.Add(btnGecmisiKaydet);
        Controls.Add(btnGonder);
        Controls.Add(txtMesaj);
        Controls.Add(grpKullanicilar);
        Controls.Add(lstMesajlar);
        Controls.Add(btnBaglantiyiKes);
        Controls.Add(btnBaglan);
        Controls.Add(txtKullaniciAdi);
        Controls.Add(lblKullaniciAdi);
        Controls.Add(nudPort);
        Controls.Add(lblPort);
        Controls.Add(txtSunucu);
        Controls.Add(lblSunucu);
        MinimumSize = new Size(800, 400);
        Name = "IstemciForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Sohbet İstemcisi";
        FormClosing += IstemciForm_FormClosing;
        ((System.ComponentModel.ISupportInitialize)nudPort).EndInit();
        grpKullanicilar.ResumeLayout(false);
        stsDurum.ResumeLayout(false);
        stsDurum.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblSunucu;
    private TextBox txtSunucu;
    private Label lblPort;
    private NumericUpDown nudPort;
    private Label lblKullaniciAdi;
    private TextBox txtKullaniciAdi;
    private Button btnBaglan;
    private Button btnBaglantiyiKes;
    private ListBox lstMesajlar;
    private GroupBox grpKullanicilar;
    private ListBox lstKullanicilar;
    private TextBox txtMesaj;
    private Button btnGonder;
    private Button btnGecmisiKaydet;
    private SaveFileDialog sfdGecmis;
    private StatusStrip stsDurum;
    private ToolStripStatusLabel lblDurum;
}
