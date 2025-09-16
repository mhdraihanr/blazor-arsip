using blazor_arsip.Data;
using blazor_arsip.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace blazor_arsip.Services;

public interface IAuth0UserSyncService
{
    Task<User> SyncUserAsync(ClaimsPrincipal claimsPrincipal);
    Task<User?> FindUserByAuth0IdAsync(string auth0Id);
    Task<User?> FindUserByEmailAsync(string email);
    Task UpdateUserLoginAsync(User user);
}

public class Auth0UserSyncService : IAuth0UserSyncService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<Auth0UserSyncService> _logger;

    public Auth0UserSyncService(ApplicationDbContext context, ILogger<Auth0UserSyncService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User> SyncUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        try
        {
            var auth0Id = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
            var name = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? 
                      claimsPrincipal.FindFirst("name")?.Value ?? 
                      email?.Split('@')[0] ?? "Unknown User";
            var picture = claimsPrincipal.FindFirst("picture")?.Value;

            if (string.IsNullOrEmpty(auth0Id) || string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Auth0 user must have ID and email");
            }

            _logger.LogInformation("Syncing Auth0 user: {Auth0Id}, Email: {Email}", auth0Id, email);

            // First, try to find user by Auth0 ID
            var user = await FindUserByAuth0IdAsync(auth0Id);
            
            if (user == null)
            {
                // If not found by Auth0 ID, try to find by email (existing database user)
                user = await FindUserByEmailAsync(email);
                
                if (user != null)
                {
                    // Update existing user with Auth0 ID
                    _logger.LogInformation("Linking existing user {Email} with Auth0 ID {Auth0Id}", email, auth0Id);
                    user.Auth0Id = auth0Id;
                    user.Name = name; // Update name from Auth0
                    if (!string.IsNullOrEmpty(picture))
                    {
                        user.PhotoUrl = picture;
                    }
                }
                else
                {
                    // Create new user from Auth0
                    _logger.LogInformation("Creating new user from Auth0: {Email}", email);
                    user = new User
                    {
                        Auth0Id = auth0Id,
                        Email = email,
                        Name = name,
                        PhotoUrl = picture,
                        PasswordHash = string.Empty, // Auth0 users don't need local password
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Users.Add(user);
                }
            }
            else
            {
                // Update existing Auth0 user info
                user.Name = name;
                if (!string.IsNullOrEmpty(picture))
                {
                    user.PhotoUrl = picture;
                }
            }

            await UpdateUserLoginAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User sync completed for {Email}", email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing Auth0 user");
            throw;
        }
    }

    public async Task<User?> FindUserByAuth0IdAsync(string auth0Id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Auth0Id == auth0Id && u.IsActive);
    }

    public async Task<User?> FindUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    public async Task UpdateUserLoginAsync(User user)
    {
        user.LastLoginAt = DateTime.UtcNow;
        _context.Users.Update(user);
    }
}