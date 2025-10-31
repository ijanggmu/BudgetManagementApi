using System;

namespace Models.Common.Policy.Calculation.Miscellaneous.TR
{
    public class CommonIndividualTRCalculationModel
    {
        public int SN { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Age { get; set; }
        public string Occupation { get; set; }
        public decimal SumInsured { get; set; }
        public decimal BasicRate { get; set; }
        public decimal PAPremiumAmount { get; set; }  /// Personal Accident Premium Amount
        public decimal MBPremiumAmount { get; set; }  /// Medical Benefit Premium Amount
        public decimal MedicalBenefitPremiumAbove19000Ft { get; set; }  /// Total Medical Benefit Premium Amount above 19000 ft
        public decimal MedicalBenefitPremiumBelow19000Ft { get; set; }  /// Total Medical Benefit Premium Amount
        ///         public decimal DirectDiscountAmount { get; set; }
        public decimal DirectDiscountAmount { get; set; }
        public decimal CorporateDiscountAmount { get; set; }
        public decimal ShortScaleDirectDiscountAmount { get; set; }
        public decimal ShortScaleCorporateDiscountAmount { get; set; }
        public decimal EPremiumAmount { get; set; }
        public decimal PAGrossPremiumAmount { get; set; } //PA + Medical sum insured
        public decimal RSMDTRate { get; set; }
        public decimal RSMDTAmount { get; set; }
        public decimal ShortScaleRSMDTAmount { get; set; }
        public decimal SubTotalA { get; set; }
        public decimal SubTotalB { get; set; }
        public decimal TotalPremium { get; set; }
        public decimal MedicalSumInsuredLimit { get; set; }
        public decimal MedicalExpensesRate { get; set; }
        public decimal AdditionalMedicalSumInsured { get; set; }
        public decimal TotalMedicalSumInsured { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public string Designation { get; set; }
        public bool IsAbove19000Ft { get; set; }
        public bool IsAdministrativeStaff { get; set; }
        public decimal Above19000Rate { get; set; }
        public decimal AdministrativeStaffRate { get; set; }
        public decimal PremiumAmount { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal? SIForPrint { get; set; }
        
    }

}
