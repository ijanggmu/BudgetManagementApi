using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public class BudgetHeadingConfiguration : IEntityTypeConfiguration<BudgetHeading>
{
    public void Configure(EntityTypeBuilder<BudgetHeading> builder)
    {
        builder.ToTable("BudgetHeadings");
        builder.HasIndex(x => new { x.Code, x.TenantId }).IsUnique();
    }
}

[EntityTypeConfiguration(typeof(BudgetHeadingConfiguration))]
public class BudgetHeading : TenantEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
}
