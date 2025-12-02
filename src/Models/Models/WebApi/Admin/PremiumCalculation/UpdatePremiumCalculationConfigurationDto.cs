using System;
using System.ComponentModel.DataAnnotations;

namespace Models.WebApi.Admin.PremiumCalculation;

public record UpdatePremiumCalculationConfigurationDto(
    DateOnly? EffectiveFrom,
    DateOnly? EffectiveTo,
    bool? IsActive,
    [MaxLength(500)]
    string? Description,
    [MaxLength(50)]
    string? CalculationEngineType
);

