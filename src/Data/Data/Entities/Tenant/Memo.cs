using Data.Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tenant;

public enum MemoStatus
{
    Draft = 0,
    Final = 1,       // Kept for backward compatibility (existing memos)
    Prepared = 2,
    Supported = 3,
    Recommended = 4,
    Approved = 5,
    Archived = 6
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
        builder.HasOne<MemoTemplate>()
            .WithMany()
            .HasForeignKey(x => x.MemoTemplateId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BudgetHeading>()
            .WithMany()
            .HasForeignKey(x => x.BudgetHeadingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BudgetSubheading>()
            .WithMany()
            .HasForeignKey(x => x.BudgetSubheadingId)
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
    public string? MemoTemplateId { get; set; }
    public string? BudgetHeadingId { get; set; }
    public string? BudgetSubheadingId { get; set; }
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
