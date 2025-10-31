using System;

namespace Models.Common.Policy.Calculation.GPA
{
    public class IndividualGPACalculationModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PrimaryId { get; set; }
        public int Age { get; set; }
        public string EmployeeId { get; set; }
        public string NatureOfOccupation { get; set; }
        public string Name { get; set; }
        public string Classification { get; set; }
        public decimal SumInsured { get; set; }
        public decimal TransactionSumInsured { get; set; }
        public decimal TotalSumInsured { get; set; }
        public decimal ABCPremiumRate { get; set; }
        public decimal ABCDPremiumRate { get; set; }
        public decimal BasicPremiumAmount { get; set; }
        public decimal AdditionalRiskAmount { get; set; }
        public decimal AdditionalMedicalBenefitAmount { get; set; }
        public decimal? MedicalBenefitsSumInsured { get; set; }
        public decimal EPremiumAmount { get; set; }
        public decimal EPremiumRate { get; set; }
        public decimal PremiumWithE { get; set; }
        public decimal GroupDiscountAmount { get; set; }
        public decimal PremiumAfterGroupDiscount { get; set; }
        public decimal DirectDiscountAmount { get; set; }
        public decimal RSMDTRate { get; set; }
        public decimal RSMDTAmount { get; set; }
        public decimal ShortScaleRSMDTAmount { get; set; }
        public decimal ForcedTenPercentOfSI { get; set; }
        public decimal SubTotalA { get; set; }
        public decimal SubTotalB { get; set; }
        public decimal TotalPremium { get; set; }
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
        public decimal PremiumAmount { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal TransactionBasicPremium { get; set; }
        public decimal TransactionRSMDT { get; set; }
        public decimal FullSumInsured { get; set; }
        public decimal FullTotalSumInsured { get; set; }
        public decimal? FullMedicalBenefitsSumInsured { get; set; }
        public decimal FullBasicPremiumAmount { get; set; }
        public decimal FullRSMDTAmount { get; set; }

        //new properties
        public string Profession { get; set; }
        public string ProfessionRelatedRisk { get; set; }
        public bool AdditionalRisk { get; set; }
        public string AdditionalRiskDetail { get; set; }
        public decimal? AdditionalMedicalSumInsured { get; set; }
        public string Designation { get; set; }
        public bool IsAdditionalRiskDetailEdited { get; set; }
        
        //GPA-Player
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal ActualPremiumAmount { get; set; }
        public decimal BasicRate { get; set; }
       
    }
}
