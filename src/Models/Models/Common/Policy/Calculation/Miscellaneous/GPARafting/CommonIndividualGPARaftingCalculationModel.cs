using System;
using Models.Common.Policy.Policy.Miscellaneous;

namespace Models.Common.Policy.Calculation.Miscellaneous.GPARafting
{
    public class CommonIndividualGPARaftingCalculationModel : ClassificationRiskCoverageRateViewModel
    {
        public int SN { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Age { get; set; }
        public string Occupation { get; set; }
        public decimal SumInsured { get; set; }
        public decimal BasicPremiumAmount { get; set; }
        public decimal ShortScaleBasicPremiumAmount { get; set; }
        public decimal EPremiumAmount { get; set; }
        public decimal ShortScaleEPremiumAmount { get; set; }
        public decimal PremiumWithE { get; set; }
        public decimal ShortScalePremiumWithE { get; set; }
        public decimal GroupDiscountAmount { get; set; }
        public decimal ShortScaleGroupDiscountAmount { get; set; }
        public decimal PremiumAfterGroupDiscount { get; set; }
        public decimal DirectDiscountAmount { get; set; }
        public decimal ShortScaleDirectDiscountAmount { get; set; }
        public decimal RSMDTAmount { get; set; }
        public decimal ShortScaleRSMDTAmount { get; set; }
        public decimal TenPercentOfSI { get; set; }
        public decimal ForcedTenPercentOfSI { get; set; }
        public decimal SubTotalA { get; set; }
        public decimal SubTotalB { get; set; }
        public decimal TotalPremium { get; set; }
        public decimal ShortScaleTotalPremium { get; set; }
        public decimal TotalODP { get; set; }
        public decimal ShortScaleODP { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
    }

}
