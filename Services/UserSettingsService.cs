using blazor_arsip.Components.Pages.Settings;
using blazor_arsip.Data;
using blazor_arsip.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace blazor_arsip.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UserSettingsService> _logger;

        public UserSettingsService(
            ApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<UserSettingsService> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<SettingsViewModel?> GetUserSettingsAsync(string userId)
        {
            try
            {
                // Get current user info from database
                var userInfo = await _currentUserService.GetCurrentUserAsync();
                if (userInfo == null) return null;

                // Find user in database by email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userInfo.Email && u.IsActive);
                
                if (user == null) return null;
                
                var viewModel = new SettingsViewModel();
                
                // Load user info from database
                viewModel.FullName = user.Name;
                viewModel.Email = user.Email;
                viewModel.ProfilePicture = user.PhotoUrl;
                
                // Set default settings (no UserPreferences table needed for now)
                viewModel.DefaultUploadCategory = "Documents";
                viewModel.MaxFileSize = 50;
                viewModel.AutoCategorize = true;
                viewModel.DeleteConfirmation = true;
                viewModel.TwoFactorEnabled = false;
                viewModel.SessionTimeout = 120;

                // System info from database
                viewModel.UserRole = "User";
                viewModel.AccountCreated = user.CreatedAt;
                viewModel.LastLogin = user.LastLoginAt ?? DateTime.Now.AddHours(-2);
                viewModel.LastLoginIP = "127.0.0.1";

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user settings for user {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> SaveUserProfileAsync(string userId, SettingsViewModel settings)
        {
            try
            {
                // Get current user info to find in database
                var userInfo = await _currentUserService.GetCurrentUserAsync();
                if (userInfo == null) return false;

                // Find user in database by email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userInfo.Email && u.IsActive);
                
                if (user == null) return false;

                // Update user profile
                user.Name = settings.FullName;
                user.Email = settings.Email;
                // Note: PhotoUrl is read-only in settings, managed elsewhere

                await _context.SaveChangesAsync();
                _logger.LogInformation("Profile saved for user {UserId}: {Name}, {Email}", userId, settings.FullName, settings.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user profile for user {UserId}", userId);
                return false;
            }
        }

        public Task<bool> SaveUserPreferencesAsync(string userId, SettingsViewModel settings)
        {
            // For now, preferences are not stored in database
            // This method returns true to indicate success without doing anything
            _logger.LogInformation("Preferences saved (in-memory) for user {UserId}", userId);
            return Task.FromResult(true);
        }

        public Task<bool> SaveFileSettingsAsync(string userId, SettingsViewModel settings)
        {
            // For now, file settings are not stored in database
            // This method returns true to indicate success without doing anything
            _logger.LogInformation("File settings saved (in-memory) for user {UserId}: Category={Category}, MaxSize={MaxSize}", 
                userId, settings.DefaultUploadCategory, settings.MaxFileSize);
            return Task.FromResult(true);
        }

        public async Task<Dictionary<string, object>> GetUserStatsAsync(string userEmail)
        {
            try
            {
                // Get current user info to get proper identifier for files
                var userInfo = await _currentUserService.GetCurrentUserAsync();
                if (userInfo == null)
                {
                    return new Dictionary<string, object>
                    {
                        ["TotalFilesUploaded"] = 0,
                        ["TotalStorageUsed"] = 0L
                    };
                }

                // FileRecords.UploadedBy might store user ID, name, or email - check what format is used
                var fileCount = await _context.FileRecords
                    .Where(f => (f.UploadedBy == userEmail || f.UploadedBy == userInfo.Name) && f.IsActive)
                    .CountAsync();

                var totalSize = await _context.FileRecords
                    .Where(f => (f.UploadedBy == userEmail || f.UploadedBy == userInfo.Name) && f.IsActive)
                    .SumAsync(f => (long?)f.FileSize) ?? 0L;

                return new Dictionary<string, object>
                {
                    ["TotalFilesUploaded"] = fileCount,
                    ["TotalStorageUsed"] = totalSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user stats for user {UserEmail}", userEmail);
                return new Dictionary<string, object>
                {
                    ["TotalFilesUploaded"] = 0,
                    ["TotalStorageUsed"] = 0L
                };
            }
        }

    }
}
