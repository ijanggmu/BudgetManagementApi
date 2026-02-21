using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public enum BudgetRequestStatus
{
    PendingApproval = 0,
    Approved = 1,
    Rejected = 2
}

public class BudgetRequestConfiguration : IEntityTypeConfiguration<BudgetRequest>
{
    public void Configure(EntityTypeBuilder<BudgetRequest> builder)
    {
        builder.ToTable("BudgetRequests");
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.RequestedDate });
        builder.Property(x => x.ApprovalHistoryJson).HasColumnType("jsonb");
    }
}

[EntityTypeConfiguration(typeof(BudgetRequestConfiguration))]
public class BudgetRequest : TenantEntity
{
    public string DepartmentId { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = default!;
    public BudgetRequestStatus Status { get; set; }
    public string? NextApproverRoleId { get; set; }
    public int CurrentApprovalStep { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? MemoFileUrl { get; set; }
    /// <summary>JSON array of { roleId, roleName, userId, userName, approvedAt, signatureUrl } for each approval step.</summary>
    public string ApprovalHistoryJson { get; set; } = "[]";
}
