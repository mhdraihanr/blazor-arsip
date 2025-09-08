# Blazor Arsip - Sistem Manajemen File Digital

## 📋 Deskripsi Proyek

**Blazor Arsip** adalah aplikasi web berbasis Blazor Server untuk manajemen file digital perusahaan. Sistem ini menyediakan fitur upload, organisasi, pencarian, dan tracking file dengan antarmuka yang user-friendly.

### 🎯 Tujuan Utama

- Menyediakan repositori file digital yang terorganisir
- Memudahkan kolaborasi dan berbagi file dalam tim
- Melacak aktivitas dan perubahan file
- Menyediakan akses file yang aman dan terkontrol

## 📁 Struktur Direktori

```
blazer-arsip/
├── Components/           # Komponen Blazor
│   ├── Layout/          # Layout (MainLayout, NavMenu, LoginLayout)
│   ├── Pages/           # Halaman aplikasi
│   │   ├── Dashboard/   # Dashboard utama
│   │   ├── FileManagement/ # Manajemen file
│   │   ├── Login.razor  # Halaman login
│   │   ├── Register.razor # Halaman registrasi
│   │   └── Logout.razor # Halaman logout
│   └── Shared/          # Komponen shared
│       ├── UserMenu.razor # Menu user dengan auth
│       ├── AuthDebug.razor # Debug authentication
│       ├── RedirectToLogin.razor # Redirect handler
│       └── ToastContainer.razor # Toast notifications
├── Controllers/         # API & Auth Controllers
│   ├── AccountController.cs # Login/Logout controller
│   ├── AuthController.cs # Authentication API
│   └── FileController.cs # File management API
├── Data/                # Entity Framework
│   └── ApplicationDbContext.cs # DB Context dengan User
├── Migrations/          # Database migrations
├── Models/             # Data models
│   ├── User.cs         # User authentication model
│   ├── UserInfo.cs     # User display model
│   ├── LoginRequest.cs # Login form model
│   ├── RegisterRequest.cs # Registration form model
│   ├── FileRecord.cs   # File storage model
│   └── UploadModel.cs  # File upload model
├── Services/           # Business logic services
│   ├── AuthenticationService.cs # Auth & password hashing
│   ├── CurrentUserService.cs # Current user context
│   ├── FileService.cs  # File operations
│   ├── FileUploadService.cs # File upload logic
│   └── ToastService.cs # UI notifications
├── Program.cs          # Startup dengan auth configuration
├── Properties/         # Project properties
├── wwwroot/           # Static files
│   ├── lib/bootstrap/ # Bootstrap framework
│   └── uploads/       # File storage directory
└── blazor-arsip.csproj # Project configuration
```

## ⚙️ Instalasi dan Konfigurasi

### Prerequisites

- .NET 9.0 SDK
- MySQL/MariaDB database
- Git Bash (untuk environment Windows)

### Langkah Instalasi

1. **Clone dan setup project**

   ```bash
   cd d:\a-csharp\blazor-arsip
   dotnet restore
   ```

2. **Setup Database**

   - Gunakan docker-compose untuk database lokal:

   ```bash
   docker-compose up -d
   ```

   - Atau konfigurasi connection string di `appsettings.json`:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=BlazorArsipDb;User=user;Password=password;"
   }
   ```

3. **Jalankan migrations**

   ```bash
   dotnet ef database update
   ```

4. **Jalankan aplikasi**
   ```bash
   dotnet watch run
   ```

Aplikasi akan berjalan di `http://localhost:5264`

### 🔑 First Login

Setelah aplikasi berjalan:

1. **Akses aplikasi** di `http://localhost:5264`
2. **Akan diredirect ke login**: `http://localhost:5264/login`
3. **Login dengan test account**:
   - Email: `raihan@company.com`
   - Password: `admin123`
4. **Atau buat account baru** melalui link "Create Account"
5. **Setelah login berhasil** akan diredirect ke dashboard

## 🚀 Contoh Penggunaan

### Menjalankan Build dan Development

```bash
# Build project
cd d:\a-csharp\blazor-arsip
dotnet build

# Jalankan dengan hot reload
dotnet watch run

# Build untuk production
dotnet publish -c Release
```

### 📝 Menggunakan Aplikasi

**Setelah login berhasil, Anda dapat mengakses:**

- **Dashboard** (`/dashboard`) - Overview file dan statistik
- **Upload Files** (`/upload`) - Upload file baru
- **Browse Files** (`/list`) - Kelola file existing  
- **Search Files** (`/search`) - Pencarian file
- **Categories** (`/categories`) - Manajemen kategori

**User Management:**
- **User Menu** (klik nama user) - Profile dan logout
- **Registration** (`/register`) - Daftar user baru
- **Logout** - Keluar dengan aman dari aplikasi

### API Endpoints

- `GET /api/file/{id}` - Download file
- `GET /api/file/{id}/preview` - Preview file
- `GET /api/file/stats` - File statistics

## 📦 Dependensi Utama

### .NET Packages

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="9.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Components.Web" Version="9.0.0" />
<PackageReference Include="System.IO.Abstractions" Version="21.1.3" />
<PackageReference Include="FluentValidation" Version="11.11.0" />
```

### Frontend Libraries

- Bootstrap 5.3.2 (wwwroot/lib/bootstrap/)
- Font Awesome Icons
- Blazor Interactive Server Components

### Database

- MySQL/MariaDB dengan Entity Framework Core
- Database schema managed via migrations

## 🏆 Best Practices untuk AI Assistance

### Struktur Kode yang Konsisten

- Gunakan dependency injection untuk semua services
- Implementasi pattern ViewModel untuk komponen Blazor
- Gunakan async/await untuk operasi I/O

### Optimalisasi Performa

```csharp
// Gunakan pagination untuk data besar
var files = await _context.FileRecords
    .Where(f => f.IsActive)
    .OrderByDescending(f => f.UploadedAt)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

// Cache data yang sering diakses
services.AddMemoryCache();
```

### Error Handling

```csharp
try
{
    // Business logic
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error processing file upload");
    await _toastService.ShowErrorAsync("Upload failed");
}
```

### Logging dan Monitoring

- Serilog untuk structured logging
- Activity tracking untuk file operations
- Performance monitoring dengan EF Core logging

## 🔧 Development Workflow

1. **Setup Environment**

   - Pastikan .NET 9.0 terinstall
   - Database running (local atau docker)
   - Restore dependencies

2. **Development**

   - Gunakan `dotnet watch run` untuk hot reload
   - Test di browser `http://localhost:5264`
   - Monitor terminal untuk error dan logs

3. **Debugging**

   - Check terminal output untuk error messages
   - Gunakan browser developer tools
   - Monitor EF Core queries di logs

4. **Deployment**
   - Build dengan `dotnet publish -c Release`
   - Deploy ke IIS atau container
   - Migrate database untuk production

## 🚨 Troubleshooting

### Common Issues

**Build Error: File in use**

```bash
# Hentikan proses yang sedang running
dotnet watch run --shutdown
# atau gunakan task manager untuk kill process
```

**Database Connection Issues**

- Pastikan database service running
- Check connection string di appsettings.json
- Verify database credentials

**Bootstrap Components Not Working**

- Pastikan Bootstrap JS loaded di App.razor
- Verify wwwroot/lib/bootstrap/ exists

**Authentication Issues**

- **Login Loop**: Clear browser cookies dan restart aplikasi
- **403 Unauthorized**: Pastikan user login dengan akun valid
- **Redirect Issues**: Check browser developer tools untuk error di console
- **Session Expired**: Login ulang jika session sudah expire

**Database User Issues**

- **No Test User**: Jalankan migration untuk create seed data
- **Password Reset**: Check database tabel `Users` untuk verify hash
- **Email Duplicate**: Gunakan email yang belum terdaftar untuk registrasi

## 🔐 Sistem Autentikasi

### Overview

Sistem autentikasi lengkap berbasis **ASP.NET Core Cookie Authentication** dengan dukungan penuh untuk:
- Login/Logout dengan email dan password
- User registration
- Session management
- Authorization pada halaman protected
- Redirect otomatis untuk user yang belum login

### 🚀 Fitur Authentication

#### ✅ Login System
- **Email & Password Authentication**
- **Cookie-based session** dengan expiry 30 hari
- **Automatic redirect** ke dashboard setelah login
- **Error handling** untuk invalid credentials
- **Return URL support** untuk redirect ke halaman yang diminta

#### ✅ User Registration
- **Self-registration** dengan validasi email unik
- **Password hashing** menggunakan BCrypt
- **Form validation** dengan data annotations
- **Auto-redirect** ke login setelah registrasi berhasil

#### ✅ Logout System
- **Complete session cleanup**
- **Cookie removal** dan redirect ke login
- **User menu integration** dengan form POST

#### ✅ Authorization
- **Page-level protection** dengan `[Authorize]` attribute
- **Automatic redirects** untuk unauthenticated users
- **Route-based authorization** dengan `AuthorizeRouteView`

### 🏗️ Arsitektur Authentication

#### Database Schema

**User Model** (`Models/User.cs`):
```csharp
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
```

#### Controllers

**AccountController** (`Controllers/AccountController.cs`):
- `POST /Account/Login` - Handles browser login form
- `POST /Account/Logout` - Clears authentication and redirects
- Cookie management dengan proper security settings

#### Services

**AuthenticationService** (`Services/AuthenticationService.cs`):
```csharp
public interface IAuthenticationService
{
    Task<User?> AuthenticateAsync(string email, string password);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
```

**CurrentUserService** (`Services/CurrentUserService.cs`):
```csharp
public interface ICurrentUserService
{
    Task<UserInfo?> GetCurrentUserAsync();
}
```

### 🔧 Konfigurasi

**Program.cs Configuration**:
```csharp
// Authentication & Authorization
builder.Services.AddAuthentication("CustomAuth")
    .AddCookie("CustomAuth", options =>
    {
        options.Cookie.Name = "BlazorArsipAuth";
        options.Cookie.HttpOnly = true;
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Authentication Services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
```

### 🛡️ Protected Pages

Semua halaman penting dilindungi dengan `[Authorize]`:
- `/dashboard` - Dashboard utama
- `/upload` - File upload
- `/list` - File management
- `/categories` - Category management
- `/search` - File search

**Contoh implementasi**:
```razor
@page "/dashboard"
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize]

<!-- Dashboard content -->
```

### 🔑 Test User Account

Untuk testing, tersedia user account:
- **Email**: `raihan@company.com`
- **Password**: `admin123`
- **Name**: Muhammad Raihan

### 📱 UI Components

#### Login Page (`/login`)
- Form dengan HTML standard (bukan EditForm)
- Browser-native form POST ke `/Account/Login`
- Error handling dari query parameters
- Link ke registration page

#### User Menu Component
- Menampilkan nama user dan email
- Avatar support (initials atau photo)
- Dropdown dengan profile options
- Logout button dengan form POST

#### Registration Page (`/register`)
- Email uniqueness validation
- Password confirmation
- Form validation dengan DataAnnotations
- Success/error messaging

### 🚦 Authorization Flow

1. **Unauthenticated user** mengakses protected page
2. **AuthorizeRouteView** mendeteksi unauthorized access
3. **RedirectToLogin** component mengalihkan ke `/login?returnUrl=...`
4. Setelah **login berhasil**, user diredirect ke halaman asli
5. **Cookie authentication** menjaga session aktif

### 🔍 Debugging Authentication

Gunakan **AuthDebug** component untuk monitoring:
```razor
<AuthDebug />
```

Menampilkan:
- Authentication status
- User name dan email
- Claims information
- Real-time updates saat auth state berubah

## 📞 Support

Untuk bantuan teknis atau issues:

1. Check logs di terminal output
2. Verify database connection
3. Confirm .NET 9.0 installation
4. Check file permissions untuk uploads directory
5. Verify authentication configuration di Program.cs

---

**Version**: 2.0.0 (with Authentication)  
**Last Updated**: January 2025  
**Framework**: .NET 9.0 Blazor Server  
**Database**: MySQL/MariaDB
**Authentication**: ASP.NET Core Cookie Authentication
**Branch**: `with-auth` (main feature branch)
