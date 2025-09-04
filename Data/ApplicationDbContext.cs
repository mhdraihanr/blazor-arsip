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
            entity.Property(e => e.DownloadCount).HasDefaultValue(0);
            
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
    }
}