using System.ComponentModel.DataAnnotations;

namespace Models.WebApi.Admin.PremiumCalculation;

public record CreatePremiumCalculationParameterDto(
    [Required]
    [MaxLength(100)]
    string ParameterKey,

    [Required]
    [MaxLength(200)]
    string ParameterName,

    [Required]
    [MaxLength(50)]
    string DataType,

    [Required]
    string Value,

    string? DefaultValue,
    decimal? MinValue,
    decimal? MaxValue,
    [MaxLength(100)]
    string? Category,
    bool IsRequired = false,
    int DisplayOrder = 0
);

