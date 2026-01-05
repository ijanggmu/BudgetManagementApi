using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.BaseEntity;

namespace Data.Entities.Common;
public class HEOMITravelRate : ApplicationBaseEntity
{
    public string Plan { get; set; }
    public int PeriodFrom { get; set; }
    public int PeriodTo { get; set; }
    public double IndividualRate { get; set; }
    public double FamilyRate { get; set; }
    public bool IsAnnualTrip { get; set; }
}
