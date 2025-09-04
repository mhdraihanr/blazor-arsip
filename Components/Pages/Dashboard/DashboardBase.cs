using blazor_arsip.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace blazor_arsip.Components.Pages.Dashboard;

public class DashboardBase : ComponentBase, IDisposable
{
    [Inject] protected IFileService FileService { get; set; } = default!;
    [Inject] protected IToastService ToastService { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    
    protected DashboardViewModel viewModel = default!;
    private DotNetObjectReference<DashboardBase>? _objRef;
    
    protected override void OnInitialized()
    {
        viewModel = new DashboardViewModel(FileService);
        _objRef = DotNetObjectReference.Create(this);
    }
    
    protected override async Task OnInitializedAsync()
    {
        await viewModel.LoadDashboardDataAsync();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("setDashboardInstance", _objRef);
            await JSRuntime.InvokeVoidAsync("eval", @"
                if (!window.fileDownloadedListenerAdded) {
                    window.addEventListener('fileDownloaded', function(event) {
                        setTimeout(function() {
                            if (window.dashboardInstance) {
                                window.dashboardInstance.invokeMethodAsync('RefreshStats');
                            }
                        }, 1000);
                    });
                    window.fileDownloadedListenerAdded = true;
                }
            ");
        }
    }
    
    [JSInvokable]
    public async Task RefreshStats()
    {
        await viewModel.LoadDashboardDataAsync();
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
    
    public void Dispose()
    {
        _objRef?.Dispose();
    }
}