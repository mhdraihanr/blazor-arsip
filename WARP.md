# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

Blazor Arsip is a .NET 9.0 Blazor Server application for digital file management with features for upload, organization, search, and tracking.

## Core Development Commands

### Build and Run
```bash
# Navigate to project directory
cd D:\a-csharp\blazor-arsip

# Restore dependencies
dotnet restore

# Run with hot reload
dotnet watch run

# Build for production
dotnet publish -c Release -o ./publish
```

### Database Management
```bash
# Start database services (MariaDB + phpMyAdmin)
docker-compose up -d

# Apply migrations
dotnet ef database update

# Create new migration
dotnet ef migrations add <MigrationName>

# Remove last migration
dotnet ef migrations remove
```

### Testing and Debugging
```bash
# Run unit tests (when available)
dotnet test

# Check build errors
dotnet build

# Clean build artifacts
dotnet clean
```

### Common Port Information
- Application: http://localhost:5264
- MariaDB: localhost:3307
- phpMyAdmin: http://localhost:8080

## Architecture Overview

### Technology Stack
- **Framework**: .NET 9.0 Blazor Server
- **Database**: MariaDB/MySQL via Entity Framework Core (Pomelo provider)
- **Frontend**: Bootstrap 5.3.2, Font Awesome icons
- **Authentication**: Custom cookie-based authentication

### Key Service Architecture
```
Program.cs
├── Authentication Services
│   ├── IAuthenticationService → AuthenticationService
│   ├── CustomAuthenticationStateProvider
│   └── ICurrentUserService → CurrentUserService
├── Business Services
│   ├── IFileService → FileService (file CRUD operations)
│   ├── IFileUploadService → FileUploadService (upload handling)
│   └── IToastService → ToastService (notifications)
└── Data Context
    └── ApplicationDbContext (EF Core)
```

### Database Schema
- **Users**: Authentication and user profiles
- **FileRecords**: Main file metadata storage
- **FileVersions**: File version tracking
- **FileActivities**: Activity logging
- **FileCategories**: File categorization (seeded with defaults)

### Component Structure
```
Components/
├── Layout/
│   ├── MainLayout.razor (authenticated layout)
│   └── NavMenu.razor (navigation menu)
├── Pages/
│   ├── Dashboard/ (dashboard views)
│   ├── FileManagement/ (file operations)
│   ├── Login.razor (authentication)
│   └── Register.razor (user registration)
└── Shared/
    ├── UserMenu.razor (user profile dropdown)
    ├── Toast.razor (notification component)
    └── ConfirmationDialog.razor
```

## Critical Patterns

### Dependency Injection Pattern
All services are registered in Program.cs and injected via constructor or `@inject`:
```csharp
builder.Services.AddScoped<IFileService, FileService>();
```

### Authentication Flow
1. Login via `/api/auth/login` endpoint
2. Cookie-based session with CustomAuth scheme
3. Claims include: NameIdentifier, Name, Email, PhotoUrl
4. Protected pages use `[Authorize]` attribute

### File Upload Pattern
- Max file size: 100MB (configurable in appsettings.json)
- Storage path: `wwwroot/uploads/`
- Supported extensions: .pdf, .doc, .docx, .xls, .xlsx, .ppt, .pptx, .txt, .jpg, .jpeg, .png, .gif, .zip, .rar, .7z

### Form Handling (Blazor Server)
All forms must include `FormName` parameter to avoid POST errors:
```razor
<EditForm Model="@FormModel" OnValidSubmit="@HandleSubmit" FormName="UniqueFormName">
```

## Development Workflow

### Starting Fresh Development
1. Ensure Docker Desktop is running
2. Start database: `docker-compose up -d`
3. Update database: `dotnet ef database update`
4. Run app: `dotnet watch run`
5. Login with test credentials (see LOGIN_INFO.md)

### Making Code Changes
1. Use `dotnet watch run` for hot reload
2. Changes to .razor files reflect immediately
3. Changes to .cs files trigger automatic rebuild

### Adding New Features
1. Create models in `Models/` directory
2. Update `ApplicationDbContext` if database changes needed
3. Create migration: `dotnet ef migrations add <Name>`
4. Implement service interface in `Services/`
5. Register service in `Program.cs`
6. Create Razor components in appropriate `Components/` subdirectory

## Common Issues and Solutions

### "File in use" Build Error
```bash
# Stop the watch process
Ctrl+C in terminal
# Or force shutdown
dotnet watch run --shutdown
```

### Database Connection Issues
- Verify Docker containers running: `docker ps`
- Check connection string port (3307 not 3306)
- Ensure MariaDB container is healthy

### Authentication Not Working
- Check cookie settings in Program.cs
- Verify `FormName` on login form
- Ensure authentication middleware order is correct

### File Upload Failures
- Check `wwwroot/uploads/` directory exists and has write permissions
- Verify file size limits in appsettings.json
- Ensure proper multipart form configuration

## Key Configuration Files

### appsettings.json
- Database connection string (Port 3307 for Docker MariaDB)
- File upload settings (max size, allowed extensions)
- Logging configuration

### docker-compose.yml
- MariaDB on port 3307
- phpMyAdmin on port 8080
- Default credentials in environment variables

### launchSettings.json
- HTTP: http://localhost:5264
- HTTPS: https://localhost:7292

## Performance Considerations

### Entity Framework Queries
- Use `.AsNoTracking()` for read-only queries
- Implement pagination for large datasets
- Use `.Include()` carefully to avoid N+1 queries

### Blazor Server Optimization
- Minimize StateHasChanged() calls
- Use virtualization for large lists
- Implement proper disposal in components

### File Handling
- Stream large files instead of loading into memory
- Implement chunked uploads for very large files
- Clean up temporary files after processing

## Security Notes

### Authentication State
- All pages except Login/Register require authentication
- User context available via ICurrentUserService
- Claims-based authorization ready for role implementation

### File Security
- Files stored with generated names to prevent path traversal
- Original filenames preserved in database
- Activity logging tracks all file operations

### Database Security
- Password hashing uses BCrypt
- Connection strings should use environment variables in production
- Prepared statements prevent SQL injection via EF Core
