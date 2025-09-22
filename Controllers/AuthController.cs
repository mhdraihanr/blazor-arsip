using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        blazor_arsip.Services.IAuthenticationService authService, 
        ILogger<AuthController> logger)
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
            var targetReturnUrl = returnUrl;
            if (string.IsNullOrEmpty(targetReturnUrl) || !Url.IsLocalUrl(targetReturnUrl))
            {
                targetReturnUrl = "/dashboard";
            }

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Auth0Callback"),
                Items = { ["returnUrl"] = targetReturnUrl }
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
        public IActionResult Auth0Callback()
        {
            _logger.LogInformation("Auth0 callback fallback endpoint hit");

            if (User.Identity?.IsAuthenticated == true)
            {
                var returnUrl = HttpContext.Request.Query["returnUrl"].FirstOrDefault();

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect("/dashboard");
                }

                if (!Url.IsLocalUrl(returnUrl))
                {
                    return Redirect("/dashboard");
                }

                return Redirect(returnUrl);
            }

            return Redirect("/login");
        }
    }
}
