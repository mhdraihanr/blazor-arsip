using blazor_arsip.Components;
using blazor_arsip.Data;
using blazor_arsip.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.IISIntegration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Note: Settings functionality will use custom User model for now

// Add custom services
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddScoped<IFileUploadService, FileUploadService>();

builder.Services.AddScoped<IToastService, ToastService>();

// Tambah CurrentUserService untuk menyediakan data user saat ini
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Add Auth0 User Sync Service
builder.Services.AddScoped<IAuth0UserSyncService, Auth0UserSyncService>();

// Add IP Address Service
builder.Services.AddScoped<IIpAddressService, IpAddressService>();

// Add User Settings Service
builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();

// Add HttpContextAccessor for authentication
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "BlazorArsipSession";
});

// Authentication & Authorization (Cookie-based with Auth0 Google integration)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "BlazorArsipAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/login";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    // Disable automatic redirects for API calls
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
})
.AddOpenIdConnect("Auth0", options =>
{
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.CallbackPath = "/signin-auth0";
    options.ClaimsIssuer = "Auth0";
    options.SaveTokens = false;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.UsePkce = true;
    options.SkipUnrecognizedRequests = true;
    
    // Fix state parameter and callback issues
    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProvider = context =>
        {
            // Add connection parameter for Google login
            context.ProtocolMessage.SetParameter("connection", "google-oauth2");
            
            // Ensure state parameter is properly set
            if (string.IsNullOrEmpty(context.ProtocolMessage.State))
            {
                context.ProtocolMessage.State = context.Options.StateDataFormat.Protect(context.Properties);
            }
            
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            
            // Log as information rather than error for timing issues
            logger.LogInformation("Auth0 middleware authentication event: {Error}", context.Exception?.Message);
            
            // Don't redirect automatically - let callback handle all redirects
            // This prevents double error handling
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // Token validation successful
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Auth0 token validated successfully");
            return Task.CompletedTask;
        },
        OnRemoteFailure = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Auth0 remote failure event: {Error}", context.Failure?.Message);
            
            // Check if user cancelled the authentication
            if (context.Failure?.Message?.Contains("access_denied") == true)
            {
                logger.LogInformation("User cancelled authentication");
                context.Response.Redirect("/login");
            }
            else
            {
                // Let the callback handle other errors
                logger.LogInformation("Allowing callback to handle the error");
                context.Response.Redirect("/api/auth/auth0-callback");
            }
            context.HandleResponse();
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorizationCore();
// Use standard server-side authentication
builder.Services.AddCascadingAuthenticationState();

// Add controllers
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:5264", "https://localhost:5265")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});



// Add file upload configuration
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 100 * 1024 * 1024; // 100MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// Add security headers
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.SuppressXFrameOptionsHeader = false;
});

// Configure forwarded headers for IP address detection
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Clear known networks and proxies to allow any
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Use forwarded headers for IP detection
app.UseForwardedHeaders();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Antiforgery middleware harus berada setelah UseRouting dan sebelum pemetaan endpoint
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
