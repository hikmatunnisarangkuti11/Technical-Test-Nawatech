# Project1 - ASP.NET Core MVC with Identity

##📋 Deskripsi  
Proyek ini adalah aplikasi web berbasis ASP.NET Core MVC yang menggunakan ASP.NET Core Identity untuk fitur:
Registrasi
Login & Logout
Manajemen user
Password sudah otomatis di-hash
---

##✅ Prasyarat  
- .NET SDK 8.0 (atau versi sesuai proyek kamu)  
- SQL Server (localdb / SQL Server Express / SQL Server biasa)  
- IDE: Visual Studio 2022 / VS Code  

---

##📦 Package yang Digunakan  
- Microsoft.AspNetCore.Identity.EntityFrameworkCore  
- Microsoft.EntityFrameworkCore.SqlServer  
- Microsoft.EntityFrameworkCore.Tools

---

## Setup & Jalankan Proyek

1. **Clone repository**  
   git clone https://github.com/hikmatunnisarangkuti11/Technical-Test-Nawatech.git
   cd Technical-Test-Nawatech.
   
2. Ubah connection string
   Buka file appsettings.json, lalu sesuaikan bagian ini dengan koneksi database:
   Salin kode : "ConnectionStrings":
   { "DefaultConnection": "Server (localdb)\\mssqllocaldb;Database=Project1Db;
   Trusted_Connection=True;
   MultipleActiveResultSets=true"}
   
3. Buat dan jalankan migrasi database
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   
4. Jalankan aplikasi:
  dotnet run

5.Buka di browser:
  https://localhost:5001
