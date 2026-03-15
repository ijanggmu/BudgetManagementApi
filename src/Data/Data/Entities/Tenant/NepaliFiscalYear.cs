using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public class NepaliFiscalYearConfiguration : IEntityTypeConfiguration<NepaliFiscalYear>
{
    public void Configure(EntityTypeBuilder<NepaliFiscalYear> builder)
    {
        builder.ToTable("NepaliFiscalYears");
        builder.HasIndex(x => new { x.Code, x.TenantId }).IsUnique();
    }
}

[EntityTypeConfiguration(typeof(NepaliFiscalYearConfiguration))]
public class NepaliFiscalYear : TenantEntity
{
    /// <summary>Display code e.g. "23/24", "24/25", "25/26".</summary>
    public string Code { get; set; } = default!;
    /// <summary>Start date and time of the fiscal year, stored in UTC.</summary>
    public DateTime StartDateUtc { get; set; }
    /// <summary>End date and time of the fiscal year, stored in UTC.</summary>
    public DateTime EndDateUtc { get; set; }
    public string? Description { get; set; }
}
