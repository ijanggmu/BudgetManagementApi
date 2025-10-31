using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BeemaEdgeApi.Utilities.ResponseWrapper;

namespace BeemaEdgeApi.Extensions.Application;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        LoggingUserObject userLoggedIn;

        using (var scope = _serviceProvider.CreateScope())
        {
            var userProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileService>();
            userLoggedIn = userProfileService.GetUser();
        }

        _logger.LogError("Exception occurred: {@Exception}, Type: {@Type}, UserName: {@UserName}, UserId: {@UserId}, Ip: {@Ip}",
            exception,
            "API",
            userLoggedIn.Username,
            userLoggedIn.UserId,
            userLoggedIn.IpAddress);

        httpContext.Response.ContentType = "application/json";
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        string errorMessage;
        int statusCode;

        if (exception.Message.Contains("body too large.", StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = "The request body is too large.";
            statusCode = (int)HttpStatusCode.RequestEntityTooLarge;
        }
        else
        {
            statusCode = (int)HttpStatusCode.InternalServerError;
            errorMessage = "An internal server error occurred.";
        }

        httpContext.Response.StatusCode = statusCode;
        var response = ErrorApiResponse.WrapError(errorMessage, statusCode);
        var json = JsonSerializer.Serialize(response, options);
        await httpContext.Response.WriteAsync(json, cancellationToken);
        return true;
    }
}
