using Data.Entities.BaseEntity;
using Data.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.AgentEntity;
public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {

        builder.HasIndex(x => x.FullName).IsUnique(false);

    }
}

[EntityTypeConfiguration(typeof(AgentConfiguration))]
public class Agent : ApplicationBaseEntity, ITenantEntity
{
    public string FullName { get; set; }
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
    
    /// <summary>
    /// Tenant ID for multi-tenancy support. Automatically set when saving.
    /// </summary>
    public string TenantId { get; set; }
}
