using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Config;

namespace Business.Common.Hmac;

public class HmacValidatorService : IHmacValidatorService
{
    private readonly HmacAuthSettings _settings;
    private readonly ILogger<HmacValidatorService> _logger;

    public HmacValidatorService(IOptions<HmacAuthSettings> options, ILogger<HmacValidatorService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<bool> IsValidAsync(HttpRequest request)
    {
        if (!request.Headers.TryGetValue("X-Api-Key", out var apiKey) ||
            !request.Headers.TryGetValue("X-Timestamp", out var timestampStr) ||
            !request.Headers.TryGetValue("X-Signature", out var signature))
        {
            _logger.LogWarning("Missing HMAC headers");
            return false;
        }

        if (!long.TryParse(timestampStr, out var timestamp) ||
            Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timestamp) > _settings.AllowedClockSkewMinutes * 60)
        {
            _logger.LogWarning("Timestamp out of range");
            return false;
        }

        if (!string.Equals(apiKey, _settings.ApiKey))
        {
            _logger.LogWarning("Invalid API Key");
            return false;
        }
        return true;
    }

    private string ComputeHmacSignature(string apiKey, string timestamp, string body)
    {
        body ??= string.Empty;
        var message = string.Concat(apiKey, ":", timestamp, ":", body);
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.Secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return Convert.ToBase64String(hash);
    }

    private static bool SlowEquals(string a, string b)
    {
        var aBytes = Encoding.UTF8.GetBytes(a);
        var bBytes = Encoding.UTF8.GetBytes(b);
        uint diff = (uint)aBytes.Length ^ (uint)bBytes.Length;
        for (int i = 0; i < aBytes.Length && i < bBytes.Length; i++)
            diff |= (uint)(aBytes[i] ^ bBytes[i]);
        return diff == 0;
    }
}

