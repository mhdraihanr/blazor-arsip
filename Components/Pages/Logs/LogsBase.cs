using Microsoft.AspNetCore.Components;
using blazor_arsip.Services;

namespace blazor_arsip.Components.Pages.Logs;

public class LogsBase : ComponentBase
{
    [Inject] protected IFileService FileService { get; set; } = null!;
    [Inject] protected ICurrentUserService CurrentUserService { get; set; } = null!;

    protected LogsViewModel viewModel = null!;

    protected override async Task OnInitializedAsync()
    {
        viewModel = new LogsViewModel(FileService);
        await viewModel.LoadActivitiesAsync();
    }

    protected async Task HandleSearch()
    {
        await viewModel.SearchAsync();
        StateHasChanged();
    }

    protected async Task HandleClearFilters()
    {
        viewModel.ClearFilters();
        await viewModel.LoadActivitiesAsync();
        StateHasChanged();
    }

    protected async Task HandleNextPage()
    {
        await viewModel.NextPageAsync();
        StateHasChanged();
    }

    protected async Task HandlePreviousPage()
    {
        await viewModel.PreviousPageAsync();
        StateHasChanged();
    }

    protected async Task HandleGoToPage(int page)
    {
        await viewModel.GoToPageAsync(page);
        StateHasChanged();
    }

    protected string GetActivityIcon(string activityType)
    {
        return activityType.ToLower() switch
        {
            "upload" => "fas fa-upload text-success",
            "download" => "fas fa-download text-info",
            "update" => "fas fa-edit text-warning",
            "delete" => "fas fa-trash text-danger",
            "view" => "fas fa-eye text-primary",
            _ => "fas fa-file text-secondary"
        };
    }

    protected string GetActivityBadgeClass(string activityType)
    {
        return activityType.ToLower() switch
        {
            "upload" => "bg-success",
            "download" => "bg-info", 
            "update" => "bg-warning text-dark",
            "delete" => "bg-danger",
            "view" => "bg-primary",
            _ => "bg-secondary"
        };
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
}
