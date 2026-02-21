using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public enum MemoStatus
{
    Draft = 0,
    Final = 1
}

public class MemoConfiguration : IEntityTypeConfiguration<Memo>
{
    public void Configure(EntityTypeBuilder<Memo> builder)
    {
        builder.ToTable("Memos");
        builder.HasOne<BudgetRequest>()
            .WithMany()
            .HasForeignKey(x => x.BudgetRequestId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.ApproversJson)
            .HasColumnType("jsonb");
        builder.HasIndex(x => new { x.BudgetRequestId, x.TenantId }).IsUnique();
    }
}

[EntityTypeConfiguration(typeof(MemoConfiguration))]
public class Memo : TenantEntity
{
    public string BudgetRequestId { get; set; } = default!;
    public string RequestedBy { get; set; } = default!;
    public string RequestedByDepartment { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = default!;
    public string Department { get; set; } = default!;
    public MemoStatus Status { get; set; }
    public string? FileUrl { get; set; }
    /// <summary>
    /// JSON array of approvers: [{ "roleId", "roleName", "userId", "userName", "signatureUrl", "approvedAt" }]
    /// </summary>
    public string ApproversJson { get; set; } = "[]";
}
