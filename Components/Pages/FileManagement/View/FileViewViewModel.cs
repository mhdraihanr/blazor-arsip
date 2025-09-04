using blazor_arsip.Models;
using blazor_arsip.Services;
using Microsoft.JSInterop;

namespace blazor_arsip.Components.Pages.FileManagement.View;

public class FileViewViewModel
{
    private readonly IFileService _fileService;
    private readonly IJSRuntime _jsRuntime;
    
    public FileViewViewModel(IFileService fileService, IJSRuntime jsRuntime)
    {
        _fileService = fileService;
        _jsRuntime = jsRuntime;
    }
    
    // Properties
    public bool IsLoading { get; private set; } = true;
    public FileRecord? FileRecord { get; private set; }
    
    // Events
    public event Action? StateChanged;
    
    // Methods
    public async Task LoadFileAsync(int id)
    {
        try
        {
            IsLoading = true;
            StateChanged?.Invoke();
            
            FileRecord = await _fileService.GetFileByIdAsync(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading file: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateChanged?.Invoke();
        }
    }
    
    public async Task DownloadFileAsync(int id)
    {
        try
        {
            var downloadUrl = $"/api/file/download/{id}";
            await _jsRuntime.InvokeVoidAsync("downloadFile", downloadUrl, $"file_{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading file: {ex.Message}");
        }
    }
    
    public bool IsPreviewSupported()
    {
        if (FileRecord == null) return false;
        
        var extension = FileRecord.FileExtension.ToLowerInvariant();
        var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".txt" };
        return supportedTypes.Contains(extension);
    }
    
    public bool IsImageFile()
    {
        if (FileRecord == null) return false;
        
        var extension = FileRecord.FileExtension.ToLowerInvariant();
        var imageTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        return imageTypes.Contains(extension);
    }
    
    public bool IsPdfFile()
    {
        if (FileRecord == null) return false;
        return FileRecord.FileExtension.ToLowerInvariant() == ".pdf";
    }
    
    public bool IsTextFile()
    {
        if (FileRecord == null) return false;
        return FileRecord.FileExtension.ToLowerInvariant() == ".txt";
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
    
    public static string GetFileIcon(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".pdf" => "fas fa-file-pdf",
            ".doc" or ".docx" => "fas fa-file-word",
            ".xls" or ".xlsx" => "fas fa-file-excel",
            ".ppt" or ".pptx" => "fas fa-file-powerpoint",
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".svg" => "fas fa-file-image",
            ".mp4" or ".avi" or ".mov" or ".wmv" or ".flv" => "fas fa-file-video",
            ".mp3" or ".wav" or ".flac" or ".aac" => "fas fa-file-audio",
            ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => "fas fa-file-archive",
            ".txt" or ".rtf" => "fas fa-file-alt",
            _ => "fas fa-file"
        };
    }
}