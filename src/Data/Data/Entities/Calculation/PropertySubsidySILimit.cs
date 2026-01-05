using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.BaseEntity;
using Data.Entities.Tenant;

namespace Data.Entities.Calculation;
public class PropertySubsidySILimit : TenantEntity
{
    public string SILabel { get; set; }
    public decimal SILimit { get; set; }
}
