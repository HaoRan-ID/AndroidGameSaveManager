namespace AndroidGameSaveManager.Models
{
    public class GameProfile
    {
        public string GameName { get; set; }
        public string AndroidPath { get; set; }
        public string PcPath { get; set; }

        // Menampilkan nama game di dalam ListBox nanti
        public override string ToString()
        {
            return GameName;
        }
    }
}