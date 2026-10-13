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
        // Fungsi Backup dengan fitur Auto-Fallback (Mendukung Game Normal & Game Terproteksi seperti Ren'Py)
        public string PullSaveData(string androidPath, string pcDestinationPath)
        {
            try
            {
                // 1. Coba cara standar terlebih dahulu
                string arguments = $"pull \"{androidPath}\" \"{pcDestinationPath}\"";
                string result = RunAdbCommand(arguments);

                // 2. Cek apakah backup standar gagal / diblokir Scoped Storage (Ren'Py)
                bool isFailedOrProtected = string.IsNullOrEmpty(result) ||
                                           result.Contains("0 files pulled") ||
                                           result.Contains("Permission denied") ||
                                           result.Contains("does not exist");

                if (isFailedOrProtected)
                {
                    // --- METODE BYPASS OTOMATIS KHUSUS REN'PY / SCOPED STORAGE ---
                    string tempAndroidFolder = "/sdcard/Download/AGSM_TempSave";

                    // Hapus folder sementara lama jika ada
                    RunAdbCommand($"shell rm -rf \"{tempAndroidFolder}\"");

                    // Salin dari folder terproteksi ke folder Download publik via shell
                    RunAdbCommand($"shell cp -r \"{androidPath}\" \"{tempAndroidFolder}\"");

                    // Pull dari folder Download publik ke PC
                    result = RunAdbCommand($"pull \"{tempAndroidFolder}\" \"{pcDestinationPath}\"");

                    // Bersihkan folder sementara di HP
                    RunAdbCommand($"shell rm -rf \"{tempAndroidFolder}\"");
                }

                return result;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // Fungsi untuk mengirim folder save dari PC kembali ke HP (Restore)
        // Fungsi untuk mengirim folder save dari PC kembali ke HP (Restore) secara bersih
        public string PushSaveData(string pcRootOrGamePath, string androidDestinationPath)
        {
            try
            {
                // 1. Ambil nama folder game dari path Android (Contoh: "Haoran" dari "/sdcard/Download/Haoran")
                string cleanAndroidPath = androidDestinationPath.TrimEnd('/');
                string gameFolderName = Path.GetFileName(cleanAndroidPath);

                // 2. Deteksi path PC: Jika user memilih folder utama (C:\AGMS), 
                // otomatis arahkan ke subfolder game-nya (C:\AGMS\Haoran)
                string actualPcSource = pcRootOrGamePath;
                if (!actualPcSource.EndsWith(gameFolderName, StringComparison.OrdinalIgnoreCase))
                {
                    string combinedPath = Path.Combine(pcRootOrGamePath, gameFolderName);
                    if (Directory.Exists(combinedPath))
                    {
                        actualPcSource = combinedPath;
                    }
                }

                // 3. Ambil direktori induk (parent) dari path Android (Contoh: "/sdcard/Download")
                int lastSlashIndex = cleanAndroidPath.LastIndexOf('/');
                if (lastSlashIndex <= 0)
                {
                    return "Error: Path Android tujuan tidak valid!";
                }
                string androidParentDir = cleanAndroidPath.Substring(0, lastSlashIndex);

                // 4. Jalankan adb push dari folder game PC langsung ke parent direktori Android
                // Perintah: adb push "C:\AGMS\Haoran" "/sdcard/Download"
                // Hasil di HP: /sdcard/Download/Haoran (Bersih tanpa folder AGMS!)
                string arguments = $"push \"{actualPcSource}\" \"{androidParentDir}\"";
                return RunAdbCommand(arguments);
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
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
        // =========================================================
        // FITUR TAMBAHAN: BACKUP KHUSUS REN'PY / SCOPED STORAGE (ANDROID 11+)
        // =========================================================
        public string PullProtectedSaveData(string androidPath, string pcDestinationPath)
        {
            try
            {
                string tempAndroidFolder = "/sdcard/Download/AGSM_TempSave";

                // 1. Salin folder terkunci ke folder Download publik
                RunAdbCommand($"shell cp -r \"{androidPath}\" \"{tempAndroidFolder}\"");

                // 2. Tarik data dari folder Download publik ke PC
                string result = PullSaveData(tempAndroidFolder, pcDestinationPath);

                // 3. Bersihkan folder sementara di HP
                RunAdbCommand($"shell rm -rf \"{tempAndroidFolder}\"");

                return result;
            }
            catch (Exception ex)
            {
                return "Error Bypass: " + ex.Message;
            }
        }
    }
}