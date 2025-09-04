using blazor_arsip.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace blazor_arsip.Components.Pages.FileManagement.View;

public class FileViewBase : ComponentBase, IDisposable
{
    [Parameter] public int Id { get; set; }
    
    [Inject] protected IFileService FileService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    
    protected FileViewViewModel viewModel = default!;
    
    protected override void OnInitialized()
    {
        viewModel = new FileViewViewModel(FileService, JSRuntime);
        viewModel.StateChanged += StateHasChanged;
    }
    
    protected override async Task OnInitializedAsync()
    {
        await viewModel.LoadFileAsync(Id);
    }
    
    protected async Task DownloadFile()
    {
        await viewModel.DownloadFileAsync(Id);
    }
    
    protected void EditFile()
    {
        Navigation.NavigateTo($"/edit/{Id}");
    }
    
    public void Dispose()
    {
        if (viewModel != null)
        {
            viewModel.StateChanged -= StateHasChanged;
        }
    }
}