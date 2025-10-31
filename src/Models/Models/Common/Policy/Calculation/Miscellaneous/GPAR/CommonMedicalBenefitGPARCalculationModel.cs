using System;
using Models.Common.Policy.Policy.Miscellaneous;

namespace Models.Common.Policy.Calculation.Miscellaneous.GPAR
{
    public class CommonMedicalBenefitGPARCalculationModel : MedicalBenefitCommonProperties
    {
        public decimal ShortScaleMedicalPremiumAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
    }

}
