using System;

namespace Models.Common.Policy.Policy.GPA
{
    public class InsuredPerson
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PrimaryId { get; set; }
        public string ParentId { get; set; }
        public bool IsActive { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public string NatureOfOccupation { get; set; }
        public decimal SumInsured { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public InsuredPersonClassification Classification { get; set; }
        public bool IsEPremiumSelected { get; set; }
        public bool IsDPremiumSelected { get; set; }
        public decimal? MedicalBenefitsSumInsured { get; set; }
        public decimal Pa { get; set; }
        public decimal Medical { get; set; }
        public decimal RSMDT { get; set; }
        public decimal FullSumInsured { get; set; }
        public decimal? FullMedicalBenefitsSumInsured { get; set; }

        //new fields
        public string Profession { get; set; }
        public string ProfessionRelatedRisk { get; set; }
        public bool AdditionalRisk { get; set; }
        public string AdditionalRiskDetail { get; set; }
        public decimal? AdditionalMedicalSumInsured { get; set; }
        public string Designation { get; set; }
        public bool IsAdditionalRiskDetailEdited { get; set; }
        
        //Gpa-player start
        
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        //Gpa-player end
    }

    public class EndorsedInsuredPerson : InsuredPerson
    {
        public decimal ABCDBasicPremium { get; set; }
        public decimal EPremium { get; set; }
        public decimal GroupDiscount { get; set; }
        public decimal DirectDiscount { get; set; }
        public decimal RSMDTAmount { get; set; }
    }
}
