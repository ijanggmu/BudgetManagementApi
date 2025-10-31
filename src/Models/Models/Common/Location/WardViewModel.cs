using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Location;
public class WardViewModel
{
    public string Code { get; set; }
    public string MunicipalityCode { get; set; }
    public string MunicipalityName { get; set; }
    public int WardNumber { get; set; }
    public string NameNp { get; set; }
}
