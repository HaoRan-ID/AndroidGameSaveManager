# 🎮 Android Game Save Manager (AGSM)

<p align="center">
  <img src="AndroidGameSaveManager/Icon/AGMS.ico" alt="AGSM Logo" width="100"/>
</p>

<p align="center">
  <b>Solusi Praktis, Otomatis, dan Aman untuk Backup & Restore Save Data Game Android ke PC Windows via ADB.</b>
</p>

<p align="center">
  <i>"Dirancang utama untuk mengelola file save game Android, aplikasi ini juga fleksibel untuk mem-backup maupun merestore berbagai folder data dan dokumen penting di penyimpanan internal perangkat Android Anda."</i>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows-blue.svg" alt="Platform">
  <img src="https://img.shields.io/badge/Framework-.NET%2010.0%20WinForms-purple.svg" alt=".NET 10">
  <img src="https://img.shields.io/badge/Architecture-x64-brightgreen.svg" alt="x64">
  <img src="https://img.shields.io/badge/ADB-Bundled-orange.svg" alt="ADB Bundled">
</p>

---

## 🌟 Fitur Utama

- 🔄 **Backup & Restore 1-Klik**: Transfer data *save game* antara Android dan PC secara presisi tanpa merusak struktur folder.
- 📁 **Dukungan Penyimpanan Umum (General Storage)**: Selain khusus untuk data game, aplikasi ini dapat dimanfaatkan sebagai utility manager untuk **mem-backup atau memulihkan folder data umum** di penyimpanan internal Android Anda ke PC.
- 🛡️ **Bypass Scoped Storage (Android 11+)**: Dilengkapi **Auto-Fallback Engine** cerdas. Mengatasi pembatasan akses direktori `Android/data` pada Android versi baru (seperti game bermesin **Ren'Py**).
- 🔍 **Auto Package Scanner**: Mendeteksi daftar aplikasi/game terinstal di HP secara langsung tanpa perlu mengetik *package name* manual.
- 📁 **Smart Path Management**: Otomatis membuat subfolder berdasarkan nama game/profil di direktori default (`C:\AGSManager`).
- ⚡ **Portable & Bundled ADB**: Tidak perlu melakukan instalasi platform-tools ADB manual di PC, semua library pendukung sudah terintegrasi secara otomatis.
- 💼 **Profile Management**: Import dan Export konfigurasi profil game/folder favorit menggunakan format JSON (`profiles.json`).

---

## 🛠️ Persyaratan Sistem

- **OS PC**: Windows 10 / Windows 11 (64-bit)
- **Perangkat Android**: Android 5.0 hingga Android 14+
- **Koneksi**: Kabel USB dengan mode **USB Debugging** yang sudah diaktifkan di menu *Developer Options* perangkat Android Anda.

---

## 🚀 Cara Penggunaan

### 1. Jalankan Aplikasi
Aplikasi dapat diinstal menggunakan **Setup.exe** atau dijalankan secara portable dari folder release.

### 2. Hubungkan HP ke PC
1. Aktifkan **USB Debugging** di Smartphone Anda.
2. Hubungkan HP ke PC menggunakan kabel USB.
3. Klik tombol **Connect / Refresh** pada Dashboard. Pastikan muncul dialog izin pada layar HP Anda (*Allow USB Debugging*) lalu centang *Always allow*.

### 3. Backup Save Data / Folder Data
1. Pilih game dari daftar **Game Profiles** atau scan otomatis lewat **Scan Game**.
2. Tentukan folder penyimpanan di PC (Default: `C:\AGSManager\<NamaGame>`).
3. Klik **Backup** dan tunggu hingga status selesai.

### 4. Restore Save Data / Folder Data
1. Pilih profil game/folder yang ingin dipulihkan.
2. Klik tombol **Restore**.
3. Data akan langsung ditransfer kembali ke direktori Android yang bersangkutan.

---

## 👨‍💻 Pengembang

Proyek ini dirancang dan dikembangkan oleh:

* **Nama**: Hao Ran.
* **GitHub**: [HaoRan-ID](https://github.com/HaoRan-ID)
* **Portfolio**: [Kunjungi Portfolio](https://linktr.ee/HaoRan.ID)

---

## 📜 Lisensi

Proyek ini dirilis di bawah lisensi **MIT License** - Anda bebas mengunduh, menggunakan, dan memodifikasi aplikasi ini sesuai kebutuhan.
