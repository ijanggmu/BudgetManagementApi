using System;
using System.ComponentModel.DataAnnotations;
using Data.Entities.BaseEntity;

namespace Data.Entities.Tenant;

public class CompanyBranding : ApplicationBaseEntity
{
    public string TenantId { get; set; }                   // PK & FK
    public Tenant Tenant { get; set; }
    public string LogoUrl { get; set; } = default!;      // CDN/blob URL
    public string PaletteJson { get; set; } = "{}";      // AA contrast enforced
    public string TypographyJson { get; set; } = "{}";
    public int Version { get; set; } = 1;
}


