using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blazor_arsip.Models
{
    public class UserPreferences
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        // Application Preferences
        public bool DarkMode { get; set; } = false;
        
        [MaxLength(10)]
        public string Language { get; set; } = "en";
        
        public int ItemsPerPage { get; set; } = 25;
        
        public bool AutoSave { get; set; } = true;
        
        public bool EmailNotifications { get; set; } = true;
        
        public bool BrowserNotifications { get; set; } = true;

        // File Management Preferences
        [MaxLength(100)]
        public string? DefaultUploadCategory { get; set; }
        
        public int MaxFileSize { get; set; } = 50;
        
        public bool AutoCategorize { get; set; } = true;
        
        public bool DeleteConfirmation { get; set; } = true;

        // Security Preferences
        public bool TwoFactorEnabled { get; set; } = false;
        
        public int SessionTimeout { get; set; } = 120;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
