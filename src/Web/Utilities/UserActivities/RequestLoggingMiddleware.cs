using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using Data.Entities.Audit.UserActivites;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using SharedKernel.Constant;
using SharedKernel.Models.Tenancy;

namespace BeemaEdgeApi.Utilities.UserActivities;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    public async Task Invoke(HttpContext context, Channel<UserActivity> channel, ITenantContext tenantContext)
    {
        var routeAlias = context.Items["RouteAlias"] as string;
        var module = context.Items["Module"] as string;
        var visibletoUser = context.Items["VisibleToUserExceptAdmin"] as bool?;

        var requestActivity = new UserActivity
        {
            RequestPathAlias = routeAlias,
            VisibleToUserExceptAdmin = visibletoUser ?? true,
            Module = module
        };

        if (context.Request.ContentLength == null || context.Request.ContentLength < SystemConstant.MaxRequestBodySize)
        {
            await RecordRequestLogAsync();
        }

        var responseBodyStream = _recyclableMemoryStreamManager.GetStream();
        var previousBodyStream = context.Response.Body;
        context.Response.Body = responseBodyStream;

        await _next(context);
        responseBodyStream.Flush();
        responseBodyStream.Seek(0, SeekOrigin.Begin);

        using StreamReader reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        var responseJson = await reader.ReadToEndAsync();
        responseBodyStream.Seek(0, SeekOrigin.Begin);

        await responseBodyStream.CopyToAsync(previousBodyStream);
        context.Response.Body = previousBodyStream;
        requestActivity.ResponseStatusCode = context.Response.StatusCode;
        requestActivity.EndAt = DateTimeOffset.UtcNow;
        var responseTime = (requestActivity.EndAt - requestActivity.At).TotalMilliseconds;
        requestActivity.ResponseTime = responseTime;

        async ValueTask RecordRequestLogAsync()
        {
            context.Request.EnableBuffering();
            var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            context.Request.Body.Position = 0;

            var requestBody = string.Empty;

            if (IsFormContentType(context.Request.ContentType))
            {
                requestBody = ExtractFileNamesFromFormFiles(context.Request.Form.Files);
            }
            else
            {
                requestBody = ReadStreamInChunks(requestStream);
                requestBody = SanitizeValue(requestBody);
            }

            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            var request = context.Request;
            var startTime = (DateTime)context.Items[SystemConstant.RequestTimeStamp];
            requestActivity.RequestSchema = request.Scheme;
            requestActivity.RequestHost = request.Host.HasValue ? context.Request.Host.Value : null;
            requestActivity.RequestPath = request.Path;
            requestActivity.At = startTime;
            requestActivity.RequestHeader = SerializeHeaders(request.Headers);
            requestActivity.RequestBody = requestBody;
            requestActivity.RequestMethod = request.Method;
            requestActivity.RequestQueryString = request.QueryString.HasValue ? request.QueryString.Value : null;
            requestActivity.IpAddress = ip;
            requestActivity.UserId = context.User.Claims.Where(x => x.Type == TokenKey.UserId)?.FirstOrDefault()?.Value;
            requestActivity.UserName = context.User.Claims.Where(x => x.Type == ClaimTypes.Name)?.FirstOrDefault()?.Value;
            requestActivity.UserAgent = request.Headers["User-Agent"].ToString();
            requestActivity.CorrelationId = context.Items[SystemConstant.CorrelationId].ToString();
            requestActivity.TenantId = tenantContext?.TenantId;


        }

        //if (!requestActivity.RequestPath.ToLower().Contains("pdf") && !requestActivity.RequestPath.ToLower().Contains("export"))
            requestActivity.ResponseBody = responseJson;

        await channel.Writer.WriteAsync(requestActivity);
    }

    private static bool IsFormContentType(string contentType)
    {
        return !string.IsNullOrEmpty(contentType) && contentType.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase);
    }

    private static string ExtractFileNamesFromFormFiles(IFormFileCollection formFiles)
    {
        if (formFiles == null || formFiles.Count == 0)
        {
            return string.Empty;
        }

        var fileNames = formFiles.Select(file => file.FileName);

        return string.Join(", ", fileNames);
    }

    private static string ReadStreamInChunks(Stream stream)
    {
        const int readChunkBufferLength = 4096;
        stream.Seek(0, SeekOrigin.Begin);
        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);
        var readChunk = new char[readChunkBufferLength];
        int readChunkLength;
        do
        {
            readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);

        return textWriter.ToString();
    }
    private string SerializeHeaders(IHeaderDictionary headers)
    {
        return JsonSerializer.Serialize(headers);
    }

    private string SanitizeValue(string requestBody)
    {
        string[] fieldsToSanitize = {
                SystemConstant.PasswordField,
                SystemConstant.ConfirmPasswordField,
                SystemConstant.NewPasswordField,
                SystemConstant.OtpField,
                SystemConstant.PinField,
                SystemConstant.MPINField,
            };

        foreach (var field in fieldsToSanitize)
        {
            var pattern = $@"(""{field}"":\s*""[^""]+"")";
            var regex = new Regex(pattern);
            requestBody = regex.Replace(requestBody, $"\"{field}\": \"****\"");
        }

        return requestBody;
    }
}
