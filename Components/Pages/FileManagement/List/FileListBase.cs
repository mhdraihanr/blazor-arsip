using blazor_arsip.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using blazor_arsip.Components.Shared;

namespace blazor_arsip.Components.Pages.FileManagement.List;

public class FileListBase : ComponentBase, IDisposable
{
    [Inject] protected IFileService FileService { get; set; } = default!;
    [Inject] protected IToastService ToastService { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected ICurrentUserService CurrentUserService { get; set; } = default!;
    [Inject] protected IIpAddressService IpAddressService { get; set; } = default!;
    
    protected FileListViewModel viewModel = default!;
    protected ConfirmationDialog confirmationDialog = default!;

    protected override void OnInitialized()
    {
        viewModel = new FileListViewModel(FileService, ToastService, JSRuntime, Navigation);
        viewModel.StateChanged += () => InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        await viewModel.LoadDataAsync();
    }
    
    protected async Task<bool> DeleteFileWithContext(int fileId)
    {
        try
        {
            var confirmed = await confirmationDialog.ShowAsync(
                "Confirm Delete",
                "Are you sure you want to delete this file? This action cannot be undone.",
                "Delete",
                "Cancel");
                
            if (confirmed)
            {
                var currentUser = await CurrentUserService.GetCurrentUserAsync();
                var userEmail = currentUser?.Email ?? "Unknown";
                var ipAddress = IpAddressService.GetClientIpAddress();
                var userAgent = IpAddressService.GetUserAgent();
                
                await FileService.DeleteFileAsync(fileId, userEmail, ipAddress, userAgent);
                await viewModel.LoadDataAsync(); // Refresh the list
                ToastService.ShowSuccess("File deleted successfully.");
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error deleting file: {ex.Message}");
            return false;
        }
    }
    
    public void Dispose()
    {
        if (viewModel != null)
        {
            viewModel.StateChanged -= () => InvokeAsync(StateHasChanged);
        }
    }
}