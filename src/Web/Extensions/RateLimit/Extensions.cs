using System;
using System.Linq;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BeemaEdgeApi.Extensions.RateLimit
{
    public static class RateLimitExtensions
    {
        internal static IServiceCollection ConfigureRateLimit(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RateLimitOptions>(config.GetSection(nameof(RateLimitOptions)));

            var options = config.GetSection(nameof(RateLimitOptions)).Get<RateLimitOptions>();
            if (options?.EnableRateLimiting == true)
            {
                services.AddRateLimiter(rateLimitOptions =>
                {
                    var limiter = CreateChainedRateLimiter(options);
                    rateLimitOptions.GlobalLimiter = limiter;

                    rateLimitOptions.RejectionStatusCode = options.RejectionStatusCode;
                    rateLimitOptions.OnRejected = async (context, token) =>
                    {
                        var message = BuildRateLimitResponseMessage(context);
                        await context.HttpContext.Response.WriteAsync(message, cancellationToken: token);
                    };
                });
            }

            return services;
        }

        internal static IApplicationBuilder UseRateLimit(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<RateLimitOptions>>().Value;

            if (options.EnableRateLimiting)
            {
                app.UseRateLimiter();
            }

            return app;
        }

        private static PartitionedRateLimiter<HttpContext> CreateChainedRateLimiter(RateLimitOptions options)
        {
            static string GetPartitionKey(HttpContext httpContext) =>
                httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();

            var minuteLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext => RateLimitPartition.GetFixedWindowLimiter(
                    GetPartitionKey(httpContext),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = options.PermitLimitInMinutes,
                        Window = TimeSpan.FromMinutes(options.WindowInMinutes)
                    }));

            var hourLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext => RateLimitPartition.GetFixedWindowLimiter(
                    GetPartitionKey(httpContext),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = options.PermitLimitInHours,
                        Window = TimeSpan.FromHours(options.WindowInHours)
                    }));

            return PartitionedRateLimiter.CreateChained(minuteLimiter, hourLimiter);
        }

        private static string BuildRateLimitResponseMessage(OnRejectedContext onRejectedContext)
        {
            if (onRejectedContext.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                return $"Too many requests. Please try again after {retryAfter.Minutes} minute(s).";
            else
                return $"Too many requests. Please try again later. ";
        }
    }
}
