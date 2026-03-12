using Data.Entities.BaseEntity;

namespace Data.Entities.Tenant;

public class Tenant : ApplicationBaseEntity
{
    public string Slug { get; set; } = default!;         // unique
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public CompanyBranding Branding { get; set; } = default!;
    /// <summary>PAN number of the organization.</summary>
    public string PanNumber { get; set; }
    /// <summary>Organization phone number.</summary>
    public string PhoneNumber { get; set; }
    /// <summary>Organization address.</summary>
    public string Address { get; set; }
    /// <summary>Company stamp image URL (used instead of signature).</summary>
    public string CompanyStampUrl { get; set; }
}


