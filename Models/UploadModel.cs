using System.ComponentModel.DataAnnotations;

namespace blazor_arsip.Models;

public class UploadModel
{
    [Required(ErrorMessage = "Category is required")]
    public string Category { get; set; } = "Documents";
    
    public string? Description { get; set; }
    public string? Tags { get; set; }
}