using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasIndex(x => x.BranchCode).IsUnique();
        builder.HasIndex(x => new { x.BranchName, x.TenantId }).IsUnique();
    }
}

[EntityTypeConfiguration(typeof(BranchConfiguration))]
public class Branch : TenantEntity
{
    public string BranchName { get; set; } = default!;
    public string BranchCode { get; set; } = default!;
    public string Province { get; set; } = default!;
    public string District { get; set; } = default!;
    public string Municipality { get; set; } = default!;
    public int Ward { get; set; }
    public bool IsActive { get; set; } = true;
}


