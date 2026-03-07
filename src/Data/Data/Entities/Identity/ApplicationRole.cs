using Data.Entities.BaseEntity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities.Identity;
public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {

        // 🔥 REMOVE Identity's default unique index on NormalizedName
        builder.HasIndex(r => r.NormalizedName)
               .IsUnique(false);

        // ✅ Correct uniqueness: TenantId + NormalizedName
        builder.HasIndex(r => new { r.TenantId, r.NormalizedName })
               .IsUnique();
    }
}

[EntityTypeConfiguration(typeof(ApplicationRoleConfiguration))]
public class ApplicationRole: IdentityRole<string>, IBaseEntity, IAuditableEntity, ITenantEntity
{
    public string Description { get; set; }
    /// <summary>Human-readable display name for the role (e.g. "Chief Executive Officer" for CEO).</summary>
    public string RoleDisplayName { get; set; }
    public string RoleType { get; set; }
    public int RoleLevel { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Tenant ID for multi-tenancy support. Automatically set when saving.
    /// </summary>
    public string TenantId { get; set; }
}
