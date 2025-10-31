using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class CashInTransitPartialViewModel:CommonRiskTypePartialViewModel
    {
        public decimal BasicPremium { get; set; }
        public decimal TPLPremium { get; set; }
        public decimal RSMDTPremium { get; set; }
        public decimal Suminsured { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public decimal Vatpercent { get; set; }
        public decimal StampAmount { get; set; }
        public decimal EndorsedTransactionBasicPremium { get; set; }
        public decimal EndorsedTransactionRSMDTPremium { get; set; }
        public decimal EndorsedTransactionTPLPremium { get; set; }
        public decimal EndorsedTransactionSuminsured { get; set; }
        
        // nEw fields
         [Required(ErrorMessage = "Sum Insured for Cash in Safe is required")]
        public decimal SumInsuredCashInSafe { get; set; }

        public decimal? SumInsuredCashInSafeTypeA { get; set; }
        public decimal? OldSumInsuredCashInSafeTypeA { get; set; }
        public decimal? SumInsuredCashInSafeTypeB { get; set; }
        public decimal? OldSumInsuredCashInSafeTypeB { get; set; }
        [Required(ErrorMessage = "Sum Insured per carrying is required")]
        public decimal SumInsuredPerCarrying { get; set; }
        public decimal? SumInsuredPerCarryingTypeA { get; set; }
        public decimal? OldSumInsuredPerCarryingTypeA { get; set; }
        public decimal? SumInsuredPerCarryingTypeB { get; set; }
        public decimal? OldSumInsuredPerCarryingTypeB { get; set; }
        public decimal? SumInsuredPerCarryingTypeC { get; set; }
        public string SumInsuredPerCarryingTypeCLabel { get; set; }
        public decimal? OldSumInsuredPerCarryingTypeC { get; set; }
        [Required(ErrorMessage = "Sum Insured Annual Aggregate is required")]
        public decimal SumInsuredAnnualAggregate { get; set; }
        public decimal? SumInsuredAnnualAggregateTypeA { get; set; }
        public decimal? OldSumInsuredAnnualAggregateTypeA { get; set; }
        public decimal? SumInsuredAnnualAggregateTypeB { get; set; }
        public decimal? OldSumInsuredAnnualAggregateTypeB { get; set; }
        public decimal? SumInsuredAnnualAggregateTypeC { get; set; }
        public decimal? OldSumInsuredAnnualAggregateTypeC { get; set; }
        public string InsuredPremesisProvince { get; set; }
        public string InsuredPremesisDistrict { get; set; }
        public string InsuredPremesisMunicipality { get; set; }
        public string InsuredPremesisWard { get; set; }
        [Required(ErrorMessage = "Street Address is required")]
        public string InsuredPremesisStreetAddress { get; set; }
        public decimal CashInSafeBasicPremiumRateA { get; set; }
        public decimal CashInSafeBasicPremiumRateB { get; set; }
        public decimal CashInTransitBasicPremiumRateA { get; set; }
        public decimal CashInTransitBasicPremiumRateB { get; set; }
        public decimal CashInTransitBasicPremiumRateC { get; set; }
        public bool IsRSMDT { get; set; }
        public bool IsComprehensive { get; set; }
        public bool IsProrata { get; set; }
        public bool IsShortscale { get; set; }
        [Required(ErrorMessage = "Days is required")]
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int Days { get; set; }
        public string TimeOnly { get; set; }
        public List<MoneyEndorsement> MoneyEndorsements { get; set; }
        public List<MoneyEndorsement> AddedMoneyEndorsements { get; set; }
        public List<MoneyEndorsement> UpdatedMoneyEndorsements { get; set; }
        public List<MoneyEndorsement> DeletedMoneyEndorsements { get; set; }

        public string EndorsementDetails { get; set; }

        public string Deductibles { get; set; }


        public string Name { get; set; }
    }
}