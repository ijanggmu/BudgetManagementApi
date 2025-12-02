using System;
using System.ComponentModel.DataAnnotations;

namespace Models.WebApi.Admin.PremiumCalculation;

public record CreatePremiumCalculationConfigurationDto(
    [Required]
    [MaxLength(100)]
    string PortfolioAlias,

    [Required]
    [MaxLength(20)]
    string FiscalYear,

    [Required]
    DateOnly EffectiveFrom,

    DateOnly? EffectiveTo,

    [MaxLength(500)]
    string? Description,

    [MaxLength(50)]
    string CalculationEngineType = "FormulaBased"
);

