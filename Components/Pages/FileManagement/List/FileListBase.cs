using blazor_arsip.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace blazor_arsip.Components.Pages.FileManagement.List;

public class FileListBase : ComponentBase, IDisposable
{
    [Inject] protected IFileService FileService { get; set; } = default!;
    [Inject] protected IToastService ToastService { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    
    protected FileListViewModel viewModel = default!;
    protected blazor_arsip.Components.Shared.ConfirmationDialog confirmationDialog = default!;

    protected override void OnInitialized()
    {
        viewModel = new FileListViewModel(FileService, ToastService, JSRuntime, Navigation);
        viewModel.StateChanged += () => InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        await viewModel.LoadDataAsync();
    }
    
    public void Dispose()
    {
        if (viewModel != null)
        {
            viewModel.StateChanged -= () => InvokeAsync(StateHasChanged);
        }
    }
}