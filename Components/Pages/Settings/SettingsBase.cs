using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using blazor_arsip.Services;
using System.Security.Claims;

namespace blazor_arsip.Components.Pages.Settings
{
    public class SettingsBase : ComponentBase
    {
        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Inject]
        protected IToastService ToastService { get; set; } = default!;

        [Inject]
        protected IUserSettingsService UserSettingsService { get; set; } = default!;

        protected SettingsViewModel viewModel = new();
        protected bool isLoading = false;
        protected bool isSaving = false;
        protected string activeTab = "profile";

        protected override async Task OnInitializedAsync()
        {
            await LoadUserData();
        }

        protected async Task LoadUserData()
        {
            try
            {
                isLoading = true;
                StateHasChanged();

                if (AuthenticationStateTask != null)
                {
                    var authState = await AuthenticationStateTask;
                    var user = authState.User;

                    if (user?.Identity?.IsAuthenticated == true)
                    {
                        var userEmail = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst(ClaimTypes.Name)?.Value;
                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            // Load user settings from database using email as identifier
                            var userSettings = await UserSettingsService.GetUserSettingsAsync(userEmail);
                            if (userSettings != null)
                            {
                                viewModel = userSettings;
                                
                                // Load user statistics
                                var stats = await UserSettingsService.GetUserStatsAsync(userEmail);
                                viewModel.TotalFilesUploaded = (int)(stats.GetValueOrDefault("TotalFilesUploaded", 0));
                                viewModel.TotalStorageUsed = (long)(stats.GetValueOrDefault("TotalStorageUsed", 0L));
                            }
                            else
                            {
                                // Fallback to claims if service fails
                                viewModel.FullName = user.FindFirst(ClaimTypes.Name)?.Value ?? "User";
                                viewModel.Email = user.FindFirst(ClaimTypes.Email)?.Value ?? "user@example.com";
                                viewModel.UserRole = user.FindFirst(ClaimTypes.Role)?.Value ?? "User";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Error loading settings: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        protected async Task SaveSettings()
        {
            try
            {
                isSaving = true;
                StateHasChanged();

                if (AuthenticationStateTask != null)
                {
                    var authState = await AuthenticationStateTask;
                    var user = authState.User;
                    var userEmail = user?.FindFirst(ClaimTypes.Email)?.Value ?? user?.FindFirst(ClaimTypes.Name)?.Value;

                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        // Save all settings based on active tab
                        bool success = false;
                        
                        if (activeTab == "files")
                        {
                            success = await UserSettingsService.SaveFileSettingsAsync(userEmail, viewModel);
                        }
                        else
                        {
                            // Save file settings (excluding profile and system tabs)
                            success = await UserSettingsService.SaveFileSettingsAsync(userEmail, viewModel);
                        }

                        if (success)
                        {
                            ToastService.ShowSuccess("Settings saved successfully!");
                        }
                        else
                        {
                            ToastService.ShowError("Error saving settings. Please try again.");
                        }
                    }
                    else
                    {
                        ToastService.ShowError("User not found. Please login again.");
                    }
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Error saving settings: {ex.Message}");
            }
            finally
            {
                isSaving = false;
                StateHasChanged();
            }
        }

        protected async Task ResetToDefaults()
        {
            try
            {
                // Reset file management settings to default values
                viewModel.DefaultUploadCategory = "Documents";
                viewModel.AutoCategorize = true;
                viewModel.DeleteConfirmation = true;
                viewModel.MaxFileSize = 50;

                await SaveSettings();
                ToastService.ShowSuccess("Settings reset to defaults!");
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Error resetting settings: {ex.Message}");
            }
        }

        protected void SetActiveTab(string tabName)
        {
            activeTab = tabName;
            StateHasChanged();
        }

        protected string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        // Disabled methods for future implementation
        // These features are temporarily disabled as requested
    }
}
