using Data.Entities.BaseEntity;
using Data.Entities.Identity;

namespace Data.Entities.CorporateEntity;
public class Corporate : ApplicationBaseEntity
{
    public string CorporateName { get; set; }
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
}
