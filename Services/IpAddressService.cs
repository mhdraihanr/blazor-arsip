using System.Net;

namespace blazor_arsip.Services;

public interface IIpAddressService
{
    string GetClientIpAddress();
    string GetUserAgent();
}

public class IpAddressService : IIpAddressService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<IpAddressService> _logger;

    public IpAddressService(IHttpContextAccessor httpContextAccessor, ILogger<IpAddressService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string GetClientIpAddress()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                _logger.LogWarning("HttpContext is null, cannot get IP address");
                return "Unknown";
            }

            _logger.LogDebug("Getting client IP address...");

            // Check for forwarded headers first (in case of reverse proxy/load balancer)
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // X-Forwarded-For can contain multiple IPs, take the first one
                var firstIp = forwardedFor.Split(',')[0].Trim();
                if (IPAddress.TryParse(firstIp, out _))
                {
                    _logger.LogInformation($"IP from X-Forwarded-For: {firstIp}");
                    return firstIp;
                }
            }

            // Check for X-Real-IP header
            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp) && IPAddress.TryParse(realIp, out _))
            {
                _logger.LogInformation($"IP from X-Real-IP: {realIp}");
                return realIp;
            }

            // Check for CF-Connecting-IP (Cloudflare)
            var cfIp = httpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(cfIp) && IPAddress.TryParse(cfIp, out _))
            {
                _logger.LogInformation($"IP from CF-Connecting-IP: {cfIp}");
                return cfIp;
            }

            // Fall back to Connection.RemoteIpAddress
            var remoteIp = httpContext.Connection.RemoteIpAddress;
            if (remoteIp != null)
            {
                // Handle IPv6 loopback and map to IPv4 if needed
                if (remoteIp.IsIPv4MappedToIPv6)
                {
                    remoteIp = remoteIp.MapToIPv4();
                }

                var ipString = remoteIp.ToString();
                
                // Convert IPv6 loopback to IPv4 loopback for consistency
                if (ipString == "::1")
                {
                    ipString = "127.0.0.1";
                }

                _logger.LogInformation($"IP from Connection.RemoteIpAddress: {ipString}");
                return ipString;
            }

            _logger.LogWarning("Could not determine client IP address from any source");
            return "127.0.0.1"; // Default to localhost for development
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client IP address");
            return "127.0.0.1"; // Default to localhost for development
        }
    }

    public string GetUserAgent()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return "Unknown";
            }

            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            return !string.IsNullOrEmpty(userAgent) ? userAgent : "Unknown";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user agent");
            return "Unknown";
        }
    }
}