using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using blazor_arsip.Services;
using blazor_arsip.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace blazor_arsip.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
    private readonly blazor_arsip.Services.IAuthenticationService _authService;
    private readonly IAuth0UserSyncService _auth0UserSyncService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        blazor_arsip.Services.IAuthenticationService authService, 
        IAuth0UserSyncService auth0UserSyncService,
        ILogger<AuthController> logger)
        {
            _authService = authService;
            _auth0UserSyncService = auth0UserSyncService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {Email}", request.Email);

                var user = await _authService.AuthenticateAsync(request.Email, request.Password);
                
                if (user == null)
                {
                    _logger.LogWarning("Login failed for user: {Email}", request.Email);
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("PhotoUrl", user.PhotoUrl ?? string.Empty)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Sign in with cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                });

                _logger.LogInformation("User {Email} logged in successfully", user.Email);

                return Ok(new 
                { 
                    success = true, 
                    user = new 
                    { 
                        user.Id, 
                        user.Name, 
                        user.Email, 
                        user.PhotoUrl 
                    } 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Get user info before signing out
                var userName = User.Identity?.Name ?? "Unknown";
                
                // Clear the authentication cookie
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                // Explicitly expire the cookie
                Response.Cookies.Delete("BlazorArsipAuth", new CookieOptions
                {
                    Path = "/",
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Strict
                });
                
                _logger.LogInformation("User {Name} logged out successfully", userName);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        [HttpGet("check")]
        [Authorize]
        public IActionResult CheckAuth()
        {
            return Ok(new 
            { 
                isAuthenticated = true,
                user = new 
                {
                    name = User.Identity?.Name,
                    email = User.FindFirst(ClaimTypes.Email)?.Value
                }
            });
        }

        [HttpGet("login-auth0")]
        public IActionResult LoginAuth0(string? returnUrl = null, string? connection = null)
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Auth0Callback"),
                Items = { ["returnUrl"] = returnUrl ?? "/dashboard" }
            };

            if (!string.IsNullOrEmpty(connection))
            {
                authenticationProperties.Items["connection"] = connection;
            }

            return Challenge(authenticationProperties, "Auth0");
        }

        [HttpGet("login-google")]
        public IActionResult LoginGoogle(string? returnUrl = null)
        {
            return LoginAuth0(returnUrl, "google-oauth2");
        }

        [HttpGet("auth0-callback")]
        public async Task<IActionResult> Auth0Callback()
        {
            try
            {
                _logger.LogInformation("Auth0 callback initiated");
                
                // Check if user is already authenticated
                if (User.Identity?.IsAuthenticated == true)
                {
                    _logger.LogInformation("User already authenticated, redirecting to dashboard");
                    return Redirect("/dashboard");
                }
                
                // Get the result from the Auth0 authentication
                var result = await HttpContext.AuthenticateAsync("Auth0");
                
                if (!result.Succeeded)
                {
                    var errorMessage = result.Failure?.Message ?? "Unknown error";
                    
                    // Log the specific error for debugging but don't always treat as failure
                    _logger.LogInformation("Auth0 authentication result not successful: {Error}", errorMessage);
                    
                    // Check if this is a recoverable error or user cancellation
                    if (errorMessage.Contains("access_denied") || errorMessage.Contains("user_cancelled"))
                    {
                        _logger.LogInformation("User cancelled authentication, redirecting to login");
                        return Redirect("/login");
                    }
                    
                    // For state or timing issues, try to check if we have valid claims anyway
                    if (HttpContext.User?.Identity?.IsAuthenticated == true)
                    {
                        _logger.LogInformation("Found valid authentication despite callback failure, proceeding");
                        // User is actually authenticated, redirect to dashboard
                        return Redirect("/dashboard");
                    }
                    
                    // Only show error for genuine authentication failures
                    _logger.LogWarning("Genuine Auth0 authentication failure: {Error}", errorMessage);
                    return Redirect("/login?error=auth0_failed");
                }

                _logger.LogInformation("Auth0 authentication succeeded, syncing user");
                
                // Sync user with database
                var user = await _auth0UserSyncService.SyncUserAsync(result.Principal);
                
                _logger.LogInformation("User synced successfully: {Email}", user.Email);
                
                // Create local cookie with user information
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("PhotoUrl", user.PhotoUrl ?? string.Empty)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                });

                _logger.LogInformation("Local cookie created for user {Email}", user.Email);

                // Get return URL from the original request or default to dashboard
                var returnUrl = HttpContext.Request.Query["returnUrl"].FirstOrDefault();
                if (string.IsNullOrEmpty(returnUrl) && result.Properties?.Items != null)
                {
                    result.Properties.Items.TryGetValue("returnUrl", out returnUrl);
                }
                
                returnUrl = returnUrl ?? "/dashboard";
                
                // Ensure we redirect to a safe URL
                if (!Url.IsLocalUrl(returnUrl))
                {
                    returnUrl = "/dashboard";
                }
                
                _logger.LogInformation("Redirecting authenticated user to: {ReturnUrl}", returnUrl);
                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Auth0 callback");
                
                // Check if user is actually authenticated despite the exception
                if (HttpContext.User?.Identity?.IsAuthenticated == true)
                {
                    _logger.LogInformation("User authenticated despite callback exception, redirecting to dashboard");
                    return Redirect("/dashboard");
                }
                
                return Redirect("/login?error=auth0_error");
            }
        }
    }
}
