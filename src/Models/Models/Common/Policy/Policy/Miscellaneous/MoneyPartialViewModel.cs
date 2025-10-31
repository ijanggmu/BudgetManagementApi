using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class MoneyPartialViewModel : ILocation
    {
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
        
        //these are added to address requirement modification ,i.e Case in counter addition.
        public decimal CashInSafeBasicPremiumRate { get; set; }
        public decimal CashInCounterBasicPremiumRate { get; set; }
        public decimal CashInTransitBasicPremiumRate { get; set; }
        public decimal IncludePropertyBasicPremiumRate { get; set; }
        
        public decimal? CashInCounter { get; set; }
        public decimal? CashInTransit { get; set; }
        public decimal? CashInSafe { get; set; }

        public bool IsRSMDT { get; set; }
        public bool IsPropertyIncluded { get; set; }
        public bool IsComprehensive { get; set; }
        [Required(ErrorMessage = "Days is required")]
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int Days { get; set; }
        public List<MoneyEndorsement> MoneyEndorsements { get; set; }
        public List<MoneyEndorsement> AddedMoneyEndorsements { get; set; }
        public List<MoneyEndorsement> UpdatedMoneyEndorsements { get; set; }
        public List<MoneyEndorsement> DeletedMoneyEndorsements { get; set; }
        public List<MoneyInBank> AddedMoneyInBankDetails { get; set; }
        public List<MoneyInBank> UpdatedMoneyInBankDetails { get; set; }
        public List<MoneyInBank> DeletedMoneyInBankDetails { get; set; }
        public List<MoneyInBank> MoneyInBankDetails { get; set; }

        public string Deductibles { get; set; }

        public string[] Locations { get; set; }

        public string Name { get; set; }
    }

    public class MoneyEndorsement
    {
        public string PrimaryId { get; set; }
        public string Detail { get; set; }
        public decimal Value { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsDeleted { get; set; }

    }
    public class MoneyInBank
    {
        public string Id { get; set; }
        public string SN { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string CashInCounter { get; set; }
        public string CashInTransit { get; set; }
        public string CashInSafe { get; set; }
        public bool TotalSumInsured { get; set; }


    }
}
