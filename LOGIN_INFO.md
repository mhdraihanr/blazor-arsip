# Informasi Login

## User yang tersedia di database:

1. **Admin**
   - Email: `admin@company.com`
   - Password: `admin123`
   
2. **Regular User**
   - Email: `user@company.com`
   - Password: `admin123`
   
3. **Test User**
   - Email: `test@company.com`
   - Password: `admin123`

## Cara Login:
1. Buka browser ke `http://localhost:5264`
2. Masukkan salah satu email dan password di atas
3. Klik tombol Login

## Troubleshooting:
- Jika stuck di "Sedang memproses...", periksa console browser (F12) untuk error
- Pastikan database MariaDB/MySQL berjalan
- Pastikan connection string di `appsettings.json` benar
