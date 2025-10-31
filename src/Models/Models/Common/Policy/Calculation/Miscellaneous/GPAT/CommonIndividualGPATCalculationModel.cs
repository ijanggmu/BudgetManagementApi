using System;
using System.Collections.Generic;
using System.Text;
using Models.Common.Policy.Policy.Miscellaneous;

namespace Models.Common.Policy.Calculation.Miscellaneous.GPAT
{
    public class CommonIndividualGPATCalculationModel : ClassificationRiskCoverageRateViewModel
    {
        public int SN { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PrimaryId { get; set; }
        public int Age { get; set; }
        public string Occupation { get; set; }
        public decimal SumInsured { get; set; }
        public decimal PremiumAmount { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal MedicalBenefitSumInsured { get; set; }
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
        public decimal RSMDTRate { get; set; }
        public decimal RSMDTAmount { get; set; }
        public decimal ShortScaleRSMDTAmount { get; set; }
        //    public decimal TenPercentOfSI { get; set; }
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

        public int Count { get; set; }
        public bool IsNamed { get; set; }
        public bool IsDSelected { get; set; }
        public bool IsESelected { get; set; }
        public bool IsNew { get; set; }

        //new fields
        public string Profession { get; set; }
        public string ProfessionRelatedRisk { get; set; }
        public bool AdditionalRisk { get; set; }
        public string AdditionalRiskDetail { get; set; }
        public decimal? AdditionalMedicalSumInsured { get; set; }
        public string Designation { get; set; }
        public bool IsAdditionalRiskDetailEdited { get; set; }
        public decimal AdditionalRiskAmount { get; set; }
        public decimal AdditionalMedicalBenefitAmount { get; set; }

    }

}
