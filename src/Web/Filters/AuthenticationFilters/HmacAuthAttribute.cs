using System;
using System.Linq;
using System.Threading.Tasks;
using Business.Common.Hmac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;

namespace BeemaEdgeApi.Filters.AuthenticationFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HmacAuthAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var request = httpContext.Request;

        var validator = context.HttpContext.RequestServices.GetRequiredService<IHmacValidatorService>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<HmacAuthAttribute>>();
        // Get client IP
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();

        // Optional: Behind proxy/load balancer (trust only if configured correctly)
        if (request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            clientIp = forwardedFor.ToString().Split(',').FirstOrDefault()?.Trim();
        }

        // Get origin and referer
        var origin = request.Headers["Origin"].FirstOrDefault();
        var referer = request.Headers["Referer"].FirstOrDefault();

        // Get user-agent
        var userAgent = request.Headers["User-Agent"].FirstOrDefault();
        if (!await validator.IsValidAsync(context.HttpContext.Request))
        {
            logger.LogWarning("HMAC validation failed for path: {Path}. IP: {IP}, Origin: {Origin}, Referer: {Referer}, User-Agent: {UserAgent}",
                            request.Path, clientIp, origin ?? "N/A", referer ?? "N/A", userAgent ?? "N/A");
            context.Result = new UnauthorizedObjectResult("HMAC validation failed.");
            return;
        }

        await next();
    }
}

