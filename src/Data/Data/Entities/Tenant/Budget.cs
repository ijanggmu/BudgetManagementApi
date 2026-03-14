using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("Budgets");
        builder.HasIndex(x => new { x.DepartmentId, x.Year, x.Quarter, x.TenantId }).IsUnique();
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BudgetHeading>()
            .WithMany()
            .HasForeignKey(x => x.BudgetHeadingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BudgetSubheading>()
            .WithMany()
            .HasForeignKey(x => x.BudgetSubheadingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<NepaliFiscalYear>()
            .WithMany()
            .HasForeignKey(x => x.FiscalYearId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

[EntityTypeConfiguration(typeof(BudgetConfiguration))]
public class Budget : TenantEntity
{
    public string DepartmentId { get; set; } = default!;
    public string? BudgetHeadingId { get; set; }
    public string? BudgetSubheadingId { get; set; }
    /// <summary>Nepali fiscal year (e.g. 23/24). Optional for backward compatibility.</summary>
    public string? FiscalYearId { get; set; }
    public int Year { get; set; }
    public int Quarter { get; set; }
    /// <summary>Quantity/units for budget line. TotalAmount = Unit * UnitAmount when both set.</summary>
    public decimal Unit { get; set; } = 1;
    /// <summary>Amount per unit. TotalAmount = Unit * UnitAmount when both set.</summary>
    public decimal UnitAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    /// <summary>
    /// When true, department roles cannot modify this budget; only Financial Authority can unlock.
    /// </summary>
    public bool IsLocked { get; set; }
    /// <summary>
    /// Revision version for audit; incremented on each update.
    /// </summary>
    public int Version { get; set; } = 1;
}
