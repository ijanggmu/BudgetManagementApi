using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Engineering
{
    public class BoilerMaterialsExcelData : BoilerSurroundingPropertyExcelData
    {
        public string Location { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal? YearOfMake { get; set; }
    }
}
