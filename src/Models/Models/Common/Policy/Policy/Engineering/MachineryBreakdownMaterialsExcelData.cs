using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Engineering
{
    public class MachineryBreakdownMaterialsExcelData
    {
        public string ItemNo { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string MakersName { get; set; }
        public string CountryOfOrigin { get; set; }
        public int YearOfMake { get; set; }
        public decimal ReplacementValue { get; set; }
    }
}
