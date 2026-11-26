namespace ChatServerApp.Forms;

partial class SunucuForm
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
        lblPort = new Label();
        nudPort = new NumericUpDown();
        btnBaslat = new Button();
        btnDurdur = new Button();
        lblDurum = new Label();
        grpIstemciler = new GroupBox();
        lvIstemciler = new ListView();
        colKullaniciAdi = new ColumnHeader();
        colAdres = new ColumnHeader();
        colBaglanmaZamani = new ColumnHeader();
        grpGunluk = new GroupBox();
        lstGunluk = new ListBox();
        ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
        grpIstemciler.SuspendLayout();
        grpGunluk.SuspendLayout();
        SuspendLayout();
        //
        // lblPort
        //
        lblPort.AutoSize = true;
        lblPort.Location = new Point(12, 17);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(38, 20);
        lblPort.TabIndex = 0;
        lblPort.Text = "Port:";
        //
        // nudPort
        //
        nudPort.Location = new Point(56, 14);
        nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        nudPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudPort.Name = "nudPort";
        nudPort.Size = new Size(90, 27);
        nudPort.TabIndex = 1;
        nudPort.Value = new decimal(new int[] { 5000, 0, 0, 0 });
        //
        // btnBaslat
        //
        btnBaslat.Location = new Point(160, 12);
        btnBaslat.Name = "btnBaslat";
        btnBaslat.Size = new Size(100, 30);
        btnBaslat.TabIndex = 2;
        btnBaslat.Text = "Başlat";
        btnBaslat.UseVisualStyleBackColor = true;
        btnBaslat.Click += btnBaslat_Click;
        //
        // btnDurdur
        //
        btnDurdur.Enabled = false;
        btnDurdur.Location = new Point(266, 12);
        btnDurdur.Name = "btnDurdur";
        btnDurdur.Size = new Size(100, 30);
        btnDurdur.TabIndex = 3;
        btnDurdur.Text = "Durdur";
        btnDurdur.UseVisualStyleBackColor = true;
        btnDurdur.Click += btnDurdur_Click;
        //
        // lblDurum
        //
        lblDurum.AutoSize = true;
        lblDurum.Location = new Point(384, 17);
        lblDurum.Name = "lblDurum";
        lblDurum.Size = new Size(144, 20);
        lblDurum.TabIndex = 4;
        lblDurum.Text = "Durum: Durduruldu";
        //
        // grpIstemciler
        //
        grpIstemciler.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        grpIstemciler.Controls.Add(lvIstemciler);
        grpIstemciler.Location = new Point(12, 52);
        grpIstemciler.Name = "grpIstemciler";
        grpIstemciler.Size = new Size(420, 436);
        grpIstemciler.TabIndex = 5;
        grpIstemciler.TabStop = false;
        grpIstemciler.Text = "Bağlı İstemciler";
        //
        // lvIstemciler
        //
        lvIstemciler.Columns.AddRange(new ColumnHeader[] { colKullaniciAdi, colAdres, colBaglanmaZamani });
        lvIstemciler.Dock = DockStyle.Fill;
        lvIstemciler.FullRowSelect = true;
        lvIstemciler.GridLines = true;
        lvIstemciler.Location = new Point(3, 23);
        lvIstemciler.Name = "lvIstemciler";
        lvIstemciler.Size = new Size(414, 410);
        lvIstemciler.TabIndex = 0;
        lvIstemciler.UseCompatibleStateImageBehavior = false;
        lvIstemciler.View = View.Details;
        //
        // colKullaniciAdi
        //
        colKullaniciAdi.Text = "Kullanıcı Adı";
        colKullaniciAdi.Width = 130;
        //
        // colAdres
        //
        colAdres.Text = "IP:Port";
        colAdres.Width = 150;
        //
        // colBaglanmaZamani
        //
        colBaglanmaZamani.Text = "Bağlanma Zamanı";
        colBaglanmaZamani.Width = 130;
        //
        // grpGunluk
        //
        grpGunluk.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpGunluk.Controls.Add(lstGunluk);
        grpGunluk.Location = new Point(440, 52);
        grpGunluk.Name = "grpGunluk";
        grpGunluk.Size = new Size(448, 436);
        grpGunluk.TabIndex = 6;
        grpGunluk.TabStop = false;
        grpGunluk.Text = "Günlük";
        //
        // lstGunluk
        //
        lstGunluk.Dock = DockStyle.Fill;
        lstGunluk.FormattingEnabled = true;
        lstGunluk.HorizontalScrollbar = true;
        lstGunluk.Location = new Point(3, 23);
        lstGunluk.Name = "lstGunluk";
        lstGunluk.Size = new Size(442, 410);
        lstGunluk.TabIndex = 0;
        //
        // SunucuForm
        //
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 500);
        Controls.Add(grpGunluk);
        Controls.Add(grpIstemciler);
        Controls.Add(lblDurum);
        Controls.Add(btnDurdur);
        Controls.Add(btnBaslat);
        Controls.Add(nudPort);
        Controls.Add(lblPort);
        MinimumSize = new Size(700, 400);
        Name = "SunucuForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Sohbet Sunucusu";
        FormClosing += SunucuForm_FormClosing;
        ((System.ComponentModel.ISupportInitialize)nudPort).EndInit();
        grpIstemciler.ResumeLayout(false);
        grpGunluk.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblPort;
    private NumericUpDown nudPort;
    private Button btnBaslat;
    private Button btnDurdur;
    private Label lblDurum;
    private GroupBox grpIstemciler;
    private ListView lvIstemciler;
    private ColumnHeader colKullaniciAdi;
    private ColumnHeader colAdres;
    private ColumnHeader colBaglanmaZamani;
    private GroupBox grpGunluk;
    private ListBox lstGunluk;
}
