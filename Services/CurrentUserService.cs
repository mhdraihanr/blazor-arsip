using blazor_arsip.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace blazor_arsip.Services;

public interface ICurrentUserService
{
    Task<UserInfo?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    Task<UserInfo?> GetCurrentUserFromClaimsAsync(ClaimsPrincipal user);
}

public class CurrentUserService : ICurrentUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        AuthenticationStateProvider authenticationStateProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserInfo?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        // Try to get from HttpContext first (for server-side)
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            return await GetCurrentUserFromClaimsAsync(httpContext.User);
        }
        
        // Fallback to AuthenticationStateProvider (for interactive scenarios)
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return await GetCurrentUserFromClaimsAsync(authState.User);
    }

    public Task<UserInfo?> GetCurrentUserFromClaimsAsync(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            return Task.FromResult<UserInfo?>(null);

        var userInfo = new UserInfo
        {
            Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            Name = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            PhotoUrl = user.FindFirst("PhotoUrl")?.Value
        };

        return Task.FromResult<UserInfo?>(userInfo);
    }
}