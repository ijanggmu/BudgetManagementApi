using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public class BudgetSubheadingConfiguration : IEntityTypeConfiguration<BudgetSubheading>
{
    public void Configure(EntityTypeBuilder<BudgetSubheading> builder)
    {
        builder.ToTable("BudgetSubheadings");
        builder.HasOne<BudgetHeading>()
            .WithMany()
            .HasForeignKey(x => x.BudgetHeadingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.Code, x.TenantId }).IsUnique();
    }
}

[EntityTypeConfiguration(typeof(BudgetSubheadingConfiguration))]
public class BudgetSubheading : TenantEntity
{
    public string BudgetHeadingId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
}
