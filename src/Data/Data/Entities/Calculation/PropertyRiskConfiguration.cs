using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.Tenant;

namespace Data.Entities.Calculation;
public class PropertyRiskConfiguration : TenantEntity
{
    public string RiskCode { get; set; }
    public string RateCode { get; set; }
    public string RiskType { get; set; }
    public string PropertyDescription { get; set; }
    public decimal Rate { get; set; }
}
