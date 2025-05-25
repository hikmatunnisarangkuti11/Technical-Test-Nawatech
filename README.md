# Project1 - ASP.NET Core MVC with Identity

## Deskripsi  
Proyek ini adalah aplikasi web ASP.NET Core MVC yang menggunakan ASP.NET Core Identity 
untuk fitur registrasi, login, logout, dan manajemen user. Password otomatis di-hash 
dan terdapat fitur email confirmation (bisa diaktifkan/dinonaktifkan).

---

## Prasyarat  
- .NET SDK 8.0 (atau versi sesuai proyek kamu)  
- SQL Server (localdb / SQL Server Express / SQL Server biasa)  
- IDE: Visual Studio 2022 / VS Code / Rider  

---

## Package yang Digunakan  
- Microsoft.AspNetCore.Identity.EntityFrameworkCore  
- Microsoft.EntityFrameworkCore.SqlServer  
- Microsoft.EntityFrameworkCore.Tools

---

## Setup & Jalankan Proyek

1. **Clone repository**  
   ```bash
   git clone [https://github.com/username/Project1.git](https://github.com/hikmatunnisarangkuti11/Technical-Test-Nawatech.git)
   cd Project1
   
2. Buka appsettings.json, sesuaikan connection string database:
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Project1Db;Trusted_Connection=True;MultipleActiveResultSets=true
  }
  
3.  Buat dan jalankan migration database
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  Jika belum install EF Core CLI:

4. dotnet run
  Atau pakai Visual Studio tekan F5 atau Ctrl + F5.

5. Buka aplikasi di browser
Akses: https://localhost:5001
