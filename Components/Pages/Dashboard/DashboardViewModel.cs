using blazor_arsip.Models;
using blazor_arsip.Services;

namespace blazor_arsip.Components.Pages.Dashboard;

public class DashboardViewModel
{
    private readonly IFileService _fileService;

    public DashboardViewModel(IFileService fileService)
    {
        _fileService = fileService;
    }

    // Properties
    public bool IsLoading { get; set; } = true;
    public int TotalFiles { get; set; } = 0;
    public long TotalSize { get; set; } = 0;
    public int TodayUploads { get; set; } = 0;
    public int TotalDownloads { get; set; } = 0;
    public List<dynamic> CategoryStats { get; set; } = new();
    public List<FileActivity> RecentActivities { get; set; } = new();

    // Methods
    public async Task LoadDashboardDataAsync()
    {
        try
        {
            var stats = await _fileService.GetDashboardStatsAsync();
            
            TotalFiles = (int)stats["totalFiles"];
            TotalSize = (long)stats["totalSize"];
            TodayUploads = (int)stats["todayUploads"];
            TotalDownloads = (int)stats["totalDownloads"];
            CategoryStats = ((IEnumerable<dynamic>)stats["categoryStats"]).ToList();
            RecentActivities = ((IEnumerable<FileActivity>)stats["recentActivities"]).ToList();
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Error loading dashboard data: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

}