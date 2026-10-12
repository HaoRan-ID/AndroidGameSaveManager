namespace AndroidGameSaveManager.Models
{
    public class AppSettings
    {
        public string DefaultBackupDir { get; set; } = @"C:\AndroidGameBackups";
        public string CustomAdbPath { get; set; } = "";
        public bool AutoConnect { get; set; } = true;
        public bool AutoSubfolder { get; set; } = true;
    }
}