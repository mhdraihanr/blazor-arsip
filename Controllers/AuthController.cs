using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using blazor_arsip.Services;
using blazor_arsip.Models;

namespace blazor_arsip.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
    private readonly blazor_arsip.Services.IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(blazor_arsip.Services.IAuthenticationService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
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

                var claimsIdentity = new ClaimsIdentity(claims, "CustomAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Sign in with cookie
                await HttpContext.SignInAsync("CustomAuth", claimsPrincipal, new AuthenticationProperties
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
                await HttpContext.SignOutAsync("CustomAuth");
                
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
    }
}
