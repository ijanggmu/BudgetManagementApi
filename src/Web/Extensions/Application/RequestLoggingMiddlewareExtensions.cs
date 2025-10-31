using Microsoft.AspNetCore.Builder;
using BeemaEdgeApi.Utilities.UserActivities;

namespace BeemaEdgeApi.Extensions.Application;

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        //builder.UseMiddleware<RouteAliasMiddleware>();

        builder.UseMiddleware<RequestLoggingMiddleware>();

        return builder;
    }
}
