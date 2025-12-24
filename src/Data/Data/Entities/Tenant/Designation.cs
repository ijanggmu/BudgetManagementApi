using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public class DesignationConfiguration : IEntityTypeConfiguration<Designation>
{
    public void Configure(EntityTypeBuilder<Designation> builder)
    {
        builder.ToTable("Designations");
        builder.HasIndex(x => new { x.Title, x.TenantId }).IsUnique();
    }
}

[EntityTypeConfiguration(typeof(DesignationConfiguration))]
public class Designation : TenantEntity
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
}

