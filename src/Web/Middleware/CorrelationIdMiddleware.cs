using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharedKernel.Constant;
namespace BeemaEdgeApi.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestTimestamp = DateTime.UtcNow;
        // Check for an existing correlation ID in the request headers
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();

        // If no correlation ID exists, generate a new one
        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // Store the correlation ID in HttpContext.Items
        context.Items[SystemConstant.CorrelationId] = correlationId;
        context.Items[SystemConstant.RequestTimeStamp] = requestTimestamp;

        // Optionally, add it to the response headers
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append(SystemConstant.CorrelationId, correlationId);
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
