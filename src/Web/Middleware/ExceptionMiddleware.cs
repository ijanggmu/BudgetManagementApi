using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BeemaEdgeApi.Utilities.ResponseWrapper;

namespace BeemaEdgeApi.Middleware;

public class ExceptionMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleWare> _logger;

    public ExceptionMiddleWare(RequestDelegate next,
        ILogger<ExceptionMiddleWare> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var userLoggedIn = new LoggingUserObject();
            using (var scope = serviceProvider.CreateScope())
            {
                var userProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileService>();
                userLoggedIn = userProfileService.GetUser();
            }

            _logger.LogError("Type: {@type}, InformationType: {@informationType}, Message: {@message}, UserName: {@userName}, UserId: {@userId}, Ip: {@ip}, Exception: {@Exception}",
                            "API", "Error", ex.Message, userLoggedIn.Username, userLoggedIn.UserId, userLoggedIn.IpAddress, ex);

            context.Response.ContentType = "application/json";
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            string errorMessage;
            int statusCode;

            if (ex.Message.Contains("body too large."))
            {
                errorMessage = ex.Message;
                statusCode = (int)HttpStatusCode.RequestEntityTooLarge;
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                errorMessage = ex.Message;
            }

            context.Response.StatusCode = statusCode;
            var response = ErrorApiResponse.WrapError(errorMessage, statusCode);
            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}

