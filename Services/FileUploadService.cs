using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using blazor_arsip.Models;

namespace blazor_arsip.Services;

public interface IFileUploadService
{
    Task<List<IBrowserFile>> ProcessDroppedFilesAsync(InputFileChangeEventArgs e);
    Task<List<IBrowserFile>> ProcessSelectedFilesAsync(InputFileChangeEventArgs e);
    Task<bool> ValidateFileAsync(IBrowserFile file, string[] allowedExtensions, long maxFileSize);
    Task<List<FileValidationResult>> ValidateFilesAsync(IReadOnlyList<IBrowserFile> files, string[] allowedExtensions, long maxFileSize);
    string GetFileIcon(string fileName);
    string FormatFileSize(long bytes);
}

public class FileValidationResult
{
    public IBrowserFile File { get; set; } = null!;
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}

public class FileUploadService : IFileUploadService
{
    private readonly ILogger<FileUploadService> _logger;
    private readonly IJSRuntime _jsRuntime;

    public FileUploadService(ILogger<FileUploadService> logger, IJSRuntime jsRuntime)
    {
        _logger = logger;
        _jsRuntime = jsRuntime;
    }

    public async Task<List<IBrowserFile>> ProcessDroppedFilesAsync(InputFileChangeEventArgs e)
    {
        _logger.LogInformation($"Processing {e.FileCount} dropped files");
        
        var files = new List<IBrowserFile>();
        
        foreach (var file in e.GetMultipleFiles(10)) // Max 10 files at once
        {
            _logger.LogInformation($"Processing dropped file: {file.Name}, Size: {file.Size} bytes");
            files.Add(file);
        }
        
        return files;
    }

    public async Task<List<IBrowserFile>> ProcessSelectedFilesAsync(InputFileChangeEventArgs e)
    {
        _logger.LogInformation($"Processing {e.FileCount} selected files");
        
        var files = new List<IBrowserFile>();
        
        foreach (var file in e.GetMultipleFiles(10)) // Max 10 files at once
        {
            _logger.LogInformation($"Processing selected file: {file.Name}, Size: {file.Size} bytes");
            files.Add(file);
        }
        
        return files;
    }

    public async Task<bool> ValidateFileAsync(IBrowserFile file, string[] allowedExtensions, long maxFileSize)
    {
        // Check file size
        if (file.Size > maxFileSize)
        {
            _logger.LogWarning($"File {file.Name} exceeds maximum size limit: {file.Size} > {maxFileSize}");
            return false;
        }

        // Check file extension
        var extension = Path.GetExtension(file.Name).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            _logger.LogWarning($"File {file.Name} has invalid extension: {extension}");
            return false;
        }

        // Additional validation can be added here (e.g., MIME type validation)
        
        return true;
    }

    public async Task<List<FileValidationResult>> ValidateFilesAsync(IReadOnlyList<IBrowserFile> files, string[] allowedExtensions, long maxFileSize)
    {
        var results = new List<FileValidationResult>();
        
        foreach (var file in files)
        {
            var result = new FileValidationResult
            {
                File = file,
                IsValid = true
            };

            // Check file size
            if (file.Size > maxFileSize)
            {
                result.IsValid = false;
                result.ErrorMessage = $"File size ({FormatFileSize(file.Size)}) exceeds maximum limit ({FormatFileSize(maxFileSize)})";
            }
            // Check file extension
            else
            {
                var extension = Path.GetExtension(file.Name).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    result.IsValid = false;
                    result.ErrorMessage = $"File type '{extension}' is not allowed";
                }
            }

            results.Add(result);
            
            if (!result.IsValid)
            {
                _logger.LogWarning($"File validation failed for {file.Name}: {result.ErrorMessage}");
            }
        }
        
        return results;
    }

    public string GetFileIcon(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        
        return extension switch
        {
            ".pdf" => "fas fa-file-pdf text-danger",
            ".doc" or ".docx" => "fas fa-file-word text-primary",
            ".xls" or ".xlsx" => "fas fa-file-excel text-success",
            ".ppt" or ".pptx" => "fas fa-file-powerpoint text-warning",
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" => "fas fa-file-image text-info",
            ".mp4" or ".avi" or ".mov" or ".wmv" or ".flv" => "fas fa-file-video text-purple",
            ".mp3" or ".wav" or ".flac" or ".aac" => "fas fa-file-audio text-success",
            ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => "fas fa-file-archive text-secondary",
            ".txt" => "fas fa-file-alt text-muted",
            ".csv" => "fas fa-file-csv text-success",
            _ => "fas fa-file text-muted"
        };
    }

    public string FormatFileSize(long bytes)
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