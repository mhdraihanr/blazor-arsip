using System.ComponentModel.DataAnnotations;

namespace blazor_arsip.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "Email wajib diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password wajib diisi")]
    [MinLength(6, ErrorMessage = "Password minimal 6 karakter")]
    public string Password { get; set; } = string.Empty;
}