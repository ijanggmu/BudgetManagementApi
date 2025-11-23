using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeemaEdgeApi.Extensions.SecurityHeaders;

public static class Extensions
{
    internal static IServiceCollection AddSecurityHeaders(this IServiceCollection services, IConfiguration config, IWebHostEnvironment environment)
    {
        // Security headers are applied via middleware, no service registration needed
        return services;
    }

    internal static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, IConfiguration config, IWebHostEnvironment environment)
    {
        app.Use(async (context, next) =>
        {
            // Strict Transport Security (HSTS) - only for HTTPS in production
            if (!environment.IsDevelopment() && context.Request.IsHttps)
            {
                context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
            }

            // Prevent clickjacking
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // Prevent MIME type sniffing
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // XSS Protection (legacy, but still useful for older browsers)
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Referrer Policy
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // Permissions Policy (formerly Feature-Policy)
            context.Response.Headers.Append("Permissions-Policy",
                "geolocation=(), microphone=(), camera=(), payment=(), usb=(), magnetometer=(), gyroscope=(), speaker=()");

            // Content Security Policy
            var csp = "default-src 'self'; " +
                      "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " + // Note: unsafe-inline/eval should be removed in production if possible
                      "style-src 'self' 'unsafe-inline'; " +
                      "img-src 'self' data: https:; " +
                      "font-src 'self' data:; " +
                      "connect-src 'self' https:; " +
                      "frame-ancestors 'none'; " +
                      "base-uri 'self'; " +
                      "form-action 'self';";

            context.Response.Headers.Append("Content-Security-Policy", csp);

            await next();
        });

        return app;
    }
}

