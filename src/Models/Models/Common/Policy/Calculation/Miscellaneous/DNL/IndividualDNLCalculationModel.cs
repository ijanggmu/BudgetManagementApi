using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Calculation.Miscellaneous.DNL
{
    public class IndividualDNLCalculationModel
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Occupation { get; set; }
        public string InsuredId { get; set; }
        public int NumberOfPerson { get; set; }
        public string GroupName { get; set; }
        public decimal? GpaInsuranceSumInsuredPerPerson { get; set; }
        public decimal? MedicalInsuranceSumInsuredPerPerson { get; set; }
        public decimal? CriticalIllnessInsuranceSumInsuredPerPerson { get; set; }
        public string Id { get; set; }
        public string PrimaryId { get; set; }
        public bool IsGpaInsuranceSelected { get; set; }
        public bool IsMedicalInsuranceSelected { get; set; }
        public bool IsCriticalIllnessInsuranceSelected { get; set; }
        public decimal? GpaInsuranceSumInsured { get; set; }
        public decimal? MedicalInsuranceSumInsured { get; set; }
        public decimal? CriticalIllnessInsuranceSumInsured { get; set; }
        public decimal GpaInsuranceInsuredAmount { get; set; }
        public decimal MedicalInsuranceInsuredAmount { get; set; }
        public decimal CriticalIllnessInsuranceInsuredAmount { get; set; }
        public decimal BasicPremiumAmount { get; set; }
        public decimal RSMDTAmountBeforeDiscount { get; set; }
        public decimal RSMDTAmount { get; set; }
        public decimal GroupDiscountAmount { get; set; }
        public decimal TotalODP { get; set; }
        public decimal TransactionBasicPremium { get; set; }
        public decimal TransactionRSMDT { get; set; }
        public decimal TotalPremium { get; set; }
        public decimal FullTotalPremium { get; set; }
        public decimal SumInsured { get; set; }
        public decimal PremiumAmount { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal FullSumInsured { get; set; }
        public decimal FullMedicalSumInsured { get; set; }
        public decimal FullMedicalInsuredAmount { get; set; }
        public decimal FullGPAInsuranceSumInsured { get; set; }
        public decimal FullGPAInsuranceInsuredAmount { get; set; }
        public decimal FullCriticalIllnessSumInsured { get; set; }
        public decimal FullCriticalIllnessInsuredAmount { get; set; }
        public decimal FullBasicPremium { get; set; }
        public decimal FullRSMDTPremium { get; set; }
        public bool IsAdded { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsExisting { get; set; }
        public bool IsNamed { get; set; }
    }
}
