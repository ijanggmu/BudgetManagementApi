using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Tenant;

public class PremiumCalculationRule : TenantEntity
{
    [Required]
    [MaxLength(450)]
    public string ConfigurationId { get; set; } = default!;

    [ForeignKey(nameof(ConfigurationId))]
    public virtual PremiumCalculationConfiguration Configuration { get; set; } = default!;

    [Required]
    [MaxLength(200)]
    public string RuleName { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string RuleType { get; set; } = default!; // Formula, Condition, Validation, Discount

    public string? Expression { get; set; } // Formula/expression e.g., "BasePremium * RateFactor + Fees"

    public string? Condition { get; set; } // JSON condition e.g., {"Age": {"$gte": 18, "$lte": 65}}

    public int Priority { get; set; } = 0; // Execution order

    public bool IsActive { get; set; } = true;
}

