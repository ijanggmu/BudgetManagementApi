using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.BaseEntity;

namespace Data.Entities.Tenant;
// moved: Product, Coverage -> Product.cs ; RateFactor, RateTable, PremiumFormula, UnderwritingRule -> RatingArtifacts.cs
public class DocumentTemplate : TenantEntity
{
    public string Name { get; set; } = default!;
    public string Kind { get; set; } = "quotation|policy";
    public string ContentUri { get; set; } = default!; // html/pdf template in blob
}


