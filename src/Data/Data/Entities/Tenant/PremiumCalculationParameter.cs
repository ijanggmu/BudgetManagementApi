using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Tenant;

public class PremiumCalculationParameter : TenantEntity
{
    [Required]
    [MaxLength(450)]
    public string ConfigurationId { get; set; } = default!;

    [ForeignKey(nameof(ConfigurationId))]
    public virtual PremiumCalculationConfiguration Configuration { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string ParameterKey { get; set; } = default!;

    [Required]
    [MaxLength(200)]
    public string ParameterName { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string DataType { get; set; } = "decimal"; // decimal, int, bool, string

    [Required]
    public string Value { get; set; } = default!; // JSON-serialized value

    public string? DefaultValue { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal? MinValue { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal? MaxValue { get; set; }

    public bool IsRequired { get; set; } = false;

    public int DisplayOrder { get; set; } = 0;

    [MaxLength(100)]
    public string? Category { get; set; } // Basic, Discount, Tax, etc.
}

