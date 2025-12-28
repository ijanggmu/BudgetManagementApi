using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.Tenant;

namespace Data.Entities.Calculation;
public class GlobalConfiguration : TenantEntity
{
    public string Type { get; set; }
    public int? TypeEnumValue { get; set; }
    public string DataType { get; set; }
    [Column(TypeName = "decimal(15, 4)")]
    public decimal Value { get; set; }
    public string Level { get; set; }
    public string ValueType { get; set; }
    public decimal LowerLimit { get; set; }
    public decimal UpperLimit { get; set; }
    public bool LowerLimitEquals { get; set; }
    public bool UpperLimitEquals { get; set; }
    [NotMapped]
    public bool IsConfigured { get; set; }
}
