using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class TractorPartialViewModel
    {
        public CommonMotorModel CommonProperties { get; set; }
        public CommonRSMDTModel CommonRSMDTModel { get; set; }
        public bool PersonalAccidentForConductor { get; set; }
        public bool IsPersonalAccidentForHelper { get; set; }
        public bool IsRiotStrikeAndTerrorismForHelper { get; set; }
        public decimal HorsePower { get; set; }
        public bool IsCubicCapacity { get; set; }
        public bool IsHorsePower { get; set; }
        public bool IsOwnUse { get; set; }
    }
}
