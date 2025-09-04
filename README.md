# Blazor Arsip

Aplikasi manajemen arsip dokumen menggunakan Blazor dengan database MariaDB.

## Prasyarat

- .NET 9.0 SDK
- Docker dan Docker Compose

## Cara Menjalankan Aplikasi

### 1. Jalankan MariaDB dengan Docker

```bash
docker-compose up -d
```

Perintah ini akan menjalankan container MariaDB di port 3307 dengan kredensial berikut:
- Database: BlazorArsipDb
- Username: root
- Password: password

### 2. Jalankan Migrasi Database

```bash
dotnet ef database update
```

Jika belum ada migrasi, buat migrasi terlebih dahulu:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Jalankan Aplikasi

```bash
dotnet run
```

Aplikasi akan berjalan di http://localhost:5264

## Konfigurasi

Konfigurasi koneksi database dapat diubah di file `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3307;Database=BlazorArsipDb;User=root;Password=password;TreatTinyAsBoolean=true;"
}
```