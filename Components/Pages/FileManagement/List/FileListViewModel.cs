using blazor_arsip.Models;
using blazor_arsip.Services;
using blazor_arsip.Components.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace blazor_arsip.Components.Pages.FileManagement.List;

public class FileListViewModel
{
    private readonly IFileService _fileService;
    private readonly IToastService _toastService;
    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager _navigation;

    public FileListViewModel(IFileService fileService, IToastService toastService, IJSRuntime jsRuntime, NavigationManager navigation)
    {
        _fileService = fileService;
        _toastService = toastService;
        _jsRuntime = jsRuntime;
        _navigation = navigation;
    }

    // Properties
    public bool IsLoading { get; set; } = true;
    public bool IsGridView { get; set; } = true;
    public string SearchTerm { get; set; } = string.Empty;
    
    private string _selectedCategory = string.Empty;
    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            _ = SearchFilesAsync();
        }
    }
    
    private DateTime? _fromDate;
    public DateTime? FromDate
    {
        get => _fromDate;
        set
        {
            _fromDate = value;
            _ = SearchFilesAsync();
        }
    }
    
    private DateTime? _toDate;
    public DateTime? ToDate
    {
        get => _toDate;
        set
        {
            _toDate = value;
            _ = SearchFilesAsync();
        }
    }
    
    public string NameFilter { get; set; } = string.Empty;
    public List<FileRecord> AllFiles { get; set; } = new();
    public List<FileRecord> FilteredFiles { get; set; } = new();
    public List<FileCategory> Categories { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages => (int)Math.Ceiling((double)FilteredFiles.Count / PageSize);

    // Events
    public event Action? StateChanged;

    // Methods
    public async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            AllFiles = (await _fileService.GetAllFilesAsync()).ToList();
            Categories = (await _fileService.GetCategoriesAsync()).ToList();
            FilteredFiles = AllFiles;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading files: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task SearchFilesAsync()
    {
        try
        {
            FilteredFiles = (await _fileService.SearchFilesAsync(SearchTerm, SelectedCategory, FromDate, ToDate)).ToList();
            CurrentPage = 1; // Reset to first page
            StateChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching files: {ex.Message}");
        }
    }

    public async Task OnSearchKeyPressAsync(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SearchFilesAsync();
        }
    }

    public async Task ClearFiltersAsync()
    {
        SearchTerm = string.Empty;
        _selectedCategory = string.Empty;
        _fromDate = null;
        _toDate = null;
        NameFilter = string.Empty;
        await SearchFilesAsync();
    }

    public void SetViewMode(bool gridView)
    {
        IsGridView = gridView;
        StateChanged?.Invoke();
    }

    public void ChangePage(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            CurrentPage = page;
            StateChanged?.Invoke();
        }
    }

    public async Task DownloadFileAsync(int fileId)
    {
        try
        {
            var downloadUrl = $"/api/file/download/{fileId}";
            await _jsRuntime.InvokeVoidAsync("downloadFile", downloadUrl, $"file_{fileId}");
            
            // Show success message
            _toastService.ShowSuccess("File download started!", "Download");
        }
        catch (Exception ex)
        {
            _toastService.ShowError($"Error downloading file: {ex.Message}", "Download Error");
        }
    }

    public async Task<bool> DeleteFileAsync(int fileId, ConfirmationDialog confirmationDialog)
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
                await _fileService.DeleteFileAsync(fileId);
                await LoadDataAsync(); // Refresh the list
                _toastService.ShowSuccess("File deleted successfully.");
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _toastService.ShowError($"Error deleting file: {ex.Message}");
            return false;
        }
    }

    private static async Task<object> NewMethod(ConfirmationDialog confirmationDialog)
    {
        return await confirmationDialog.ShowAsync(
            "Confirm Delete",
            "Are you sure you want to delete this file? This action cannot be undone.",
            "Delete",
            "Cancel");
    }


    public static string GetFileIcon(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "fas fa-file-pdf",
            ".doc" or ".docx" => "fas fa-file-word",
            ".xls" or ".xlsx" => "fas fa-file-excel",
            ".ppt" or ".pptx" => "fas fa-file-powerpoint",
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" => "fas fa-file-image",
            ".mp4" or ".avi" or ".mov" or ".wmv" or ".flv" => "fas fa-file-video",
            ".mp3" or ".wav" or ".flac" or ".aac" => "fas fa-file-audio",
            ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => "fas fa-file-archive",
            ".txt" or ".rtf" => "fas fa-file-alt",
            ".csv" => "fas fa-file-csv",
            _ => "fas fa-file"
        };
    }

    // Navigation methods
    public void ViewFile(int fileId)
    {
        _navigation.NavigateTo($"/file/{fileId}");
    }

    public void EditFile(int fileId)
    {
        _navigation.NavigateTo($"/edit/{fileId}");
    }

    public static string TruncateFileName(string fileName, int maxLength)
    {
        if (fileName.Length <= maxLength) return fileName;
        return fileName.Substring(0, maxLength - 3) + "...";
    }

    public static string FormatFileSize(long bytes)
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