using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using AndroidGameSaveManager.Models;
using AndroidGameSaveManager.Services;

namespace AndroidGameSaveManager.Views
{
    public class UcGameProfiles : UserControl
    {
        private AdbService _adbService;
        private List<GameProfile> _profiles;
        private string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "profiles.json");

        private Color _accentLineColor = Color.FromArgb(192, 132, 252);
        private int _accentLineThickness = 4;

        private Label lblTitle;
        private ListBox lstProfiles;

        private Panel pnlFormCard;
        private Label lblName, lblAndroid, lblPc;
        private TextBox txtName, txtAndroid, txtPc;
        private Button btnDetectPackage;
        private Button btnAdd, btnDelete, btnBackupSelected, btnRestoreSelected;
        private Label lblLog;

        public UcGameProfiles()
        {
            _adbService = new AdbService();
            _profiles = new List<GameProfile>();

            InitializeComponentCustom();
            LoadProfiles();
        }

        private void InitializeComponentCustom()
        {
            this.BackColor = Color.FromArgb(18, 18, 24);
            this.Dock = DockStyle.Fill;
            this.AutoScroll = true;
            this.Padding = new Padding(20);

            lblTitle = new Label();
            lblTitle.Text = "Game Profiles";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;

            lstProfiles = new ListBox();
            lstProfiles.Location = new Point(20, 60);
            lstProfiles.Size = new Size(220, 440);
            lstProfiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lstProfiles.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lstProfiles.BackColor = Color.FromArgb(24, 24, 35);
            lstProfiles.ForeColor = Color.White;
            lstProfiles.BorderStyle = BorderStyle.FixedSingle;
            lstProfiles.SelectedIndexChanged += LstProfiles_SelectedIndexChanged;

            pnlFormCard = CreateAccentedCard(new Point(260, 60), new Size(460, 380));
            pnlFormCard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            int labelX = 20;
            int inputX = 20;

            lblName = new Label { Text = "Nama Game:", Location = new Point(labelX, 20), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = _accentLineColor, AutoSize = true };
            txtName = new TextBox { Location = new Point(inputX, 42), Size = new Size(420, 25), Font = new Font("Segoe UI", 9.5F), BackColor = Color.FromArgb(35, 35, 48), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            // TAMBAHKAN EVENT INI DI BAWAHNYA:
            txtName.TextChanged += (s, e) => {
                if (!string.IsNullOrEmpty(txtName.Text.Trim()))
                {
                    // Setiap nama game diketik, path PC otomatis menyesuaikan ke C:\AGSManager\NamaGame
                    txtPc.Text = Path.Combine(@"C:\AGSManager", txtName.Text.Trim());
                }
            };

            lblAndroid = new Label { Text = "Path Folder Android:", Location = new Point(labelX, 82), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = _accentLineColor, AutoSize = true };

            // Tombol Deteksi diposisikan di sebelah kanan sejajar dengan label Android
            btnDetectPackage = new Button { Text = "🔍 Deteksi dari HP", Location = new Point(315, 78), Size = new Size(125, 26), BackColor = Color.FromArgb(75, 46, 131), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnDetectPackage.FlatAppearance.BorderSize = 0;
            btnDetectPackage.Click += BtnDetectPackage_Click;

            txtAndroid = new TextBox { Location = new Point(inputX, 108), Size = new Size(420, 25), Font = new Font("Segoe UI", 9.5F), BackColor = Color.FromArgb(35, 35, 48), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            lblPc = new Label { Text = "Path Folder PC:", Location = new Point(labelX, 148), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = _accentLineColor, AutoSize = true };
            txtPc = new TextBox { Location = new Point(inputX, 170), Size = new Size(420, 25), Font = new Font("Segoe UI", 9.5F), BackColor = Color.FromArgb(35, 35, 48), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            // Tombol Aksi Form
            btnAdd = new Button { Text = "Simpan Profil", Location = new Point(20, 225), Size = new Size(200, 38), BackColor = Color.FromArgb(124, 58, 237), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            btnDelete = new Button { Text = "Hapus Profil", Location = new Point(240, 225), Size = new Size(200, 38), BackColor = Color.FromArgb(190, 24, 93), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            // Tombol Eksekusi Cepat
            btnBackupSelected = new Button { Text = "Backup Game Ini", Location = new Point(20, 285), Size = new Size(200, 42), BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnBackupSelected.FlatAppearance.BorderSize = 0;
            btnBackupSelected.Click += BtnBackupSelected_Click;

            btnRestoreSelected = new Button { Text = "Restore Game Ini", Location = new Point(240, 285), Size = new Size(200, 42), BackColor = Color.FromArgb(245, 158, 11), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnRestoreSelected.FlatAppearance.BorderSize = 0;
            btnRestoreSelected.Click += BtnRestoreSelected_Click;

            pnlFormCard.Controls.Add(lblName);
            pnlFormCard.Controls.Add(txtName);
            pnlFormCard.Controls.Add(lblAndroid);
            pnlFormCard.Controls.Add(btnDetectPackage);
            pnlFormCard.Controls.Add(txtAndroid);
            pnlFormCard.Controls.Add(lblPc);
            pnlFormCard.Controls.Add(txtPc);
            pnlFormCard.Controls.Add(btnAdd);
            pnlFormCard.Controls.Add(btnDelete);
            pnlFormCard.Controls.Add(btnBackupSelected);
            pnlFormCard.Controls.Add(btnRestoreSelected);

            pnlFormCard.Resize += (s, e) => {
                int formWidth = pnlFormCard.Width;
                if (formWidth > 400)
                {
                    txtName.Width = formWidth - 40;
                    txtAndroid.Width = formWidth - 40;
                    txtPc.Width = formWidth - 40;

                    // Tombol deteksi otomatis menempel di sisi kanan secara dinamis
                    btnDetectPackage.Location = new Point(formWidth - 145, 78);

                    int halfWidth = (formWidth - 50) / 2;
                    if (halfWidth > 150)
                    {
                        btnAdd.Width = halfWidth;
                        btnDelete.Width = halfWidth;
                        btnDelete.Location = new Point(halfWidth + 30, btnDelete.Location.Y);

                        btnBackupSelected.Width = halfWidth;
                        btnRestoreSelected.Width = halfWidth;
                        btnRestoreSelected.Location = new Point(halfWidth + 30, btnRestoreSelected.Location.Y);
                    }
                }
            };

            lblLog = new Label { Text = "Log: Pilih game dari daftar atau buat profil baru.", Location = new Point(20, 515), Font = new Font("Segoe UI", 9, FontStyle.Italic), ForeColor = Color.FromArgb(160, 160, 175), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, AutoSize = true, MaximumSize = new Size(700, 0) };

            this.Resize += (s, e) => {
                int totalWidth = this.ClientSize.Width;
                if (totalWidth > 520)
                {
                    pnlFormCard.Width = totalWidth - 290;
                }
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lstProfiles);
            this.Controls.Add(pnlFormCard);
            this.Controls.Add(lblLog);
        }

        private void BtnDetectPackage_Click(object sender, EventArgs e)
        {
            if (!_adbService.IsDeviceConnected())
            {
                MessageBox.Show("Perangkat Android tidak terhubung! Pastikan USB Debugging aktif.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var packages = _adbService.GetInstalledPackages();
            if (packages.Count == 0)
            {
                MessageBox.Show("Tidak dapat mendeteksi aplikasi atau perangkat kosong.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (Form dlg = new Form())
            {
                dlg.Text = "Pilih Aplikasi / Game Terpasang";
                dlg.Size = new Size(450, 500);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.BackColor = Color.FromArgb(18, 18, 24);
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;

                Label lblInfo = new Label { Text = "Pilih package game Anda dari daftar di bawah:", Location = new Point(15, 15), ForeColor = Color.White, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), AutoSize = true };

                TextBox txtSearch = new TextBox { Location = new Point(15, 42), Size = new Size(405, 25), BackColor = Color.FromArgb(35, 35, 48), ForeColor = Color.White, Font = new Font("Segoe UI", 9.5F), BorderStyle = BorderStyle.FixedSingle };
                txtSearch.PlaceholderText = "Cari package name...";

                ListBox lstBox = new ListBox { Location = new Point(15, 75), Size = new Size(405, 330), BackColor = Color.FromArgb(24, 24, 35), ForeColor = Color.FromArgb(52, 211, 153), Font = new Font("Consolas", 9.5F), BorderStyle = BorderStyle.FixedSingle };

                foreach (var pkg in packages) lstBox.Items.Add(pkg);

                txtSearch.TextChanged += (s, ev) => {
                    lstBox.Items.Clear();
                    foreach (var pkg in packages)
                    {
                        if (pkg.ToLower().Contains(txtSearch.Text.ToLower()))
                        {
                            lstBox.Items.Add(pkg);
                        }
                    }
                };

                Button btnSelect = new Button { Text = "Gunakan Package Ini", Location = new Point(255, 415), Size = new Size(165, 35), BackColor = Color.FromArgb(124, 58, 237), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand };
                btnSelect.FlatAppearance.BorderSize = 0;

                btnSelect.Click += (s, ev) => {
                    if (lstBox.SelectedItem != null)
                    {
                        string selectedPkg = lstBox.SelectedItem.ToString();
                        txtAndroid.Text = $"/sdcard/Android/data/{selectedPkg}/files";

                        if (string.IsNullOrEmpty(txtName.Text.Trim()))
                        {
                            string[] parts = selectedPkg.Split('.');
                            if (parts.Length > 0) txtName.Text = parts[parts.Length - 1];
                        }

                        // TAMBAHKAN BARIS INI: Agar path PC otomatis terisi ke C:\AGSManager\NamaGame
                        string gameNameForPath = string.IsNullOrEmpty(txtName.Text) ? "GameSave" : txtName.Text.Trim();
                        txtPc.Text = Path.Combine(@"C:\AGSManager", gameNameForPath);

                        dlg.DialogResult = DialogResult.OK;
                        dlg.Close();
                    }
                    // ...
                };

                dlg.Controls.AddRange(new Control[] { lblInfo, txtSearch, lstBox, btnSelect });
                dlg.ShowDialog();
            }
        }

        private Panel CreateAccentedCard(Point location, Size size)
        {
            Panel pnl = new Panel();
            pnl.Location = location;
            pnl.Size = size;
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

        private void LoadProfiles()
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    string jsonString = File.ReadAllText(jsonFilePath);
                    var profiles = JsonSerializer.Deserialize<List<GameProfile>>(jsonString);
                    if (profiles != null)
                    {
                        _profiles = profiles;
                        RefreshListBox();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat profil: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveProfilesToFile()
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(_profiles, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonFilePath, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan profil: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshListBox()
        {
            lstProfiles.Items.Clear();
            foreach (var profile in _profiles)
            {
                lstProfiles.Items.Add(profile);
            }
        }

        private void LstProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstProfiles.SelectedItem is GameProfile selected)
            {
                txtName.Text = selected.GameName;
                txtAndroid.Text = selected.AndroidPath;
                txtPc.Text = selected.PcPath;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string android = txtAndroid.Text.Trim();
            string pc = txtPc.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(android) || string.IsNullOrEmpty(pc))
            {
                MessageBox.Show("Semua kolom harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existing = _profiles.Find(p => p.GameName.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.AndroidPath = android;
                existing.PcPath = pc;
                lblLog.Text = $"Profil '{name}' berhasil diperbarui.";
            }
            else
            {
                _profiles.Add(new GameProfile { GameName = name, AndroidPath = android, PcPath = pc });
                lblLog.Text = $"Profil '{name}' berhasil ditambahkan.";
            }

            SaveProfilesToFile();
            RefreshListBox();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstProfiles.SelectedItem is GameProfile selected)
            {
                _profiles.Remove(selected);
                SaveProfilesToFile();
                RefreshListBox();

                txtName.Clear();
                txtAndroid.Clear();
                txtPc.Clear();
                lblLog.Text = $"Profil '{selected.GameName}' dihapus.";
            }
            else
            {
                MessageBox.Show("Pilih game yang ingin dihapus dari daftar terlebih dahulu.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnBackupSelected_Click(object sender, EventArgs e)
        {
            if (lstProfiles.SelectedItem is GameProfile selected)
            {
                try
                {
                    if (!Directory.Exists(selected.PcPath))
                    {
                        Directory.CreateDirectory(selected.PcPath);
                    }

                    lblLog.Text = $"Sedang mem-backup '{selected.GameName}'...";
                    string result = _adbService.PullSaveData(selected.AndroidPath, selected.PcPath);

                    lblLog.Text = $"Backup '{selected.GameName}' Selesai! {(string.IsNullOrEmpty(result) ? "Berhasil." : result)}";
                    lblLog.ForeColor = Color.FromArgb(52, 211, 153);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Pilih game dari daftar yang ingin di-backup!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnRestoreSelected_Click(object sender, EventArgs e)
        {
            if (lstProfiles.SelectedItem is GameProfile selected)
            {
                DialogResult confirm = MessageBox.Show($"Yakin ingin me-restore save game untuk '{selected.GameName}' ke HP?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    lblLog.Text = $"Sedang me-restore '{selected.GameName}'...";
                    string result = _adbService.PushSaveData(selected.PcPath, selected.AndroidPath);

                    lblLog.Text = $"Restore '{selected.GameName}' Selesai! {(string.IsNullOrEmpty(result) ? "Berhasil." : result)}";
                    lblLog.ForeColor = Color.FromArgb(245, 158, 11);
                }
            }
            else
            {
                MessageBox.Show("Pilih game dari daftar yang ingin di-restore!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}