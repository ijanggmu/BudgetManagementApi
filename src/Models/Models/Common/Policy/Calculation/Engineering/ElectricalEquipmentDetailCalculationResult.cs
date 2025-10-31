using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Engineering
{
    public class ElectricalEquipmentDetailCalculationResult : CommonEngineeringDetailsProperty
    {
        public CalculationSubDetailAmountModel BasicPremiumForSectionA { get; set; }
        public CalculationSubDetailAmountModel BasicPremiumForSectionB { get; set; }
        public CalculationSubDetailAmountModel BasicPremiumForSectionC { get; set; }
        public RSMDTProperties MaxRSMDT { get; set; }
        public RSMDTProperties SecondRSMDT { get; set; }
        public RSMDTProperties MinRSMDT { get; set; }
        public bool IsPremiumManual { get; set; }
        
    }
}
