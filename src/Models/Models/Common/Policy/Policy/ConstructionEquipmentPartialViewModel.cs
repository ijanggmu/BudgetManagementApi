using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class ConstructionEquipmentPartialViewModel
    {
        public CommonMotorModel CommonProperties { get; set; }
        public CommonRSMDTModel CommonRSMDTModel { get; set; }
        public bool IsPersonalAccidentForHelper { get; set; }
        public bool IsRiotStrikeAndTerrorismForHelper { get; set; }
        public string SumInsuredAmountForHelper { get; set; }
        public bool OwnUseForPrivateTourismSchool { get; set; }
        public decimal GoodsCarryingCapacity { get; set; }
        public bool IsHeavyEquipment { get; set; }
    }
}
