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

        // Log full exception details for debugging (server-side only)
        _logger.LogError(exception, 
            "Exception occurred: Type: {@Type}, UserName: {@UserName}, UserId: {@UserId}, Ip: {@Ip}, Path: {@Path}",
            "API",
            userLoggedIn.Username,
            userLoggedIn.UserId,
            userLoggedIn.IpAddress,
            httpContext.Request.Path);

        httpContext.Response.ContentType = "application/json";
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        string errorMessage;
        int statusCode;
        int errorCode;

        // Handle specific exception types with appropriate error codes
        if (exception.Message.Contains("body too large.", StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = "The request body is too large.";
            statusCode = (int)HttpStatusCode.RequestEntityTooLarge;
            errorCode = 4131;
        }
        else if (exception is UnauthorizedAccessException)
        {
            errorMessage = "Unauthorized access.";
            statusCode = (int)HttpStatusCode.Unauthorized;
            errorCode = 4011;
        }
        else if (exception is ArgumentException || exception is ArgumentNullException)
        {
            errorMessage = "Invalid request parameters.";
            statusCode = (int)HttpStatusCode.BadRequest;
            errorCode = 4001;
        }
        else
        {
            statusCode = (int)HttpStatusCode.InternalServerError;
            errorMessage = "An internal server error occurred.";
            errorCode = 5001;
        }

        httpContext.Response.StatusCode = statusCode;
        var response = ErrorApiResponse.WrapError(errorMessage, errorCode);
        var json = JsonSerializer.Serialize(response, options);
        await httpContext.Response.WriteAsync(json, cancellationToken);
        return true;
    }
}
