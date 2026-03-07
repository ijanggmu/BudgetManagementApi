using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public class BudgetMemoAuditLogConfiguration : IEntityTypeConfiguration<BudgetMemoAuditLog>
{
    public void Configure(EntityTypeBuilder<BudgetMemoAuditLog> builder)
    {
        builder.ToTable("BudgetMemoAuditLogs");
        builder.HasIndex(x => new { x.TenantId, x.CreatedOn });
        builder.HasIndex(x => new { x.TenantId, x.EntityType, x.EntityId });
    }
}

[EntityTypeConfiguration(typeof(BudgetMemoAuditLogConfiguration))]
public class BudgetMemoAuditLog : TenantEntity
{
    public string EntityType { get; set; } = default!; // "Budget" | "Memo"
    public string EntityId { get; set; } = default!;
    public string Action { get; set; } = default!; // Created, Updated, Deleted, Locked, Unlocked, StatusChange
    public string? Details { get; set; }
    public string? UserName { get; set; }
}
