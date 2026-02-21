using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public class ApprovalConfigConfiguration : IEntityTypeConfiguration<ApprovalConfig>
{
    public void Configure(EntityTypeBuilder<ApprovalConfig> builder)
    {
        builder.ToTable("ApprovalConfigs");
        builder.HasIndex(x => new { x.DepartmentId, x.TenantId }).IsUnique();
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.StepsJson)
            .HasColumnType("jsonb");
    }
}

[EntityTypeConfiguration(typeof(ApprovalConfigConfiguration))]
public class ApprovalConfig : TenantEntity
{
    public string DepartmentId { get; set; } = default!;
    /// <summary>
    /// JSON array of approval steps: [{ "stepOrder": 1, "minAmount": 0, "maxAmount": 1000000, "approverRoleId": "...", "approverRoleName": "...", "isMandatory": true }]
    /// </summary>
    public string StepsJson { get; set; } = "[]";
}
