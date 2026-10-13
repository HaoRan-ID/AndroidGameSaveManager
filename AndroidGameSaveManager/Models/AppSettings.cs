namespace AndroidGameSaveManager.Models
{
    public class AppSettings
    {
        public string DefaultBackupDir { get; set; } = @"C:\AGSManager"; // <-- Ubah ke C:\AGSManager
        public string CustomAdbPath { get; set; } = "";
        public bool AutoConnect { get; set; } = true;
        public bool AutoSubfolder { get; set; } = true;
    }
}