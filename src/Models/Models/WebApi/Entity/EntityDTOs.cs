namespace Models.BeemaEdgeApi.Entity;

public class EntitySettingsResponseDto
{
    public string TenantId { get; set; } = string.Empty;
    public string PaletteJson { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string LogoSignedUrl { get; set; } = string.Empty;
    public string UnderwriterName { get; set; }
    public string PanNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string CompanyStampUrl { get; set; }
    public string CompanyStampSignedUrl { get; set; }
}

public class UpdateEntitySettingsDto
{
    public string PanNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string CompanyStampUrl { get; set; }
}



