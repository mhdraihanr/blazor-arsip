using blazor_arsip.Models;
using blazor_arsip.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Headers;

namespace blazor_arsip.Components.Pages.FileManagement.Upload;

public class FileUploadViewModel
{
    private readonly IFileService _fileService;
    private readonly IFileUploadService _fileUploadService;
    private readonly IJSRuntime _jsRuntime;
    private readonly IToastService _toastService;

    public FileUploadViewModel(IFileService fileService, IFileUploadService fileUploadService, IJSRuntime jsRuntime, IToastService toastService)
    {
        _fileService = fileService;
        _fileUploadService = fileUploadService;
        _jsRuntime = jsRuntime;
        _toastService = toastService;
        UploadModel = new UploadModel { Category = "Documents" };
    }

    // Properties
    public UploadModel UploadModel { get; set; }
    public List<IBrowserFile> SelectedFiles { get; set; } = new();
    public List<FileCategory> Categories { get; set; } = new();
    public Dictionary<string, int> UploadProgress { get; set; } = new();
    
    public bool IsUploading { get; set; } = false;
    public bool IsDragOver { get; set; } = false;
    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;
    
    private readonly long _maxFileSize = 100 * 1024 * 1024; // 100MB
    private readonly string[] _allowedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".jpg", ".jpeg", ".png", ".gif", ".zip", ".rar", ".7z" };
    
    public string[] AllowedExtensions => _allowedExtensions;
    public long MaxFileSize => _maxFileSize;

    // Events
    public event Action? StateChanged;

    // Methods
    public async Task InitializeAsync()
    {
        Categories = (await _fileService.GetCategoriesAsync()).ToList();
        UploadModel.Category = "Documents";
    }

    public async Task SetupDragAndDropAsync()
    {
        await _jsRuntime.InvokeVoidAsync("setupDragAndDrop", "dropZone", "fileInput");
    }
    
    public async Task InitializeDragDropAsync()
    {
        await SetupDragAndDropAsync();
    }

    public async Task HandleFileSelectionAsync(InputFileChangeEventArgs e)
    {
        Console.WriteLine($"HandleFileSelection called with {e.FileCount} files");
        
        try
        {
            var files = await _fileUploadService.ProcessSelectedFilesAsync(e);
            await ProcessFilesAsync(files);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error processing selected files: {ex.Message}";
            StateChanged?.Invoke();
        }
    }

    public void HandleDrop(DragEventArgs e)
    {
        IsDragOver = false;
        // The actual file handling is done by JavaScript
        // which will trigger the InputFile change event
    }
    
    public void TriggerFileInput()
    {
        // File input will be triggered by the label click or button click
        // No JavaScript interop needed in Blazor Server
        Console.WriteLine("File input triggered via HTML label");
    }

    public void HandleDragOver(DragEventArgs e)
    {
        // Prevent default behavior is handled by @ondragover:preventDefault="true"
    }

    public void HandleDragEnter(DragEventArgs e)
    {
        IsDragOver = true;
    }

    public void HandleDragLeave(DragEventArgs e)
    {
        IsDragOver = false;
    }

    public async Task ProcessFilesAsync(List<IBrowserFile> files)
    {
        Console.WriteLine($"ProcessFilesAsync called with {files.Count} files");
        ErrorMessage = string.Empty;
        
        try
        {
            // Validate files using FileUploadService
            var validationResults = await _fileUploadService.ValidateFilesAsync(files, _allowedExtensions, _maxFileSize);
            
            var validFiles = new List<IBrowserFile>();
            var errors = new List<string>();

            foreach (var result in validationResults)
            {
                if (result.IsValid)
                {
                    // Check if file already selected - for drag & drop, replace existing
                    var existingFile = SelectedFiles.FirstOrDefault(f => f.Name == result.File.Name);
                    if (existingFile != null)
                    {
                        SelectedFiles.Remove(existingFile);
                        UploadProgress.Remove(existingFile.Name);
                        Console.WriteLine($"Replaced existing file: {result.File.Name}");
                    }
                    
                    validFiles.Add(result.File);
                }
                else
                {
                    errors.Add($"{result.File.Name}: {result.ErrorMessage}");
                }
            }

            SelectedFiles.AddRange(validFiles);

            if (errors.Any())
            {
                ErrorMessage = string.Join("<br>", errors);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error validating files: {ex.Message}";
        }
        
        StateChanged?.Invoke();
    }

    public void RemoveFile(IBrowserFile file)
    {
        SelectedFiles.Remove(file);
        UploadProgress.Remove(file.Name);
        StateChanged?.Invoke();
    }

    public void ClearSelection()
    {
        SelectedFiles.Clear();
        UploadProgress.Clear();
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        StateChanged?.Invoke();
    }

    public async Task HandleValidSubmitAsync()
    {
        if (!SelectedFiles.Any())
        {
            _toastService.ShowError("Silakan pilih setidaknya satu file untuk diunggah.", "File Tidak Dipilih");
            return;
        }

        IsUploading = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        var uploadedCount = 0;
        var failedCount = 0;

        // Create a copy of the files list to avoid issues with IBrowserFile references
        var filesToUpload = SelectedFiles.ToList();

        try
        {
            foreach (var file in filesToUpload)
            {
                try
                {
                    UploadProgress[file.Name] = 0;
                    StateChanged?.Invoke();

                    // Create FormFileWrapper asynchronously to avoid synchronous read issues
                    FormFileWrapper formFile;
                    try
                    {
                        formFile = await FormFileWrapper.CreateAsync(file);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Failed to read file '{file.Name}': {ex.Message}", ex);
                    }

                    // Simulate upload progress
                    for (int i = 0; i <= 100; i += 10)
                    {
                        UploadProgress[file.Name] = i;
                        StateChanged?.Invoke();
                        await Task.Delay(50); // Simulate upload time
                    }

                    await _fileService.UploadFileAsync(
                        formFile,
                        "Current User", // In a real app, get from authentication
                        UploadModel.Description,
                        UploadModel.Tags,
                        UploadModel.Category
                    );

                    uploadedCount++;
                }
                catch (InvalidOperationException ex)
                {
                    failedCount++;
                    _toastService.ShowError($"Error membaca file '{file.Name}': {ex.Message}", "Error File");
                    Console.WriteLine($"File read error for {file.Name}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    failedCount++;
                    Console.WriteLine($"Error uploading {file.Name}: {ex.Message}");
                    
                    // Provide more specific error messages
                    if (ex.Message.Contains("Synchronous reads are not supported"))
                    {
                        _toastService.ShowError("Error membaca file: Silakan coba unggah file lagi.", "Error Upload");
                    }
                    else if (ex.Message.Contains("size") || ex.Message.Contains("large"))
                    {
                        _toastService.ShowError($"File '{file.Name}' terlalu besar. Ukuran maksimum adalah {FormatFileSize(_maxFileSize)}.", "File Terlalu Besar");
                    }
                    else if (ex.Message.Contains("format") || ex.Message.Contains("extension"))
                    {
                        _toastService.ShowError($"File '{file.Name}' memiliki format yang tidak didukung. Format yang diizinkan: {string.Join(", ", _allowedExtensions)}.", "Format Tidak Didukung");
                    }
                    else
                    {
                        _toastService.ShowError($"Upload gagal untuk '{file.Name}': {ex.Message}", "Error Upload");
                    }
                }
            }

            if (uploadedCount > 0)
            {
                if (failedCount > 0)
                {
                    _toastService.ShowWarning($"Berhasil mengunggah {uploadedCount} file. {failedCount} file gagal diunggah.", "Upload Selesai");
                }
                else
                {
                    _toastService.ShowSuccess($"Berhasil mengunggah {uploadedCount} file.", "Upload Berhasil");
                }

                // Clear form after successful upload
                SelectedFiles.Clear();
                UploadProgress.Clear();
                UploadModel = new UploadModel { Category = "Documents" };
            }
            else
            {
                _toastService.ShowError("Semua file gagal diunggah. Silakan coba lagi.", "Upload Gagal");
            }
        }
        catch (Exception ex)
        {
            _toastService.ShowError($"Upload gagal: {ex.Message}", "Error Upload");
        }
        finally
        {
            IsUploading = false;
            StateChanged?.Invoke();
        }
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
            ".jpg" or ".jpeg" or ".png" or ".gif" => "fas fa-file-image",
            ".zip" or ".rar" or ".7z" => "fas fa-file-archive",
            ".txt" => "fas fa-file-alt",
            _ => "fas fa-file"
        };
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



    // Wrapper class to convert IBrowserFile to IFormFile
    public class FormFileWrapper : IFormFile
    {
        private readonly string _contentType;
        private readonly string _fileName;
        private readonly long _length;
        private readonly byte[] _fileData;

        private FormFileWrapper(string contentType, string fileName, long length, byte[] fileData)
        {
            _contentType = contentType;
            _fileName = fileName;
            _length = length;
            _fileData = fileData;
        }

        public static async Task<FormFileWrapper> CreateAsync(IBrowserFile file)
        {
            try
            {
                // Read file data asynchronously to avoid synchronous read issues
                using var stream = file.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var fileData = memoryStream.ToArray();
                
                return new FormFileWrapper(file.ContentType, file.Name, file.Size, fileData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file data for {file.Name}: {ex.Message}");
                throw new InvalidOperationException($"Failed to read file data: {ex.Message}", ex);
            }
        }

        public string ContentType => _contentType;
        public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{_fileName}\"";
        public IHeaderDictionary Headers => new HeaderDictionary();
        public long Length => _length;
        public string Name => "file";
        public string FileName => _fileName;

        public Stream OpenReadStream() 
        {
            try 
            {
                return new MemoryStream(_fileData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening read stream for {_fileName}: {ex.Message}");
                throw;
            }
        }

        public void CopyTo(Stream target)
        {
            using var stream = OpenReadStream();
            stream.CopyTo(target);
        }

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            using var stream = OpenReadStream();
            await stream.CopyToAsync(target, cancellationToken);
        }
    }
}