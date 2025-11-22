using Data.Entities.BaseEntity;
using Data.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities.AdminEntity;
public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {

        builder.HasIndex(x => x.FullName).IsUnique(false);

    }
}

[EntityTypeConfiguration(typeof(AdminConfiguration))]
public class Admin : ApplicationBaseEntity, ITenantEntity
{
    public string FullName { get; set; }
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
    
    /// <summary>
    /// Tenant ID for multi-tenancy support. Null for SuperAdmin, set for tenant-specific admins.
    /// </summary>
    public string TenantId { get; set; }
}


