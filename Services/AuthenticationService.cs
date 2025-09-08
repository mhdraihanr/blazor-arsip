using blazor_arsip.Models;
using blazor_arsip.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace blazor_arsip.Services;

public interface IAuthenticationService
{
    Task<User?> AuthenticateAsync(string email, string password);
    Task<bool> ValidatePasswordAsync(string password, string passwordHash);
    string HashPassword(string password);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _context;

    public AuthenticationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null)
            return null;

        if (!ValidatePassword(password, user.PasswordHash))
            return null;

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return user;
    }

    public Task<bool> ValidatePasswordAsync(string password, string passwordHash)
    {
        return Task.FromResult(ValidatePassword(password, passwordHash));
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool ValidatePassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}