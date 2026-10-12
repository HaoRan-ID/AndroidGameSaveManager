using System;
using System.Drawing;
using System.Windows.Forms;
using AndroidGameSaveManager.Views;

namespace AndroidGameSaveManager
{
    public class MainForm : Form
    {
        private Panel pnlMenu;
        private Panel pnlContainer;
        private Button btnDashboard;
        private Button btnProfiles;
        private Button btnSettings;
        private Label lblAppTitle;
        private Label lblSubTitle;
        private Label lblMenuHeader;
        private Label lblFooter;

        public MainForm()
        {
            InitializeComponentCustom();
            SwitchUserControl(new UcDashboard());
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Android Game Save Manager";
            this.Size = new Size(960, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(18, 18, 24); // Dark background

            // Sidebar Menu (Dark Violet Theme)
            pnlMenu = new Panel();
            pnlMenu.Dock = DockStyle.Left;
            pnlMenu.Width = 220;
            pnlMenu.BackColor = Color.FromArgb(13, 13, 18);

            // Judul Aplikasi di Sidebar
            lblAppTitle = new Label();
            lblAppTitle.Text = "ANDROID\nSAVE";
            lblAppTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblAppTitle.ForeColor = Color.White;
            lblAppTitle.Location = new Point(20, 20);
            lblAppTitle.AutoSize = true;
            pnlMenu.Controls.Add(lblAppTitle);

            lblSubTitle = new Label();
            lblSubTitle.Text = "GAME UTILITY";
            lblSubTitle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblSubTitle.ForeColor = Color.FromArgb(168, 85, 247); // Purple accent
            lblSubTitle.Location = new Point(20, 65);
            lblSubTitle.AutoSize = true;
            pnlMenu.Controls.Add(lblSubTitle);

            lblMenuHeader = new Label();
            lblMenuHeader.Text = "MAIN MENU";
            lblMenuHeader.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblMenuHeader.ForeColor = Color.FromArgb(148, 163, 184);
            lblMenuHeader.Location = new Point(20, 110);
            lblMenuHeader.AutoSize = true;
            pnlMenu.Controls.Add(lblMenuHeader);

            // Panel Kontainer Konten di Kanan
            pnlContainer = new Panel();
            pnlContainer.Dock = DockStyle.Fill;
            pnlContainer.BackColor = Color.FromArgb(18, 18, 24);
            pnlContainer.Padding = new Padding(10);

            // Tombol Navigasi Menu
            btnDashboard = CreateMenuButton("📊  Dashboard", 140);
            btnDashboard.Click += (s, e) => {
                HighlightButton(btnDashboard);
                SwitchUserControl(new UcDashboard());
            };

            btnProfiles = CreateMenuButton("🎮  Game Profiles", 195);
            btnProfiles.Click += (s, e) => {
                HighlightButton(btnProfiles);
                SwitchUserControl(new UcGameProfiles());
            };

            btnSettings = CreateMenuButton("⚙️  Settings", 250);
            btnSettings.Click += (s, e) => {
                HighlightButton(btnSettings);
                SwitchUserControl(new UcSettings()); // Diaktifkan ke halaman UcSettings
            };

            pnlMenu.Controls.Add(btnDashboard);
            pnlMenu.Controls.Add(btnProfiles);
            pnlMenu.Controls.Add(btnSettings);

            // Footer Sidebar (Diatur agar selalu menempel di bawah secara responsif)
            lblFooter = new Label();
            lblFooter.Text = "ANDROID GAME\nSAVE MANAGER";
            lblFooter.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblFooter.ForeColor = Color.FromArgb(100, 116, 139);
            lblFooter.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblFooter.Location = new Point(20, pnlMenu.Height - 70);
            lblFooter.AutoSize = true;
            pnlMenu.Controls.Add(lblFooter);

            // Event resize panel menu agar footer otomatis mengikuti tinggi jendela
            pnlMenu.Resize += (s, e) => {
                lblFooter.Location = new Point(20, pnlMenu.Height - 70);
            };

            // Masukkan panel ke Form utama
            this.Controls.Add(pnlContainer);
            this.Controls.Add(pnlMenu);

            HighlightButton(btnDashboard);
        }

        private Button CreateMenuButton(string text, int topPos)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(12, topPos);
            btn.Size = new Size(196, 45);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = Color.FromArgb(226, 232, 240);
            btn.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(15, 0, 0, 0);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void HighlightButton(Button activeBtn)
        {
            btnDashboard.BackColor = Color.Transparent;
            btnProfiles.BackColor = Color.Transparent;
            btnSettings.BackColor = Color.Transparent;

            activeBtn.BackColor = Color.FromArgb(75, 46, 131);
        }

        private void SwitchUserControl(UserControl uc)
        {
            pnlContainer.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            pnlContainer.Controls.Add(uc);
        }
    }
}