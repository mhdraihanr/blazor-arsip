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
blazor-arsip/
├── Components/           # Komponen Blazor
│   ├── Layout/          # Layout utama (MainLayout, NavMenu)
│   ├── Pages/           # Halaman aplikasi
│   │   ├── Dashboard/   # Dashboard utama
│   │   └── FileManagement/ # Manajemen file
│   └── Shared/          # Komponen shared (Toast, UserMenu)
├── Controllers/         # API Controllers
│   └── FileController.cs
├── Data/                # Entity Framework
│   └── ApplicationDbContext.cs
├── Migrations/          # Database migrations
├── Models/             # Data models
│   ├── FileRecord.cs
│   ├── UploadModel.cs
│   └── UserInfo.cs
├── Services/           # Business logic services
│   ├── CurrentUserService.cs
│   ├── FileService.cs
│   ├── FileUploadService.cs
│   └── ToastService.cs
├── Program.cs          # Startup configuration
├── Properties/         # Project properties
├── wwwroot/           # Static files
│   ├── lib/bootstrap/ # Bootstrap framework
│   └── uploads/       # File storage
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

### Mengakses File Upload

- Upload file melalui `/upload`
- Kelola file di `/files`
- Dashboard overview di `/dashboard`

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

## 🔐 Sistem Autentikasi

### Struktur Autentikasi

Sistem menggunakan autentikasi email dan password dengan integrasi Blazor Server authentication:

**Model User** (`Models/UserInfo.cs`):

```csharp
public class UserInfo
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}
```

**Current User Service** (`Services/CurrentUserService.cs`):

```csharp
public interface ICurrentUserService
{
    Task<UserInfo?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}

public class CurrentUserService : ICurrentUserService
{
    public Task<UserInfo?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        // Implementasi autentikasi email dan password
        var user = new UserInfo
        {
            Email = "user@company.com",
            Name = "Authenticated User",
            PhotoUrl = null
        };
        return Task.FromResult<UserInfo?>(user);
    }
}
```

### Fitur Login

- **Email & Password Authentication**: Login menggunakan kombinasi email dan password
- **Session Management**: Management session user yang aman
- **User Context**: Akses informasi user melalui dependency injection
- **Form Handling**: Menggunakan `FormName` parameter pada `<EditForm>` untuk mengatasi masalah POST request

#### Perbaikan Form POST Issue

Untuk mengatasi error "The POST request does not specify which form is being submitted", pastikan:

1. **Tambahkan FormName pada EditForm**:
   ```razor
   <EditForm Model="FormModel" OnValidSubmit="HandleLogin" FormName="LoginForm">
   ```

2. **Verifikasi form submission**: FormName parameter memastikan Blazor dapat mengidentifikasi form yang di-submit

3. **Data validation**: Form menggunakan DataAnnotationsValidator untuk validasi client-side

### Konfigurasi Program.cs

```csharp
// Registrasi services autentikasi
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Catatan: Sistem saat ini menggunakan mock user untuk demo.
// Untuk implementasi autentikasi lengkap, tambahkan:
// services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie(options =>
//     {
//         options.LoginPath = "/login";
//         options.AccessDeniedPath = "/access-denied";
//     });
```

### Status Autentikasi Saat Ini

- **Mode Demo**: Menggunakan mock user data untuk testing
- **CurrentUserService**: Menyediakan informasi user saat ini
- **Planned Features**: Login email/password akan diimplementasikan pada fase berikutnya

## 📞 Support

Untuk bantuan teknis atau issues:

1. Check logs di terminal output
2. Verify database connection
3. Confirm .NET 9.0 installation
4. Check file permissions untuk uploads directory
5. Verify authentication configuration di Program.cs

---

**Version**: 1.0.0  
**Last Updated**: 2024  
**Framework**: .NET 9.0 Blazor Server  
**Database**: MySQL/MariaDB
