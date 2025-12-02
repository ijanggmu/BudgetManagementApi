using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Tenant;

public class PremiumCalculationConfiguration : TenantEntity
{
    [Required]
    [MaxLength(100)]
    public string PortfolioAlias { get; set; } = default!;

    [Required]
    [MaxLength(20)]
    public string FiscalYear { get; set; } = default!;

    public int Version { get; set; } = 1;

    [Required]
    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string CalculationEngineType { get; set; } = "FormulaBased"; // FormulaBased, RuleBased

    // Navigation properties
    public virtual ICollection<PremiumCalculationParameter> Parameters { get; set; } = new List<PremiumCalculationParameter>();
    public virtual ICollection<PremiumCalculationRule> Rules { get; set; } = new List<PremiumCalculationRule>();
    public virtual ICollection<PremiumCalculationRateTable> RateTables { get; set; } = new List<PremiumCalculationRateTable>();
}

