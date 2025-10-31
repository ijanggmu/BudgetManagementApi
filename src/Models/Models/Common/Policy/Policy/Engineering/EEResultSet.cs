using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Engineering
{
    public class EEResultSet
    {
        public List<EEExcessAmountRate> ExcessAmountRate { get; set; }
        public List<ElectricalEquipmentMaterialsExcelData> ExcelData { get; set; }
    }
}
