using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class PAPartialViewModel : CommonAnusuchiViewModel
    {
        //public CommonAnusuchiViewModel CommonAnusuchiViewModel { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string Nominee { get; set; }
        public string NomineeRelation { get; set; }
        public decimal SumInsured { get; set; }
        public decimal OldSumInsured { get; set; }
        public decimal? MedicalBenefit { get; set; }
        public decimal? OldMedicalBenefit { get; set; }
        public decimal? MedicalBenifitTransaction { get; set; }
        public decimal LimitOfIndemnity { get; set; }
        public decimal OldLimitOfIndemnity { get; set; }
        public decimal TransactionLimitOfIndemnity { get; set; }

        public string Remarks { get; set; }
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int PolicyPeriodInDays { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public decimal BasicPremiumRate { get; set; }
        public bool IsDPremiumSelected { get; set; }
        public decimal DPremiumRate { get; set; }
        public bool IsEPremiumSelected { get; set; }
        public decimal EPremiumRate { get; set; }
        public bool IsRSMDTPremiumSelected { get; set; }
        public decimal RSMDTPremiumRate { get; set; }
        public decimal DirectDiscountRate { get; set; }
        public decimal? MedicalBenefitRate { get; set; }

        public bool IsPremiumManual { get; set; }
        public decimal BasicPremiumAmount { get; set; }
        public decimal RSMDTAmount { get; set; }
        public string ConfirmationDetails { get; set; }
       
        public string PASubType { get; set; }
        public decimal BasicPremiumBiker { get; set; }
        public decimal BasicPremiumShramjibi { get; set; }
        public decimal BasicPremiumNari { get; set; }
        public decimal MedicalBiker { get; set; }
        public decimal MedicalShramjibi { get; set; }
        public decimal MedicalNari { get; set; }
        public decimal RSMDTBiker { get; set; }
        public decimal RSMDTShramjibi { get; set; }
        public decimal RSMDTNari { get; set; }

        // new fields
        public string Address { get; set; }
        public string Designation { get; set; }
        public int Age { get; set; }
        public string ContactNumber { get; set; }
        public string Profession { get; set; }
        public string ProfessionRelatedRisk { get; set; }
        public decimal RSMDTRate { get; set; }
        public decimal AdditionalMedicalBenefitRate { get; set; }
        public decimal AdditionalRiskRate { get; set; }
        public decimal? AdditionalMedicalBenefit { get; set; }
        public bool AdditionalRisk { get; set; }
        public string AdditionalRiskDetails { get; set; }
        public decimal MedicalPremiumAmount { get; set; }
        public string MedicalTreatment { get; set; }
        public string Deductibles { get; set; }
        public bool IsAdditionalRiskDetailEdited { get; set; }


    }
}
