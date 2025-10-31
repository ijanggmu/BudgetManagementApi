using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Utilities.ResponseWrapper;
using Infrastructure.Common.Khalti;
using Infrastructure.CoreApi.CoreApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;
using SharedKernel.CustomException;

namespace BeemaEdgeApi.Extensions.Refit;

public static class ApplicationRefitExtension
{
    public static IServiceCollection AddApplicationCoreApiRefitServices(this IServiceCollection services, IConfiguration config)
    {
        var apiSettings = config.GetSection("CoreApiOption");
        var khaltiSettings = config.GetSection("PaymentGatewayOptions:Khalti");

        services.AddTransient<RefitResponseHandler>();

        services.AddRefitClient<ICoreApiService>(new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            })
        })
        .ConfigureHttpClient((sp, client) =>
        {
            var baseUrl = apiSettings["BaseUrl"] ?? throw new ArgumentNullException(nameof(apiSettings), "CoreApi:BaseUrl is missing.");
            var apiKey = apiSettings["Key"] ?? throw new ArgumentNullException(nameof(apiSettings), "CoreApi:Key is missing.");

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("apiKey", apiKey);
        })
        .AddHttpMessageHandler<RefitResponseHandler>();

        services.AddRefitClient<IKhaltiApiService>(new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            })
        })
        .ConfigureHttpClient((sp, client) =>
        {
            var khaltiBaseUrl = khaltiSettings["BaseUrl"] ?? throw new ArgumentNullException(nameof(khaltiSettings), "Khalti:BaseUrl is missing.");
            var khaltiSecret = khaltiSettings["Secrets"] ?? throw new ArgumentNullException(nameof(khaltiSettings), "Khalti:Secrets is missing.");

            client.BaseAddress = new Uri(khaltiBaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"key {khaltiSecret}");
        });

        return services;
    }
}

public class RefitResponseHandler : DelegatingHandler
{
    private readonly ILogger<RefitResponseHandler> _logger;

    public RefitResponseHandler(ILogger<RefitResponseHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestBody = request.Content != null ? await request.Content.ReadAsStringAsync(cancellationToken) : null;

        _logger.LogInformation("""
            Sending HTTP Request:
            Method: {Method}
            URI: {Uri}
            Headers: {Headers}
            Body: {Body}
            """,
            request.Method,
            request.RequestUri,
            request.Headers.ToString(),
            requestBody
        );

        var response = await base.SendAsync(request, cancellationToken);

        var responseBody = response.Content != null ? await response.Content.ReadAsStringAsync(cancellationToken) : null;

        _logger.LogInformation("""
            Received HTTP Response:
            StatusCode: {StatusCode}
            URI: {Uri}
            Headers: {Headers}
            Body: {Body}
            """,
            response.StatusCode,
            request.RequestUri,
            response.Headers.ToString(),
            responseBody
        );

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(response, request);
        }
        return response;
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response, HttpRequestMessage request)
    {
        var content = await response.Content.ReadAsStringAsync();

        _logger.LogError("Received error response from {Uri} with status {StatusCode}. Content: {Content}",
            request.RequestUri, response.StatusCode, content);

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new RefitException($"Empty error response received. Reason: {response.ReasonPhrase}", (int)response.StatusCode);
        }
        throw new RefitException(content, (int)response.StatusCode);
    }
}

