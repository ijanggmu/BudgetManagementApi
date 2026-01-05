using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.BaseEntity;

namespace Data.Entities.Marine;
public class MarineTariffSchedule : ApplicationBaseEntity
{
    public string ProductCategory { get; set; }
    public string ProductCategoryCode { get; set; }
    public string Product { get; set; }
    public string ProductCode { get; set; }
    public decimal AllRiskValue { get; set; }
    public decimal BasicRiskValue { get; set; }
    public string ProductDescription { get; set; }
    public decimal MinimumRisk { get; set; }
}
