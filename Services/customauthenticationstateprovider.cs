using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;

namespace blazor_arsip.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CustomAuthenticationStateProvider> _logger;

    public CustomAuthenticationStateProvider(
        IHttpContextAccessor httpContextAccessor,
        ILogger<CustomAuthenticationStateProvider> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("CustomAuth - User is authenticated: {UserName}, Email: {Email}", 
                    httpContext.User.Identity.Name,
                    httpContext.User.FindFirst(ClaimTypes.Email)?.Value);
                return Task.FromResult(new AuthenticationState(httpContext.User));
            }
            
            _logger.LogInformation("CustomAuth - User is not authenticated");
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication state");
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }
    }

    // Force refresh authentication state
    public Task InitializeAsync()
    {
        // No initialization needed for cookie authentication
        return Task.CompletedTask;
    }

    public Task UpdateAuthenticationState(UserSession? userSession)
    {
        // Force refresh the authentication state
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return Task.CompletedTask;
    }
    
    public void RefreshAuthenticationState()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        try
        {
            _logger.LogInformation("Clearing authentication state for logout");
            // Force refresh the authentication state to check current status
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            
            // Additional delay to ensure state propagation
            await Task.Delay(50);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout state change");
        }
    }
}

public class UserSession
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}