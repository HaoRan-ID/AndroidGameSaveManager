using System;
using System.IO;
using System.Text.Json;
using System.Drawing;
using System.Windows.Forms;
using AndroidGameSaveManager.Models;

namespace AndroidGameSaveManager.Views
{
    public class UcSettings : UserControl
    {
        private AppSettings _settings;
        private string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        private string profilesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "profiles.json");

        // Warna aksen garis terang (Sesuai warna ungu UI yang lain)
        private Color _accentLineColor = Color.FromArgb(192, 132, 252); // Bright Light Purple
        private int _accentLineThickness = 4; // Ketebalan garis 4px

        // UI Components
        private Label lblTitle;
        private Label lblSubtitleHeader;

        // Card 1: Path & Penyimpanan
        private Panel pnlPathCard;
        private Label lblPathTitle, lblDefaultDir, lblAdbPath;
        private TextBox txtDefaultDir, txtAdbPath;
        private Button btnBrowseDir, btnBrowseAdb;

        // Card 2: Otomasi & Perilaku
        private Panel pnlAutoCard;
        private Label lblAutoTitle;
        private CheckBox chkAutoConnect, chkAutoSubfolder;

        // Card 3: Manajemen Profil JSON
        private Panel pnlDataCard;
        private Label lblDataTitle;
        private Button btnExportProfiles, btnImportProfiles, btnResetProfiles;

        // Card 4: About / Informasi
        private Panel pnlAboutCard;
        private Label lblAboutTitle, lblAppInfo;

        // Tombol Simpan Global
        private Button btnSaveSettings;
        private Label lblStatusMsg;

        public UcSettings()
        {
            _settings = new AppSettings();
            LoadSettings();
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.BackColor = Color.FromArgb(18, 18, 24);
            this.Dock = DockStyle.Fill;
            this.AutoScroll = true;
            this.Padding = new Padding(20);

            // Judul Halaman
            lblTitle = new Label();
            lblTitle.Text = "Settings";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;

            lblSubtitleHeader = new Label();
            lblSubtitleHeader.Text = "Sesuaikan konfigurasi penyimpanan, otomasi, dan data aplikasi.";
            lblSubtitleHeader.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblSubtitleHeader.ForeColor = Color.FromArgb(160, 160, 175);
            lblSubtitleHeader.Location = new Point(20, 45);
            lblSubtitleHeader.AutoSize = true;

            // Helper to generate the accented cards
            pnlPathCard = CreateAccentedCard(new Point(20, 80), new Size(720, 155));
            pnlAutoCard = CreateAccentedCard(new Point(20, 245), new Size(720, 100));
            pnlDataCard = CreateAccentedCard(new Point(20, 355), new Size(720, 95));
            pnlAboutCard = CreateAccentedCard(new Point(20, 460), new Size(720, 80));

            // ==========================================
            // KARTU 1: PATH & PENYIMPANAN - Konten
            // ==========================================
            lblPathTitle = new Label { Text = "Konfigurasi Path & Direktori", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = _accentLineColor, Location = new Point(15, 12), AutoSize = true };

            lblDefaultDir = new Label { Text = "Default Backup Directory (PC):", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(15, 42), AutoSize = true };
            txtDefaultDir = new TextBox { Location = new Point(15, 64), Size = new Size(585, 25), Font = new Font("Segoe UI", 9.5F), BackColor = Color.FromArgb(35, 35, 48), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Text = _settings.DefaultBackupDir };

            btnBrowseDir = new Button { Text = "Browse", Location = new Point(610, 62), Size = new Size(95, 28), BackColor = Color.FromArgb(75, 46, 131), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnBrowseDir.FlatAppearance.BorderSize = 0;
            btnBrowseDir.Click += (s, e) => {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == DialogResult.OK) txtDefaultDir.Text = fbd.SelectedPath;
                }
            };

            lblAdbPath = new Label { Text = "Custom ADB Path (Opsional - Kosongkan jika pakai bawaan):", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(15, 98), AutoSize = true };
            txtAdbPath = new TextBox { Location = new Point(15, 120), Size = new Size(585, 25), Font = new Font("Segoe UI", 9.5F), BackColor = Color.FromArgb(35, 35, 48), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Text = _settings.CustomAdbPath };

            btnBrowseAdb = new Button { Text = "Browse", Location = new Point(610, 118), Size = new Size(95, 28), BackColor = Color.FromArgb(75, 46, 131), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnBrowseAdb.FlatAppearance.BorderSize = 0;
            btnBrowseAdb.Click += (s, e) => {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.FileName = "adb.exe";
                    if (ofd.ShowDialog() == DialogResult.OK) txtAdbPath.Text = ofd.FileName;
                }
            };
            pnlPathCard.Controls.AddRange(new Control[] { lblPathTitle, lblDefaultDir, txtDefaultDir, btnBrowseDir, lblAdbPath, txtAdbPath, btnBrowseAdb });

            // ==========================================
            // KARTU 2: OTOMASI & PERILAKU - Konten
            // ==========================================
            lblAutoTitle = new Label { Text = "Otomasi & Perilaku Aplikasi", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = _accentLineColor, Location = new Point(15, 12), AutoSize = true };
            chkAutoConnect = new CheckBox { Text = "Auto-Connect perangkat saat aplikasi dibuka", Location = new Point(18, 42), Size = new Size(400, 24), Font = new Font("Segoe UI", 9.5F), ForeColor = Color.White, Checked = _settings.AutoConnect };
            chkAutoSubfolder = new CheckBox { Text = "Auto-Create Subfolder berdasarkan nama game saat backup", Location = new Point(18, 70), Size = new Size(450, 24), Font = new Font("Segoe UI", 9.5F), ForeColor = Color.White, Checked = _settings.AutoSubfolder };
            pnlAutoCard.Controls.AddRange(new Control[] { lblAutoTitle, chkAutoConnect, chkAutoSubfolder });

            // ==========================================
            // KARTU 3: MANAJEMEN PROFIL JSON - Konten
            // ==========================================
            lblDataTitle = new Label { Text = "Manajemen Data Profil Game (profiles.json)", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = _accentLineColor, Location = new Point(15, 12), AutoSize = true };
            btnExportProfiles = new Button { Text = "Export / Backup JSON", Location = new Point(15, 45), Size = new Size(160, 35), BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnExportProfiles.FlatAppearance.BorderSize = 0;
            btnExportProfiles.Click += BtnExportProfiles_Click;

            btnImportProfiles = new Button { Text = "Import / Restore JSON", Location = new Point(185, 45), Size = new Size(160, 35), BackColor = Color.FromArgb(245, 158, 11), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnImportProfiles.FlatAppearance.BorderSize = 0;
            btnImportProfiles.Click += BtnImportProfiles_Click;

            btnResetProfiles = new Button { Text = "Reset Data Profil", Location = new Point(355, 45), Size = new Size(140, 35), BackColor = Color.FromArgb(239, 68, 68), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnResetProfiles.FlatAppearance.BorderSize = 0;
            btnResetProfiles.Click += BtnResetProfiles_Click;
            pnlDataCard.Controls.AddRange(new Control[] { lblDataTitle, btnExportProfiles, btnImportProfiles, btnResetProfiles });

            // ==========================================
            // KARTU 4: ABOUT / INFORMASI - Konten
            // ==========================================
            lblAboutTitle = new Label { Text = "Tentang Aplikasi", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = _accentLineColor, Location = new Point(15, 12), AutoSize = true };
            lblAppInfo = new Label { Text = "Android Game Save Manager v1.0.0 Stable\nPlatform: C# .NET 8.0 Windows Forms (Portable ADB Built-in)", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(160, 160, 175), Location = new Point(15, 38), AutoSize = true };
            pnlAboutCard.Controls.AddRange(new Control[] { lblAboutTitle, lblAppInfo });

            // ==========================================
            // TOMBOL SIMPAN GLOBAL SETTINGS
            // ==========================================
            btnSaveSettings = new Button { Text = "Simpan Pengaturan", Location = new Point(20, 555), Size = new Size(180, 42), BackColor = Color.FromArgb(124, 58, 237), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSaveSettings.FlatAppearance.BorderSize = 0;
            btnSaveSettings.Click += BtnSaveSettings_Click;

            lblStatusMsg = new Label { Text = "", Location = new Point(215, 565), Font = new Font("Segoe UI", 9.5F, FontStyle.Italic), ForeColor = Color.FromArgb(52, 211, 153), AutoSize = true };

            // Responsif Lebar Panel saat Resize Jendela
            this.Resize += (s, e) => {
                int availableWidth = this.ClientSize.Width - 40;
                if (availableWidth > 600)
                {
                    pnlPathCard.Width = availableWidth;
                    pnlAutoCard.Width = availableWidth;
                    pnlDataCard.Width = availableWidth;
                    pnlAboutCard.Width = availableWidth;

                    txtDefaultDir.Width = availableWidth - 135;
                    btnBrowseDir.Location = new Point(availableWidth - 110, 62);

                    txtAdbPath.Width = availableWidth - 135;
                    btnBrowseAdb.Location = new Point(availableWidth - 110, 118);
                }
            };

            // Tambahkan semua kontrol ke UserControl
            this.Controls.AddRange(new Control[] { lblTitle, lblSubtitleHeader, pnlPathCard, pnlAutoCard, pnlDataCard, pnlAboutCard, btnSaveSettings, lblStatusMsg });
        }

        // Helper Method: Membuat panel kartu dengan garis aksen di atasnya
        private Panel CreateAccentedCard(Point location, Size size)
        {
            Panel pnl = new Panel();
            pnl.Location = location;
            pnl.Size = size;
            pnl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnl.BackColor = Color.FromArgb(24, 24, 35);
            // Beri top-padding internal 4px (setebal garis) agar konten tidak tertutup garis
            pnl.Padding = new Padding(0, _accentLineThickness, 0, 0);

            // Subscribe ke event Paint untuk menggambar garis aksen di atas kartu
            pnl.Paint += DrawAccentLine;

            return pnl;
        }

        // Helper Event Method: Menggambar garis aksen di bagian paling atas panel
        private void DrawAccentLine(object sender, PaintEventArgs e)
        {
            Panel pnl = sender as Panel;
            if (pnl == null) return;

            // Buat 'Pen' dengan warna aksen terang dan ketebalan yang ditentukan
            using (Pen accentPen = new Pen(_accentLineColor, _accentLineThickness))
            {
                // Gambar garis lurus di koordinat y=0 (paling atas panel) melintasi seluruh lebar panel
                e.Graphics.DrawLine(accentPen, 0, 0, pnl.Width, 0);
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(settingsFilePath))
                {
                    string json = File.ReadAllText(settingsFilePath);
                    var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                    if (loaded != null) _settings = loaded;
                }
            }
            catch { /* Abaikan jika file corrupt/baru */ }
        }

        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                _settings.DefaultBackupDir = txtDefaultDir.Text.Trim();
                _settings.CustomAdbPath = txtAdbPath.Text.Trim();
                _settings.AutoConnect = chkAutoConnect.Checked;
                _settings.AutoSubfolder = chkAutoSubfolder.Checked;

                string json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsFilePath, json);

                lblStatusMsg.Text = "Pengaturan berhasil disimpan!";
                lblStatusMsg.ForeColor = Color.FromArgb(52, 211, 153);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan pengaturan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportProfiles_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "JSON Files (*.json)|*.json";
                sfd.FileName = "profiles_backup.json";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(profilesFilePath))
                    {
                        File.Copy(profilesFilePath, sfd.FileName, true);
                        MessageBox.Show("Profil game berhasil diexport ke file tujuan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Belum ada data profil game yang tersimpan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void BtnImportProfiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON Files (*.json)|*.json";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(ofd.FileName, profilesFilePath, true);
                    MessageBox.Show("Profil game berhasil diimport! Silakan muat ulang halaman game profiles.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnResetProfiles_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Apakah Anda yakin ingin menghapus seluruh daftar profil game?", "Konfirmasi Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                if (File.Exists(profilesFilePath))
                {
                    File.Delete(profilesFilePath);
                }
                MessageBox.Show("Data profil berhasil direset.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}