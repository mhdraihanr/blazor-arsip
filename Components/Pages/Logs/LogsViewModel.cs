using blazor_arsip.Models;
using blazor_arsip.Services;

namespace blazor_arsip.Components.Pages.Logs;

public class LogsViewModel
{
    private readonly IFileService _fileService;

    public LogsViewModel(IFileService fileService)
    {
        _fileService = fileService;
    }

    // Properties
    public bool IsLoading { get; set; } = true;
    public List<FileActivityWithUser> Activities { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int TotalPages { get; set; } = 1;
    public int TotalActivities { get; set; } = 0;
    public string SearchTerm { get; set; } = string.Empty;
    public string SelectedActivityType { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    // Methods
    public async Task LoadActivitiesAsync()
    {
        try
        {
            IsLoading = true;
            var allActivities = await _fileService.GetAllActivitiesWithUsersAsync(
                searchTerm: SearchTerm,
                activityType: SelectedActivityType,
                fromDate: FromDate,
                toDate: ToDate,
                page: CurrentPage,
                pageSize: PageSize
            );
            
            Activities = allActivities.Activities;
            TotalActivities = allActivities.TotalCount;
            TotalPages = (int)Math.Ceiling((double)TotalActivities / PageSize);
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Error loading activities: {ex.Message}");
            Activities = new List<FileActivityWithUser>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task SearchAsync()
    {
        CurrentPage = 1;
        await LoadActivitiesAsync();
    }

    public async Task NextPageAsync()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadActivitiesAsync();
        }
    }

    public async Task PreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadActivitiesAsync();
        }
    }

    public async Task GoToPageAsync(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            CurrentPage = page;
            await LoadActivitiesAsync();
        }
    }

    public void ClearFilters()
    {
        SearchTerm = string.Empty;
        SelectedActivityType = string.Empty;
        FromDate = null;
        ToDate = null;
        CurrentPage = 1;
    }
}

public class FileActivityWithUser
{
    public int Id { get; set; }
    public int FileRecordId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public string PerformedByName { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public FileRecord FileRecord { get; set; } = null!;
}

public class ActivitiesResult
{
    public List<FileActivityWithUser> Activities { get; set; } = new();
    public int TotalCount { get; set; }
}
