using blazor_arsip.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace blazor_arsip.Components.Pages.FileManagement.Upload;

public class FileUploadBase : ComponentBase, IDisposable
{
    [Inject] protected IFileService FileService { get; set; } = default!;
    [Inject] protected IFileUploadService FileUploadService { get; set; } = default!;
    [Inject] protected IToastService ToastService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    
    protected FileUploadViewModel viewModel = default!;
    protected InputFile fileInputRef = default!;
    
    protected override void OnInitialized()
    {
        viewModel = new FileUploadViewModel(FileService, FileUploadService, JSRuntime, ToastService);
    }
    
    protected override async Task OnInitializedAsync()
    {
        await viewModel.InitializeAsync();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await viewModel.InitializeDragDropAsync();
        }
    }
    
    protected async Task HandleFileSelection(InputFileChangeEventArgs e)
    {
        await viewModel.HandleFileSelectionAsync(e);
    }

    protected void HandleDragEnter(DragEventArgs e)
    {
        viewModel.IsDragOver = true;
    }

    protected void HandleDragLeave(DragEventArgs e)
    {
        viewModel.IsDragOver = false;
    }

    protected void HandleDrop(DragEventArgs e)
    {
        viewModel.HandleDrop(e);
    }

    protected void RemoveFile(IBrowserFile file)
    {
        viewModel.RemoveFile(file);
    }
    
    protected async Task HandleValidSubmitAsync()
    {
        await viewModel.HandleValidSubmitAsync();
    }
    
    protected void HandleDragOver(DragEventArgs e)
    {
        viewModel.HandleDragOver(e);
    }
    
    protected void TriggerFileInput()
    {
        viewModel.TriggerFileInput();
    }
    
    protected void ClearSelection()
    {
        viewModel.ClearSelection();
    }
    
    public void Dispose()
    {
        // FileUploadViewModel doesn't implement IDisposable
        // No cleanup needed
    }
}