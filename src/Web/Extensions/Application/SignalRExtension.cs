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
        //services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        //{
        //    options.Events = new JwtBearerEvents
        //    {
        //        OnMessageReceived = context =>
        //        {
        //            // Support token in query string for SignalR WebSocket connections
        //            var accessToken = context.Request.Query["access_token"];
        //            var path = context.HttpContext.Request.Path;

        //            // Only apply to SignalR hub endpoints
        //            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
        //            {
        //                context.Token = accessToken;
        //            }
        //            // Also check Authorization header (standard JWT flow)
        //            else if (string.IsNullOrEmpty(context.Token))
        //            {
        //                var authHeader = context.Request.Headers["Authorization"].ToString();
        //                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        //                {
        //                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
        //                }
        //            }

        //            return Task.CompletedTask;
        //        }
        //    };
        //});

        return services;
    }
}

