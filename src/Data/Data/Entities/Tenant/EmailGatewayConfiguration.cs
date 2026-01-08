using Data.Entities.BaseEntity;
using Data.Entities.Tenant;

namespace Data.Entities.Tenant;

/// <summary>
/// Email Gateway Configuration entity for tenant-wise email provider settings
/// </summary>
public class EmailGatewayConfiguration : TenantEntity
{
    /// <summary>
    /// Provider name (e.g., "SMTP", "SendGrid", "Mailgun", "AWS SES")
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;

    /// <summary>
    /// SMTP Host or API endpoint
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// SMTP Port (e.g., 25, 465, 587)
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Username or API key (encrypted)
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Password or API secret (encrypted)
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// From email address
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Display name for the sender
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Whether to use SSL/TLS
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Whether this configuration is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Additional settings as JSON (for provider-specific configurations)
    /// </summary>
    public string? AdditionalSettings { get; set; }
}

