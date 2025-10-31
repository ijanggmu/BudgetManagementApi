using Data.Entities.BaseEntity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Identity;
public class RoleClaimConfiguration : IEntityTypeConfiguration<ApplicationRoleClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
    {

    }

}

[EntityTypeConfiguration(typeof(RoleClaimConfiguration))]
public class ApplicationRoleClaim : IdentityRoleClaim<string>, IBaseEntity, IAuditableEntity
{
    public List<string> Permissions { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public bool IsDeleted { get; set; }
}
