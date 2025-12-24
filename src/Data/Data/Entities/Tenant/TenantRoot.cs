using Data.Entities.BaseEntity;

namespace Data.Entities.Tenant;

public class Tenant : ApplicationBaseEntity
{
    public string Slug { get; set; } = default!;         // unique
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public CompanyBranding Branding { get; set; } = default!;
    public string? UnderwriterDigitalSignatureUrl { get; set; }
    public string? UnderwriterName { get; set; }
}


