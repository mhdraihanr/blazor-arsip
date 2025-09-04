using blazor_arsip.Services;
using Microsoft.AspNetCore.Components;

namespace blazor_arsip.Components.Pages.FileManagement.Edit;

public class EditFileBase : ComponentBase, IDisposable
{
    [Parameter] public int Id { get; set; }
    
    [Inject] protected IFileService FileService { get; set; } = default!;
    [Inject] protected IToastService ToastService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    
    protected EditFileViewModel viewModel = default!;

    protected override void OnInitialized()
    {
        viewModel = new EditFileViewModel(FileService, ToastService);
        viewModel.StateChanged += () => InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        await viewModel.LoadFileDetailsAsync(Id);
    }
    
    public void Dispose()
    {
        if (viewModel != null)
        {
            viewModel.StateChanged -= () => InvokeAsync(StateHasChanged);
        }
    }
}