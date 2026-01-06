namespace Models.BeemaEdgeApi.Entity;

public class EntitySettingsResponseDto
{
    public string TenantId { get; set; } = string.Empty;
    public string PaletteJson { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string LogoSignedUrl { get; set; } = string.Empty;
    public string UnderwriterDigitalSignatureUrl { get; set; }
    public string UnderwriterDigitalSignatureSignedUrl { get; set; }
    public string UnderwriterName { get; set; }
}

public class UpdateEntitySettingsDto
{
    public string UnderwriterDigitalSignatureUrl { get; set; }
    public string UnderwriterName { get; set; }
}


