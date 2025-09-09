using blazor_arsip.Components.Pages.Settings;

namespace blazor_arsip.Services
{
    public interface IUserSettingsService
    {
        Task<SettingsViewModel?> GetUserSettingsAsync(string userId);
        Task<bool> SaveUserProfileAsync(string userId, SettingsViewModel settings);
        Task<bool> SaveUserPreferencesAsync(string userId, SettingsViewModel settings);
        Task<bool> SaveFileSettingsAsync(string userId, SettingsViewModel settings);
        Task<Dictionary<string, object>> GetUserStatsAsync(string userId);
    }
}
