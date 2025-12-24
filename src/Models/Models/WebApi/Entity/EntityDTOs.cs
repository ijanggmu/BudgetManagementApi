namespace Models.BeemaEdgeApi.Entity;

public class EntitySettingsResponseDto
{
    public string TenantId { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string? UnderwriterDigitalSignatureUrl { get; set; }
    public string? UnderwriterName { get; set; }
}

public class UpdateEntitySettingsDto
{
    public string? UnderwriterDigitalSignatureUrl { get; set; }
    public string? UnderwriterName { get; set; }
}

