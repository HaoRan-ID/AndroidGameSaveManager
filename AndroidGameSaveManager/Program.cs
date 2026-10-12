namespace AndroidGameSaveManager
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            // Memanggil MainForm buatan kita
            Application.Run(new MainForm());
        }
    }
}