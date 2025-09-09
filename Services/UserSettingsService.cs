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
                
                // Load user preferences from database
                var preferences = await _context.UserPreferences
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);
                
                if (preferences != null)
                {
                    // Load saved preferences
                    viewModel.DarkMode = preferences.DarkMode;
                    viewModel.Language = preferences.Language;
                    viewModel.ItemsPerPage = preferences.ItemsPerPage;
                    viewModel.EmailNotifications = preferences.EmailNotifications;
                    viewModel.BrowserNotifications = preferences.BrowserNotifications;
                    viewModel.AutoSave = preferences.AutoSave;
                    
                    // File settings
                    viewModel.DefaultUploadCategory = preferences.DefaultUploadCategory;
                    viewModel.MaxFileSize = preferences.MaxFileSize;
                    viewModel.AutoCategorize = preferences.AutoCategorize;
                    viewModel.DeleteConfirmation = preferences.DeleteConfirmation;
                    viewModel.TwoFactorEnabled = preferences.TwoFactorEnabled;
                    viewModel.SessionTimeout = preferences.SessionTimeout;
                }
                else
                {
                    // Load default preferences if no saved preferences
                    viewModel.DarkMode = false;
                    viewModel.Language = "en";
                    viewModel.ItemsPerPage = 25;
                    viewModel.EmailNotifications = true;
                    viewModel.BrowserNotifications = true;
                    viewModel.AutoSave = true;
                    
                    // File settings
                    viewModel.DefaultUploadCategory = "Documents";
                    viewModel.MaxFileSize = 50;
                    viewModel.AutoCategorize = true;
                    viewModel.DeleteConfirmation = true;
                    viewModel.TwoFactorEnabled = false;
                    viewModel.SessionTimeout = 120;
                }

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
                // Note: PhotoUrl would be updated through separate photo upload functionality

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

        public async Task<bool> SaveUserPreferencesAsync(string userId, SettingsViewModel settings)
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

                // Get or create user preferences
                var preferences = await _context.UserPreferences
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);
                
                if (preferences == null)
                {
                    // Create new preferences
                    preferences = new UserPreferences
                    {
                        UserId = user.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.UserPreferences.Add(preferences);
                }

                // Update preferences
                preferences.DarkMode = settings.DarkMode;
                preferences.Language = settings.Language;
                preferences.ItemsPerPage = settings.ItemsPerPage;
                preferences.EmailNotifications = settings.EmailNotifications;
                preferences.BrowserNotifications = settings.BrowserNotifications;
                preferences.AutoSave = settings.AutoSave;
                preferences.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Preferences saved for user {UserId}: DarkMode={DarkMode}, Language={Language}", 
                    userId, settings.DarkMode, settings.Language);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user preferences for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> SaveFileSettingsAsync(string userId, SettingsViewModel settings)
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

                // Get or create user preferences
                var preferences = await _context.UserPreferences
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);
                
                if (preferences == null)
                {
                    // Create new preferences
                    preferences = new UserPreferences
                    {
                        UserId = user.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.UserPreferences.Add(preferences);
                }

                // Update file settings
                preferences.DefaultUploadCategory = settings.DefaultUploadCategory;
                preferences.MaxFileSize = settings.MaxFileSize;
                preferences.AutoCategorize = settings.AutoCategorize;
                preferences.DeleteConfirmation = settings.DeleteConfirmation;
                preferences.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("File settings saved for user {UserId}: Category={Category}, MaxSize={MaxSize}", 
                    userId, settings.DefaultUploadCategory, settings.MaxFileSize);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file settings for user {UserId}", userId);
                return false;
            }
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
