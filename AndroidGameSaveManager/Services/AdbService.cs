using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace AndroidGameSaveManager.Services
{
    public class AdbService
    {
        // Mengarahkan path secara otomatis ke dalam folder platform-tools di direktori aplikasi
        private string adbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "platform-tools", "adb.exe");

        public string RunAdbCommand(string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = adbPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        process.WaitForExit();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public bool IsDeviceConnected()
        {
            string output = RunAdbCommand("devices");
            string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Length > 1;
        }

        // ==========================================
        // FITUR BARU: BACKUP & RESTORE SAVE GAME
        // ==========================================

        // Fungsi untuk mengambil folder save dari HP ke PC (Backup)
        public string PullSaveData(string androidPath, string pcDestinationPath)
        {
            // Perintah ADB: adb pull "path_di_hp" "path_di_pc"
            string arguments = $"pull \"{androidPath}\" \"{pcDestinationPath}\"";
            return RunAdbCommand(arguments);
        }

        // Fungsi untuk mengirim folder save dari PC kembali ke HP (Restore)
        public string PushSaveData(string pcSourcePath, string androidDestinationPath)
        {
            // Perintah ADB: adb push "path_di_pc" "path_di_hp"
            string arguments = $"push \"{pcSourcePath}\" \"{androidDestinationPath}\"";
            return RunAdbCommand(arguments);
        }

        public List<string> GetInstalledPackages()
        {
            var packages = new List<string>();
            try
            {
                // Menggunakan parameter '-3' agar hanya menampilkan aplikasi/game pihak ketiga (non-sistem)
                string output = RunAdbCommand("shell pm list packages -3");
                using (StringReader reader = new StringReader(output))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("package:"))
                        {
                            packages.Add(line.Substring("package:".Length).Trim());
                        }
                    }
                }
            }
            catch { }
            packages.Sort();
            return packages;
        }
    }
}