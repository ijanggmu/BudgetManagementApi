using System.Net.Http.Headers;
using Data.Entities.Log;
using Hangfire;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Business.Common.Sms;
public class SmsOptions
{
    public string from { get; set; }
    public string token { get; set; }
    public string url { get; set; }

}
public class SmsService : ISmsService
{
    private readonly SmsOptions _smsServerConfig;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SmsService> _logger;
    private readonly ISmsLogService _smsLogService;

    public SmsService(IOptions<SmsOptions> smsServerConfig,
        IHttpClientFactory httpClientFactory,
        ILogger<SmsService> logger,
        ISmsLogService smsLogService)
    {
        _smsServerConfig = smsServerConfig.Value;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _smsLogService = smsLogService;
    }
    public void QueueSms(SmsRequest mailRequest)
    {
        BackgroundJobClient jobClient = new BackgroundJobClient();
        IState state = new EnqueuedState("sms");
        jobClient.Create(() => SendSmsAsync(mailRequest), state);
    }


    public async Task SendSmsAsync(SmsRequest request)
    {
        if (request.To == null || request.To.Count == 0)
        {
            _logger.LogWarning("SMS request has no recipients.");
            return;
        }

        var phoneNumbers = string.Join(",", request.To);

        var smsLog = new SmsLog
        {
            To = phoneNumbers,
            Body = request.Body,
            From = _smsServerConfig.from,
            SmsType = request.SmsType,
            SentDate = DateTime.UtcNow,
        };

        try
        {
            var apiUrl = _smsServerConfig.url;
            var payload = new
            {
                _smsServerConfig.from,
                to = phoneNumbers,
                _smsServerConfig.token,
                text = request.Body
            };

            var dataAsString = JsonConvert.SerializeObject(payload);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            _logger.LogInformation("Sending SMS to {PhoneNumbers}", phoneNumbers);

            var client = ApiConfiguration();
            var response = await client.PostAsync(apiUrl, content);
            response.EnsureSuccessStatusCode();
            smsLog.IsSuccess = true;


        }
        catch (Exception ex)
        {
            smsLog.IsSuccess = false;
            smsLog.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error sending SMS to {PhoneNumbers}", phoneNumbers);
            throw;
        }
        finally
        {
            if (request.SensitiveData != null && request.SensitiveData.Count > 0)
            {
                foreach (var data in request.SensitiveData)
                {
                    smsLog.Body = request.Body.Replace(data, "****");
                }
            }

            await _smsLogService.LogSmsAsync(smsLog);
        }
    }

    private HttpClient ApiConfiguration()
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }
}
