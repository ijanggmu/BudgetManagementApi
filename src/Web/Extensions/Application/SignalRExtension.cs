using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace BeemaEdgeApi.Extensions.Application;

public static class SignalRExtension
{
    /// <summary>
    /// Adds SignalR services with JWT authentication support
    /// </summary>
    public static IServiceCollection AddSignalRExtension(this IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            // Enable detailed errors for debugging (disable in production)
            options.EnableDetailedErrors = true;
        });

        // Configure JWT authentication for SignalR connections
        // SignalR can receive tokens via query string or header
        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            // Extend existing JWT events to support SignalR token extraction
            var existingOnMessageReceived = options.Events?.OnMessageReceived;
            
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // First, run existing OnMessageReceived handler (if any)
                    if (existingOnMessageReceived != null)
                    {
                        existingOnMessageReceived(context);
                    }

                    // Support token in query string for SignalR WebSocket connections
                    // This is needed because WebSocket connections cannot set custom headers
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    // Only apply to SignalR hub endpoints
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    // Also check Authorization header (standard JWT flow) if token not already set
                    else if (string.IsNullOrEmpty(context.Token))
                    {
                        var authHeader = context.Request.Headers["Authorization"].ToString();
                        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = authHeader.Substring("Bearer ".Length).Trim();
                        }
                    }
                    // Also check cookies (for web clients)
                    else if (string.IsNullOrEmpty(context.Token) && context.Request.Cookies.ContainsKey("X-Access-Token"))
                    {
                        var cookieValue = context.Request.Cookies["X-Access-Token"];
                        if (!string.IsNullOrEmpty(cookieValue))
                        {
                            context.Token = System.Web.HttpUtility.UrlDecode(cookieValue);
                        }
                    }

                    return Task.CompletedTask;
                },
                OnChallenge = options.Events?.OnChallenge,
                OnAuthenticationFailed = options.Events?.OnAuthenticationFailed,
                OnTokenValidated = options.Events?.OnTokenValidated,
                OnForbidden = options.Events?.OnForbidden
            };
        });

        return services;
    }
}

