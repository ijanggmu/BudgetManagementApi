using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Engineering
{
    public class ElectricalEquipmentMaterialsExcelData
    {
        public string SN { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Section { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitValue { get; set; }
        public decimal SumInsured { get; set; }
        public decimal Basic { get; set; }
        public decimal RSMDT { get; set; }
    }
}
