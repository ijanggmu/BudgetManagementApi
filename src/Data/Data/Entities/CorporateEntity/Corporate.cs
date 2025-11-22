using Data.Entities.BaseEntity;
using Data.Entities.Identity;

using Data.Entities.BaseEntity;

namespace Data.Entities.CorporateEntity;
public class Corporate : ApplicationBaseEntity, ITenantEntity
{
    public string CorporateName { get; set; }
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
    
    /// <summary>
    /// Tenant ID for multi-tenancy support. Automatically set when saving.
    /// </summary>
    public string TenantId { get; set; }
}
