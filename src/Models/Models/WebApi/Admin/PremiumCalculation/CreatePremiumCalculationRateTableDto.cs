using System.ComponentModel.DataAnnotations;

namespace Models.WebApi.Admin.PremiumCalculation;

public record CreatePremiumCalculationRateTableDto(
    [Required]
    [MaxLength(100)]
    string TableName,

    [Required]
    string SchemaJson,

    [Required]
    string DataJson,

    [Required]
    [MaxLength(100)]
    string LookupKey
);

