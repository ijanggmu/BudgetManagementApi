using System.ComponentModel.DataAnnotations;

namespace Models.WebApi.Admin.PremiumCalculation;

public record CreatePremiumCalculationRuleDto(
    [Required]
    [MaxLength(200)]
    string RuleName,

    [Required]
    [MaxLength(50)]
    string RuleType,

    string? Expression,
    string? Condition,
    int Priority = 0,
    bool IsActive = true
);

