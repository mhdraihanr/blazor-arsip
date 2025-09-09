using System.ComponentModel.DataAnnotations;

namespace blazor_arsip.Components.Pages.Settings
{
    public class SettingsViewModel
    {
        // User Profile Settings
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;


        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }

        // Note: Application preferences (DarkMode, Language, etc.) have been removed
        // These settings are now handled globally or locally as needed

        // Security Settings
        [Display(Name = "Two-Factor Authentication")]
        public bool TwoFactorEnabled { get; set; } = false;

        [Display(Name = "Session Timeout (minutes)")]
        [Range(15, 480)]
        public int SessionTimeout { get; set; } = 120;

        // File Management Settings
        [Display(Name = "Default Upload Category")]
        public string? DefaultUploadCategory { get; set; }

        [Display(Name = "Auto-categorize Files")]
        public bool AutoCategorize { get; set; } = true;

        [Display(Name = "Delete Confirmation")]
        public bool DeleteConfirmation { get; set; } = true;

        [Display(Name = "Maximum File Size (MB)")]
        [Range(1, 500)]
        public int MaxFileSize { get; set; } = 50;

        // System Information (Read-only)
        public string UserRole { get; set; } = "User";
        public DateTime LastLogin { get; set; } = DateTime.Now;
        public string LastLoginIP { get; set; } = "127.0.0.1";
        public DateTime AccountCreated { get; set; } = DateTime.Now;
        public int TotalFilesUploaded { get; set; } = 0;
        public long TotalStorageUsed { get; set; } = 0;

        // Available options
        public List<string> AvailableCategories { get; set; } = new()
        {
            "Documents", "Images", "Videos", "Archives", "Spreadsheets", "Presentations"
        };
    }
}
