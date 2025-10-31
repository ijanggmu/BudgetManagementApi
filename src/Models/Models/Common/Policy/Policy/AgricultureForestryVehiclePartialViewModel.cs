using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class AgricultureForestryVehiclePartialViewModel
    {
        public CommonMotorModel CommonProperties { get; set; }
        public CommonRSMDTModel CommonRSMDTModel { get; set; }
        public bool PersonalAccidentForConductor { get; set; }
        public bool IsPersonalAccidentForHelper { get; set; }
        public bool IsRiotStrikeAndTerrorismForHelper { get; set; }
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,2})?", ErrorMessage = ("Must be a number with 2 decimal points and maximum length of 25"))]
        public string SumInsuredAmountForHelper { get; set; }
        public bool OwnUseForPrivateTourismSchool { get; set; }
        public decimal GoodsCarryingCapacity { get; set; }
    }
}
