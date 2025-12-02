using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Tenant;

public class PremiumCalculationRateTable : TenantEntity
{
    [Required]
    [MaxLength(450)]
    public string ConfigurationId { get; set; } = default!;

    [ForeignKey(nameof(ConfigurationId))]
    public virtual PremiumCalculationConfiguration Configuration { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string TableName { get; set; } = default!; // e.g., "AgeBasedRates", "VehicleTypeRates"

    [Required]
    public string SchemaJson { get; set; } = "{}"; // Column definitions

    [Required]
    public string DataJson { get; set; } = "[]"; // Rate table data (JSON array)

    [Required]
    [MaxLength(100)]
    public string LookupKey { get; set; } = default!; // Primary lookup column
}

