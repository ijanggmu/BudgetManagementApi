namespace Models.WebApi.Gateway;

// Email Gateway DTOs
public record CreateEmailGatewayDto(
    string ProviderName,
    string Host,
    int Port,
    string UserName,
    string Password,
    string FromEmail,
    string DisplayName,
    bool EnableSsl = true,
    bool IsActive = true,
    string? AdditionalSettings = null
);

public record UpdateEmailGatewayDto(
    string? ProviderName,
    string? Host,
    int? Port,
    string? UserName,
    string? Password,
    string? FromEmail,
    string? DisplayName,
    bool? EnableSsl,
    bool? IsActive,
    string? AdditionalSettings
);

public record EmailGatewayResponseDto(
    string Id,
    string TenantId,
    string ProviderName,
    string Host,
    int Port,
    string FromEmail,
    string DisplayName,
    bool EnableSsl,
    bool IsActive,
    string? AdditionalSettings,
    DateTime CreatedOn,
    DateTime? LastModifiedOn
);

// SMS Gateway DTOs
public record CreateSmsGatewayDto(
    string ProviderName,
    string ApiUrl,
    string ApiKey,
    string? ApiSecret,
    string From,
    bool IsActive = true,
    string? AdditionalSettings = null
);

public record UpdateSmsGatewayDto(
    string? ProviderName,
    string? ApiUrl,
    string? ApiKey,
    string? ApiSecret,
    string? From,
    bool? IsActive,
    string? AdditionalSettings
);

public record SmsGatewayResponseDto(
    string Id,
    string TenantId,
    string ProviderName,
    string ApiUrl,
    string From,
    bool IsActive,
    string? AdditionalSettings,
    DateTime CreatedOn,
    DateTime? LastModifiedOn
);

