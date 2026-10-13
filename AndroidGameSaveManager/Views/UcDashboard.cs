using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AndroidGameSaveManager.Services;

namespace AndroidGameSaveManager.Views
{
    public class UcDashboard : UserControl
    {
        private AdbService _adbService;

        // Warna aksen garis terang (Sesuai tema ungu UI)
        private Color _accentLineColor = Color.FromArgb(192, 132, 252);
        private int _accentLineThickness = 4;

        private Label lblTitle;
        private Label lblSubtitleHeader;
        private Panel pnlConnectionCard;
        private Button btnCheckConnection;
        private Label lblStatus;

        private Panel pnlActionCard;
        private Label lblSectionBackup;
        private Label lblAndroidPath;
        private TextBox txtAndroidPath;
        private Label lblPcPath;
        private TextBox txtPcPath;
        private Button btnBrowse;
        private Button btnBackup;
        private Button btnRestore;

        private Label lblLogTitle;
        private TextBox txtLog;
        private Button btnClearLog;

        public UcDashboard()
        {
            _adbService = new AdbService();
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
            lblTitle.Text = "Dashboard";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;

            lblSubtitleHeader = new Label();
            lblSubtitleHeader.Text = "Kelola backup save game Android melalui USB.";
            lblSubtitleHeader.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblSubtitleHeader.ForeColor = Color.FromArgb(160, 160, 175);
            lblSubtitleHeader.Location = new Point(20, 45);
            lblSubtitleHeader.AutoSize = true;

            // 1. KARTU KONEKSI PERANGKAT (Menggunakan Accented Card)
            pnlConnectionCard = CreateAccentedCard(new Point(20, 75), new Size(720, 80));

            Label lblCardTitle1 = new Label { Text = "Koneksi perangkat Android", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = _accentLineColor, Location = new Point(15, 12), AutoSize = true };
            pnlConnectionCard.Controls.Add(lblCardTitle1);

            btnCheckConnection = new Button();
            btnCheckConnection.Text = "Cek Koneksi USB";
            btnCheckConnection.Size = new Size(145, 38);
            pnlConnectionCard.Resize += (s, e) => {
                btnCheckConnection.Location = new Point(pnlConnectionCard.Width - 160, 22);
            };
            btnCheckConnection.Location = new Point(540, 22);
            btnCheckConnection.BackColor = Color.FromArgb(124, 58, 237);
            btnCheckConnection.ForeColor = Color.White;
            btnCheckConnection.FlatStyle = FlatStyle.Flat;
            btnCheckConnection.FlatAppearance.BorderSize = 0;
            btnCheckConnection.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCheckConnection.Cursor = Cursors.Hand;
            btnCheckConnection.Click += BtnCheckConnection_Click;

            lblStatus = new Label();
            lblStatus.Text = "Status: Belum diperiksa";
            lblStatus.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblStatus.ForeColor = Color.FromArgb(200, 200, 210);
            lblStatus.Location = new Point(15, 42);
            lblStatus.AutoSize = true;

            pnlConnectionCard.Controls.Add(btnCheckConnection);
            pnlConnectionCard.Controls.Add(lblStatus);

            // 2. KARTU AKSI BACKUP & RESTORE (Menggunakan Accented Card)
            pnlActionCard = CreateAccentedCard(new Point(20, 165), new Size(720, 210));

            lblSectionBackup = new Label();
            lblSectionBackup.Text = "Pengaturan Path Android dan Backup";
            lblSectionBackup.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblSectionBackup.ForeColor = _accentLineColor;
            lblSectionBackup.Location = new Point(15, 12);
            lblSectionBackup.AutoSize = true;

            lblAndroidPath = new Label();
            lblAndroidPath.Text = "Path folder Android";
            lblAndroidPath.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblAndroidPath.ForeColor = Color.White;
            lblAndroidPath.Location = new Point(15, 42);
            lblAndroidPath.AutoSize = true;

            txtAndroidPath = new TextBox();
            txtAndroidPath.Location = new Point(15, 64);
            txtAndroidPath.Height = 25;
            pnlActionCard.Resize += (s, e) => {
                txtAndroidPath.Width = pnlActionCard.Width - 30;
                txtPcPath.Width = pnlActionCard.Width - 135;
                btnBrowse.Location = new Point(pnlActionCard.Width - 110, 122);
                btnClearLog.Location = new Point(this.Width - 145, 388);
            };
            txtAndroidPath.Font = new Font("Segoe UI", 9.5F);
            txtAndroidPath.BackColor = Color.FromArgb(35, 35, 48);
            txtAndroidPath.ForeColor = Color.White;
            txtAndroidPath.BorderStyle = BorderStyle.FixedSingle;
            txtAndroidPath.Text = "/sdcard/Download";

            lblPcPath = new Label();
            lblPcPath.Text = "Folder tujuan backup di PC";
            lblPcPath.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPcPath.ForeColor = Color.White;
            lblPcPath.Location = new Point(15, 102);
            lblPcPath.AutoSize = true;

            txtPcPath = new TextBox();
            txtPcPath.Location = new Point(15, 124);
            txtPcPath.Height = 25;
            txtPcPath.Font = new Font("Segoe UI", 9.5F);
            txtPcPath.BackColor = Color.FromArgb(35, 35, 48);
            txtPcPath.ForeColor = Color.White;
            txtPcPath.BorderStyle = BorderStyle.FixedSingle;
            txtPcPath.Text = @"C:\AGSManager"; // <-- Pastikan di sini sudah @"C:\AGSManager"

            btnBrowse = new Button();
            btnBrowse.Text = "Browse";
            btnBrowse.Location = new Point(575, 122);
            btnBrowse.Size = new Size(95, 28);
            btnBrowse.BackColor = Color.FromArgb(75, 46, 131);
            btnBrowse.ForeColor = Color.White;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.Cursor = Cursors.Hand;
            btnBrowse.Click += (s, e) => {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        txtPcPath.Text = fbd.SelectedPath;
                    }
                }
            };

            btnBackup = new Button();
            btnBackup.Text = "Backup (Pull)";
            btnBackup.Location = new Point(15, 160);
            btnBackup.Size = new Size(130, 35);
            btnBackup.BackColor = Color.FromArgb(124, 58, 237);
            btnBackup.ForeColor = Color.White;
            btnBackup.FlatStyle = FlatStyle.Flat;
            btnBackup.FlatAppearance.BorderSize = 0;
            btnBackup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBackup.Cursor = Cursors.Hand;
            btnBackup.Click += BtnBackup_Click;

            btnRestore = new Button();
            btnRestore.Text = "Restore (Push)";
            btnRestore.Location = new Point(155, 160);
            btnRestore.Size = new Size(130, 35);
            btnRestore.BackColor = Color.FromArgb(147, 51, 234);
            btnRestore.ForeColor = Color.White;
            btnRestore.FlatStyle = FlatStyle.Flat;
            btnRestore.FlatAppearance.BorderSize = 0;
            btnRestore.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRestore.Cursor = Cursors.Hand;
            btnRestore.Click += BtnRestore_Click;

            pnlActionCard.Controls.Add(lblSectionBackup);
            pnlActionCard.Controls.Add(lblAndroidPath);
            pnlActionCard.Controls.Add(txtAndroidPath);
            pnlActionCard.Controls.Add(lblPcPath);
            pnlActionCard.Controls.Add(txtPcPath);
            pnlActionCard.Controls.Add(btnBrowse);
            pnlActionCard.Controls.Add(btnBackup);
            pnlActionCard.Controls.Add(btnRestore);

            // 3. KONSOL TERMINAL LOG
            lblLogTitle = new Label();
            lblLogTitle.Text = "Console Log Terminal";
            lblLogTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblLogTitle.ForeColor = Color.White;
            lblLogTitle.Location = new Point(20, 390);
            lblLogTitle.AutoSize = true;

            btnClearLog = new Button();
            btnClearLog.Text = "Bersihkan Log";
            btnClearLog.Location = new Point(575, 388);
            btnClearLog.Size = new Size(125, 28);
            btnClearLog.BackColor = Color.FromArgb(35, 35, 48);
            btnClearLog.ForeColor = Color.White;
            btnClearLog.FlatStyle = FlatStyle.Flat;
            btnClearLog.FlatAppearance.BorderSize = 0;
            btnClearLog.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            btnClearLog.Cursor = Cursors.Hand;
            btnClearLog.Click += (s, e) => txtLog.Clear();

            txtLog = new TextBox();
            txtLog.Multiline = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.ReadOnly = true;
            txtLog.Location = new Point(20, 425);
            txtLog.Height = 130;
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.BackColor = Color.FromArgb(13, 13, 18);
            txtLog.ForeColor = Color.FromArgb(52, 211, 153);
            txtLog.Font = new Font("Consolas", 9.5F, FontStyle.Regular);
            txtLog.BorderStyle = BorderStyle.None;
            txtLog.Text = "[System] Aplikasi siap digunakan.\r\n";

            this.Resize += (s, e) => {
                int availableWidth = this.ClientSize.Width - 40;
                if (availableWidth > 500)
                {
                    pnlConnectionCard.Width = availableWidth;
                    pnlActionCard.Width = availableWidth;
                    txtLog.Width = availableWidth;
                }
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitleHeader);
            this.Controls.Add(pnlConnectionCard);
            this.Controls.Add(pnlActionCard);
            this.Controls.Add(lblLogTitle);
            this.Controls.Add(btnClearLog);
            this.Controls.Add(txtLog);
        }

        // Helper Method untuk Kartu Bergaris Aksen
        private Panel CreateAccentedCard(Point location, Size size)
        {
            Panel pnl = new Panel();
            pnl.Location = location;
            pnl.Size = size;
            pnl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnl.BackColor = Color.FromArgb(24, 24, 35);
            pnl.Padding = new Padding(0, _accentLineThickness, 0, 0);
            pnl.Paint += DrawAccentLine;
            return pnl;
        }

        private void DrawAccentLine(object sender, PaintEventArgs e)
        {
            Panel pnl = sender as Panel;
            if (pnl == null) return;
            using (Pen accentPen = new Pen(_accentLineColor, _accentLineThickness))
            {
                e.Graphics.DrawLine(accentPen, 0, 0, pnl.Width, 0);
            }
        }

        private void AppendLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            txtLog.AppendText($"[{timestamp}] {message}\r\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void BtnCheckConnection_Click(object sender, EventArgs e)
        {
            bool isConnected = _adbService.IsDeviceConnected();
            if (isConnected)
            {
                lblStatus.Text = "Status: Perangkat Terhubung! (USB Ready)";
                lblStatus.ForeColor = Color.FromArgb(52, 211, 153);
                AppendLog("Koneksi OK: Perangkat Android terdeteksi.");
            }
            else
            {
                lblStatus.Text = "Status: Perangkat Tidak Terhubung / USB Debugging Belum Aktif.";
                lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
                AppendLog("Koneksi Gagal: Perangkat tidak ditemukan.");
            }
        }

        private void BtnBackup_Click(object sender, EventArgs e)
        {
            string androidPath = txtAndroidPath.Text.Trim();
            string pcPath = txtPcPath.Text.Trim();

            if (string.IsNullOrEmpty(androidPath) || string.IsNullOrEmpty(pcPath))
            {
                MessageBox.Show("Path Android dan PC tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!Directory.Exists(pcPath))
                {
                    Directory.CreateDirectory(pcPath);
                }

                AppendLog($"Memulai Backup (Pull)... Dari: {androidPath} -> Ke: {pcPath}");
                string result = _adbService.PullSaveData(androidPath, pcPath);

                if (string.IsNullOrEmpty(result))
                {
                    AppendLog("Backup Selesai: Berhasil disalin tanpa pesan error.");
                }
                else
                {
                    AppendLog("Output ADB:\n" + result.Trim());
                }
            }
            catch (Exception ex)
            {
                AppendLog("ERROR Exception: " + ex.Message);
            }
        }

        private void BtnRestore_Click(object sender, EventArgs e)
        {
            string pcPath = txtPcPath.Text.Trim();
            string androidPath = txtAndroidPath.Text.Trim();

            if (string.IsNullOrEmpty(pcPath) || string.IsNullOrEmpty(androidPath))
            {
                MessageBox.Show("Path PC dan Android tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Apakah Anda yakin ingin melakukan restore data ke HP?", "Konfirmasi Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                AppendLog($"Memulai Restore (Push)... Dari: {pcPath} -> Ke: {androidPath}");
                string result = _adbService.PushSaveData(pcPath, androidPath);

                if (string.IsNullOrEmpty(result))
                {
                    AppendLog("Restore Selesai: Berhasil dikirim tanpa pesan error.");
                }
                else
                {
                    AppendLog("Output ADB:\n" + result.Trim());
                }
            }
        }
    }
}