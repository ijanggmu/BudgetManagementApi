using Data.Entities.BaseEntity;
using Data.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.FodoEntity;
public class FodoConfiguration : IEntityTypeConfiguration<Fodo>
{
    public void Configure(EntityTypeBuilder<Fodo> builder)
    {
        builder.ToTable("Fodos");
        builder.HasIndex(x => x.FullName).IsUnique(false);
    }
}

[EntityTypeConfiguration(typeof(FodoConfiguration))]
public class Fodo : ApplicationBaseEntity, ITenantEntity
{
    public string FullName { get; set; }
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
    
    /// <summary>
    /// Tenant ID for multi-tenancy support. Automatically set when saving.
    /// </summary>
    public string TenantId { get; set; }
}
