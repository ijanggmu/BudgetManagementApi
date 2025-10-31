using Data.Entities.BaseEntity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Identity;
public class ApplicationUserRolesConfiguration : IEntityTypeConfiguration<ApplicationUserRoles>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRoles> builder)
    {
    }
}

[EntityTypeConfiguration(typeof(ApplicationUserRolesConfiguration))]
public class ApplicationUserRoles : IdentityUserRole<string>, IBaseEntity, IAuditableEntity
{
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

}
