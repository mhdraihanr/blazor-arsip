# Codex Rules — Blazor Arsip (C# Blazor Server)

Tujuan dokumen ini: memberi panduan kerja yang konsisten untuk asisten Codex saat memodifikasi project Blazor Arsip agar perubahan terarah, aman, dan sesuai arsitektur yang ada.

## Ruang Lingkup & Prinsip

- Fokus perubahan yang diminta user; hindari refactor luas tanpa alasan kuat.
- Ikuti pola & konvensi folder/penamaan yang sudah ada di repo.
- Manfaatkan Dependency Injection untuk service baru; jangan `new` langsung ketergantungan di komponen.
- Utamakan perbaikan akar masalah, bukan patch permukaan.
- Jangan menambah dependensi/alat baru tanpa kebutuhan jelas.

## Struktur & Konvensi Proyek

- Blazor Server (.NET 9) dengan auth cookie (`CustomAuth`).
- Razor Components berada di `Components/`:
  - `Components/Layout` untuk layout (`MainLayout`, `NavMenu`, `LoginLayout`).
  - `Components/Pages` untuk halaman. Pola umum halaman kompleks:
    - `Feature.razor` (UI)
    - `FeatureBase.cs` (code-behind/logic komponen)
    - `FeatureViewModel.cs` (state/VM)
- API berada di `Controllers/` (ASP.NET Core MVC Controllers, prefiks `api/`).
- Akses data via EF Core di `Data/` + `Models/` + `Migrations/`.
- Business logic via `Services/` dengan interface `I*Service` dan implementasi `*Service`.
- Static files di `wwwroot/` (upload file di `wwwroot/uploads/yyyy/MM/`).

## Aturan Implementasi Blazor

- Komponen halaman:
  - Letakkan di `Components/Pages/<Area>/` bila berkelompok (contoh: `FileManagement/*`).
  - Untuk halaman terlindungi, tambahkan `@attribute [Authorize]` dan pastikan navigasi via `NavMenu.razor` bila perlu.
  - Route utama dikelola oleh `Components/Routes.razor` dengan `AuthorizeRouteView`.
- Code-behind & VM:
  - Logika non-UI di `*Base.cs`; state di `*ViewModel.cs` agar UI tetap bersih.
  - Gunakan `async/await` untuk operasi I/O dan panggilan service.
- UI/UX:
  - Gunakan Bootstrap yang sudah tersedia (wwwroot/lib/bootstrap) dan ikon Font Awesome yang sudah dipakai.
  - Ikuti pola `MainLayout.razor` untuk interaksi sidebar/dark mode.

## Aturan Controllers/API

- Taruh endpoint di `Controllers/*Controller.cs` dengan `[ApiController]` + `[Route("api/[controller]")]`.
- Return tipe `IActionResult`/`ActionResult<T>`; validasi input, tangani error dengan `try/catch` + logging.
- Untuk endpoint yang memerlukan auth, gunakan `[Authorize]` atau validasi klaim sesuai kebutuhan.
- Jangan melakukan redirect otomatis untuk request API (sudah dikonfigurasi di `Program.cs`).

## Data, EF Core, dan Migrations

- Konfigurasi DB: MySQL/MariaDB (Pomelo). Connection string di `appsettings*.json` key `DefaultConnection`.
- Tambah/ubah skema hanya lewat EF Core migrations, bukan modifikasi manual file snapshot.
- Saat menyimpan file, ikuti strategi path tanggal (`yyyy/MM`) dan simpan metadata di `FileRecord`.
- Logging aktivitas file gunakan `FileActivity` melalui metode service yang ada (`LogActivityAsync`).

## Authentication & Authorization

- Sistem auth: ASP.NET Core Cookie (`CustomAuth`), login/logout via `AuthController`/`AccountController`.
- Akses halaman dilindungi oleh `AuthorizeRouteView` di `Components/Routes.razor` dan `[Authorize]` di komponen.
- Gunakan `ICurrentUserService` untuk data user aktif di komponen/service.

## Logging & Error Handling

- Log di service/controller menggunakan `ILogger<T>` dengan konteks jelas.
- Tangkap exception pada boundary (controller/service), tampilkan pesan ramah di UI via `ToastService` bila relevan.
- Jangan mengekspos detail internal exception ke client.

## Gaya Kode

- Ikuti gaya C# konvensional: PascalCase untuk public members, camelCase untuk lokal/privat.
- Hindari komentar inline berlebih; lebih baik beri nama metode/properti yang jelas.
- Jaga file tetap kecil dan kohesif; ekstrak metode bila perlu.

## Tugas Umum & Checklist

1) Menambah Halaman Baru
- Buat di `Components/Pages/<Area>/<Name>.razor` (+ `*Base.cs`, `*ViewModel.cs` bila kompleks).
- Tambah route `@page "/route"` dan `[Authorize]` bila perlu.
- Update `Components/Layout/NavMenu.razor` untuk navigasi jika perlu.
- Konsumsi service via DI; jangan akses DB langsung dari komponen.

2) Menambah Service Baru
- Tambah interface `I<Name>Service` dan implementasi `<Name>Service` di `Services/`.
- Registrasikan di DI container (`Program.cs` → `builder.Services.AddScoped<...>();`).
- Tulis unit-of-work kecil; satu service fokus pada satu area domain.

3) Menambah Endpoint API
- Tambah controller di `Controllers/` dengan rute `api/<name>`.
- Validasi input, gunakan service layer, tangani error, tambahkan logging.
- Pastikan endpoint tidak mengembalikan redirect untuk request API unauthorized (sudah di-handle di cookie events).

4) Upload/Download File
- Simpan fisik ke `wwwroot/uploads/yyyy/MM/` dan metadata ke DB via `IFileService.UploadFileAsync`.
- Gunakan `GetFilePathAsync` untuk resolving path; cek keberadaan file sebelum operasi.
- Saat hapus, soft delete di DB dan hapus fisik bila ada; tetap log aktivitas.

5) Build & Run Lokal
- Restore & build: `dotnet restore` → `dotnet build`.
- Jalankan dev: `dotnet watch run` (hot reload). Akses `http://localhost:5264`.
- Migrate DB: `dotnet ef database update` (pastikan tools terpasang).

## Do / Don’t untuk Codex

- Do: gunakan `apply_patch` untuk perubahan file; potong output baca file ≤ 250 baris per chunk.
- Do: jelaskan rencana kerja bila langkah multi-tahap; jaga perubahan minimal dan terfokus.
- Do: referensi file dengan path klik-able saat mendeskripsikan perubahan.
- Don’t: commit/branch kecuali diminta; jangan ubah file biner/hasil build di `bin/` dan `obj/`.
- Don’t: ubah `Migrations/*` secara manual; selalu pakai perintah migration.
- Don’t: tambahkan framework/dep baru tanpa persetujuan user.

## Catatan Khusus Proyek Ini

- Layout & navigasi: `Components/Layout/MainLayout.razor` dan `NavMenu.razor` mengatur sidebar + dark mode. Pertahankan event JS yang ada.
- Routing terpusat di `Components/Routes.razor` dengan `AuthorizeRouteView` dan `RedirectToLogin`.
- Service yang tersedia (contoh): `IFileService`, `IFileUploadService`, `IToastService`, `ICurrentUserService`, `IAuthenticationService`, `IIpAddressService`, `IUserSettingsService` — gunakan kembali sebelum menambah yang baru.
- Untuk styling komponen, gunakan CSS scoped di file `.razor.css` pada path yang sama.

## Contoh Template Komponen (Ringkas)

```razor
@page "/example"
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize]
@inject IToastService Toast

<h1>Example</h1>
<button class="btn btn-primary" @onclick="DoWork">Run</button>

@code {
    private async Task DoWork()
    {
        try
        {
            // call services here
            await Toast.ShowSuccessAsync("Done");
        }
        catch (Exception ex)
        {
            // log via injected logger if in Base class
            await Toast.ShowErrorAsync("Something went wrong");
        }
    }
}
```

---

Versi: 1.0 — Disusun untuk memandu Codex bekerja konsisten di repo ini.

