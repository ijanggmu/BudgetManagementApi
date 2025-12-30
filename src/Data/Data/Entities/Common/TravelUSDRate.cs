using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.BaseEntity;

namespace Data.Entities.Common;
public class TravelUSDRate : ApplicationBaseEntity
{
    public string Issuer { get; set; }
    public string Group { get; set; }
    public string PlanType { get; set; }
    public int PeriodFrom { get; set; }
    public int PeriodTo { get; set; }
    [Column(TypeName = "decimal(15, 2)")]
    public decimal IndividaulRate { get; set; }
    [Column(TypeName = "decimal(15, 2)")]
    public decimal FamilyRate { get; set; }
    public string DestintionIncludes { get; set; }
    public string MultipleEntries { get; set; }
    public int AgeFrom { get; set; }
    public int AgeTo { get; set; }
    public string Currency { get; set; }

}
