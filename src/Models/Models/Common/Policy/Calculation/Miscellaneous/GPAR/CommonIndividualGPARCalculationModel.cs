using System;

namespace Models.Common.Policy.Calculation.Miscellaneous.GPAR
{
    public class CommonIndividualGPARCalculationModel
    {
        public int SN { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Age { get; set; }
        public string Occupation { get; set; }
        public decimal SumInsured { get; set; }
        public decimal BasicRate { get; set; }
        public decimal PAPremiumAmount { get; set; }  /// Personal Accident Premium Amount
        public decimal EPremiumAmount { get; set; }
        public decimal ShortScalePAPremiumAmount { get; set; }
        public decimal ShortScaleEPremiumAmount { get; set; }
        public decimal DirectDiscountAmount { get; set; }
        public decimal SpecialDiscountAmount { get; set; }
        public decimal ShortScaleDirectDiscountAmount { get; set; }
        public decimal ShortScaleSpecialDiscountAmount { get; set; }
        public decimal PAGrossPremiumAmount { get; set; }
        public decimal PAGrossPremiumAmountBeforeSpecialDiscount { get; set; }
        public decimal ShortScalePAGrossPremiumAmount { get; set; }
        public decimal ShortScalePAGrossPremiumAmountBeforeSpecialDiscount { get; set; }
        public decimal RSMDTRate { get; set; }
        public decimal RSMDTAmount { get; set; }
        public decimal ShortScaleRSMDTAmount { get; set; }
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

        // new additional fields
        public bool AdditionalRisk { get; set; }
        public decimal AdditionalRiskRate { get; set; }
        public decimal AdditionalRiskAmount { get; set; }
        public string Profession { get; set; }
        public string ProfessionRelatedRisk { get; set; }
        public string Designation { get; set; }
        public string AdditionalRiskDetail { get; set; }
        public bool IsAbove1900 { get; set; }
        public decimal Above1900Rate { get; set; }

        public decimal UptoBaseCampRate { get; set; }

        public bool IsAdditionalRiskDetailEdited { get; set; }
        public decimal PremiumAmount { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal? SIForPrint { get; set; }


    }

}
