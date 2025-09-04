using System.ComponentModel.DataAnnotations;
using blazor_arsip.Models;
using blazor_arsip.Services;

namespace blazor_arsip.Components.Pages.FileManagement.Edit;

public class EditFileViewModel
{
    private readonly IFileService _fileService;
    private readonly IToastService _toastService;
    
    public EditFileViewModel(IFileService fileService, IToastService toastService)
    {
        _fileService = fileService;
        _toastService = toastService;
        EditModel = new EditFileModel();
    }
    
    // Properties
    public FileRecord? FileRecord { get; private set; }
    public EditFileModel EditModel { get; private set; }
    public List<FileCategory> Categories { get; set; } = new();
    public bool IsLoading { get; private set; } = true;
    public bool IsSaving { get; private set; } = false;
    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;
    
    // Events
    public event Action? StateChanged;
    
    // Methods
    public async Task LoadFileDetailsAsync(int id)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            StateChanged?.Invoke();
            
            // Load categories
            Categories = (await _fileService.GetCategoriesAsync()).ToList();
            
            FileRecord = await _fileService.GetFileByIdAsync(id);
            
            if (FileRecord != null)
            {
                EditModel.FileName = FileRecord.FileName;
                EditModel.Description = FileRecord.Description ?? string.Empty;
                EditModel.Tags = FileRecord.Tags ?? string.Empty;
                EditModel.Category = FileRecord.Category ?? "Documents";
            }
        }
        catch (Exception ex)
        {
            _toastService.ShowError($"Error memuat file: {ex.Message}", "Error Load File");
        }
        finally
        {
            IsLoading = false;
            StateChanged?.Invoke();
        }
    }
    
    public async Task HandleValidSubmitAsync()
    {
        try
        {
            IsSaving = true;
            ErrorMessage = string.Empty;
            StateChanged?.Invoke();
            
            if (FileRecord != null)
            {
                FileRecord.FileName = EditModel.FileName;
                FileRecord.Description = EditModel.Description;
                FileRecord.Tags = EditModel.Tags;
                FileRecord.Category = EditModel.Category;
                FileRecord.ModifiedBy = "System";
                
                await _fileService.UpdateFileAsync(FileRecord);
                
                _toastService.ShowSuccess("File berhasil diperbarui!", "Update Berhasil");
                await LoadFileDetailsAsync(FileRecord.Id);
            }
        }
        catch (Exception ex)
        {
            _toastService.ShowError($"Error memperbarui file: {ex.Message}", "Error Update");
        }
        finally
        {
            IsSaving = false;
            StateChanged?.Invoke();
        }
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

public class EditFileModel
{
    [Required(ErrorMessage = "File name is required")]
    public string FileName { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Tags { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Category is required")]
    public string Category { get; set; } = "Documents";
}