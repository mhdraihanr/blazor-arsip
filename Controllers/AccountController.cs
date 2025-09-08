using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using blazor_arsip.Services;
using blazor_arsip.Models;

namespace blazor_arsip.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly blazor_arsip.Services.IAuthenticationService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(blazor_arsip.Services.IAuthenticationService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest request, [FromQuery] string? returnUrl)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {Email}", request.Email);

                var user = await _authService.AuthenticateAsync(request.Email, request.Password);
                
                // Normalize returnUrl
                var redirectUrl = "/dashboard";
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    redirectUrl = returnUrl;
                }

                if (user == null)
                {
                    _logger.LogWarning("Login failed for user: {Email}", request.Email);

                    // If this looks like a browser form post, redirect back to login with error
                    var acceptsHtml = Request.Headers["Accept"].Any(h => h.Contains("text/html", StringComparison.OrdinalIgnoreCase));
                    if (acceptsHtml || Request.HasFormContentType)
                    {
                        var backUrl = $"/login?error=1&returnUrl={Uri.EscapeDataString(redirectUrl)}";
                        return LocalRedirect(backUrl);
                    }

                    return BadRequest(new { message = "Invalid email or password" });
                }

                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("PhotoUrl", user.PhotoUrl ?? string.Empty)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CustomAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Sign in with cookie
                await HttpContext.SignInAsync("CustomAuth", claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                });

                _logger.LogInformation("User {Email} logged in successfully", user.Email);

                // If this looks like a browser form post, redirect to the returnUrl/dashboard so the browser stores the cookie
                var acceptsHtmlSuccess = Request.Headers["Accept"].Any(h => h.Contains("text/html", StringComparison.OrdinalIgnoreCase));
                if (acceptsHtmlSuccess || Request.HasFormContentType)
                {
                    return LocalRedirect(redirectUrl);
                }

                // Otherwise return JSON
                return Ok(new { success = true, redirect = redirectUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("Logout")]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userName = User.Identity?.Name ?? "Unknown";
                
                // Clear the authentication cookie
                await HttpContext.SignOutAsync("CustomAuth");
                
                // Clear session data
                HttpContext.Session.Clear();
                
                // Explicitly expire the cookie
                Response.Cookies.Delete("BlazorArsipAuth", new CookieOptions
                {
                    Path = "/",
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Strict
                });
                
                _logger.LogInformation("User {Name} logged out successfully", userName);
                
                // Always redirect to login after logout
                return LocalRedirect("/login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return LocalRedirect("/login");
            }
        }
    }
}
