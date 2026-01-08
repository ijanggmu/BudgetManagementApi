using Data.Entities.BaseEntity;
using Data.Entities.Tenant;

namespace Data.Entities.Tenant;

/// <summary>
/// SMS Gateway Configuration entity for tenant-wise SMS provider settings
/// </summary>
public class SmsGatewayConfiguration : TenantEntity
{
    /// <summary>
    /// Provider name (e.g., "SparrowSMS", "Twilio", "AWS SNS", "Nexmo")
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;

    /// <summary>
    /// API endpoint URL
    /// </summary>
    public string ApiUrl { get; set; } = string.Empty;

    /// <summary>
    /// API key or token (encrypted)
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// API secret (encrypted, if required)
    /// </summary>
    public string? ApiSecret { get; set; }

    /// <summary>
    /// Sender ID or From number
    /// </summary>
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// Whether this configuration is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Additional settings as JSON (for provider-specific configurations)
    /// </summary>
    public string? AdditionalSettings { get; set; }
}

