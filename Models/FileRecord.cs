using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blazor_arsip.Models;

public class FileRecord
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FileExtension { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string MimeType { get; set; } = string.Empty;

    [Required]
    public long FileSize { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? Tags { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = "Documents";

    [Required]
    [StringLength(100)]
    public string UploadedBy { get; set; } = string.Empty;

    [Required]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedAt { get; set; }

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public bool IsPublic { get; set; } = false;

    [StringLength(32)]
    public string? FileHash { get; set; }

    public DateTime? LastAccessedAt { get; set; }

    // Navigation properties
    public virtual ICollection<FileVersion> Versions { get; set; } = new List<FileVersion>();
    public virtual ICollection<FileActivity> Activities { get; set; } = new List<FileActivity>();
}

public class FileVersion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FileRecordId { get; set; }

    [Required]
    [StringLength(255)]
    public string VersionFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string VersionFilePath { get; set; } = string.Empty;

    [Required]
    public int VersionNumber { get; set; }

    [Required]
    public long FileSize { get; set; }

    [StringLength(500)]
    public string? ChangeDescription { get; set; }

    [Required]
    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(32)]
    public string? FileHash { get; set; }

    // Navigation property
    [ForeignKey("FileRecordId")]
    public virtual FileRecord FileRecord { get; set; } = null!;
}

public class FileActivity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FileRecordId { get; set; }

    [Required]
    [StringLength(50)]
    public string ActivityType { get; set; } = string.Empty; // Upload, Download, Update, Delete, View

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string PerformedBy { get; set; } = string.Empty;

    [Required]
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

    [StringLength(45)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    // Navigation property
    [ForeignKey("FileRecordId")]
    public virtual FileRecord FileRecord { get; set; } = null!;
}

public class FileCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(7)]
    public string? ColorCode { get; set; } = "#007bff";

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? CreatedBy { get; set; }
}