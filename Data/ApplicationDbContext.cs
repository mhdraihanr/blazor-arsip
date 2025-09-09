using Microsoft.EntityFrameworkCore;
using blazor_arsip.Models;

namespace blazor_arsip.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<FileRecord> FileRecords { get; set; }
    public DbSet<FileVersion> FileVersions { get; set; }
    public DbSet<FileActivity> FileActivities { get; set; }
    public DbSet<FileCategory> FileCategories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure FileRecord
        modelBuilder.Entity<FileRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.OriginalFileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileExtension).IsRequired().HasMaxLength(100);
            entity.Property(e => e.MimeType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100).HasDefaultValue("General");
            entity.Property(e => e.UploadedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UploadedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.IsPublic).IsRequired().HasDefaultValue(false);
            
            entity.HasIndex(e => e.FileName);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.UploadedAt);
            entity.HasIndex(e => e.IsActive);
        });

        // Configure FileVersion
        modelBuilder.Entity<FileVersion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VersionFileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.VersionFilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(e => e.FileRecord)
                  .WithMany(e => e.Versions)
                  .HasForeignKey(e => e.FileRecordId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.FileRecordId);
            entity.HasIndex(e => e.VersionNumber);
        });

        // Configure FileActivity
        modelBuilder.Entity<FileActivity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ActivityType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PerformedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PerformedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            
            entity.HasOne(e => e.FileRecord)
                  .WithMany(e => e.Activities)
                  .HasForeignKey(e => e.FileRecordId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.FileRecordId);
            entity.HasIndex(e => e.ActivityType);
            entity.HasIndex(e => e.PerformedAt);
        });

        // Configure FileCategory
        modelBuilder.Entity<FileCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ColorCode).HasMaxLength(7).HasDefaultValue("#007bff");
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.LastLoginAt);
            
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure UserPreferences
        modelBuilder.Entity<UserPreferences>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Language).IsRequired().HasMaxLength(10).HasDefaultValue("en");
            entity.Property(e => e.DefaultUploadCategory).HasMaxLength(100);
            entity.Property(e => e.DarkMode).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.ItemsPerPage).IsRequired().HasDefaultValue(25);
            entity.Property(e => e.AutoSave).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.EmailNotifications).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.BrowserNotifications).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.MaxFileSize).IsRequired().HasDefaultValue(50);
            entity.Property(e => e.AutoCategorize).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.DeleteConfirmation).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.TwoFactorEnabled).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.SessionTimeout).IsRequired().HasDefaultValue(120);
            entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.UserId).IsUnique();
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default categories
        modelBuilder.Entity<FileCategory>().HasData(
            new FileCategory { Id = 1, Name = "Documents", Description = "General documents and text files", ColorCode = "#007bff", CreatedBy = "System" },
            new FileCategory { Id = 2, Name = "Images", Description = "Image files and graphics", ColorCode = "#28a745", CreatedBy = "System" },
            new FileCategory { Id = 3, Name = "Videos", Description = "Video files and multimedia", ColorCode = "#dc3545", CreatedBy = "System" },
            new FileCategory { Id = 4, Name = "Audio", Description = "Audio files and music", ColorCode = "#ffc107", CreatedBy = "System" },
            new FileCategory { Id = 5, Name = "Archives", Description = "Compressed files and archives", ColorCode = "#6f42c1", CreatedBy = "System" },
            new FileCategory { Id = 6, Name = "Spreadsheets", Description = "Excel and spreadsheet files", ColorCode = "#20c997", CreatedBy = "System" },
            new FileCategory { Id = 7, Name = "Presentations", Description = "PowerPoint and presentation files", ColorCode = "#fd7e14", CreatedBy = "System" },
            new FileCategory { Id = 8, Name = "Other", Description = "Other file types", ColorCode = "#6c757d", CreatedBy = "System" }
        );

        // Seed default users (password: "admin123" hashed with BCrypt)
        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                Id = 1, 
                Email = "admin@company.com", 
                Name = "Administrator", 
                PasswordHash = "$2a$11$8K1p/a0dRTlNqo/x3/Yd4.WdRuBdHdXRf5mGvFlvzeH4p5rEeIXJG", // admin123
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User 
            { 
                Id = 2, 
                Email = "user@company.com", 
                Name = "Regular User", 
                PasswordHash = "$2a$11$8K1p/a0dRTlNqo/x3/Yd4.WdRuBdHdXRf5mGvFlvzeH4p5rEeIXJG", // admin123
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User 
            { 
                Id = 3, 
                Email = "test@company.com", 
                Name = "Test User", 
                PasswordHash = "$2a$11$8K1p/a0dRTlNqo/x3/Yd4.WdRuBdHdXRf5mGvFlvzeH4p5rEeIXJG", // admin123
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
             }
         );
    }
}