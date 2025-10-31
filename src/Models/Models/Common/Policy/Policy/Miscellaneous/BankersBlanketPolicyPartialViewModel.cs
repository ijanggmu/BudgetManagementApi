using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class BankersBlanketPolicyPartialViewModel
    {
        public int NumberOfEmployees { get; set; }
        public decimal MaximumAmountInATM { get; set; }
        public decimal BasicSumInsured { get; set; }
        public decimal OldBasicSumInsured { get; set; }
        public decimal? AdditionalSumInsuredForClauseA { get; set; }
        public decimal? OldAdditionalSumInsuredForClauseA { get; set; }
        public decimal? AdditionalSumInsuredForClauseB { get; set; }
        public decimal? OldAdditionalSumInsuredForClauseB { get; set; }
        public decimal CashOnCounter { get; set; }
        public decimal OldCashOnCounter { get; set; }
        public decimal TransactionCashOnCounter { get; set; }
        public decimal LimitOnATM { get; set; }
        public decimal OldLimitOnATM { get; set; }
        public decimal TransationLimitOnATM { get; set; }
        public decimal SumInsuredForClauseA { get; set; }
        public decimal SumInsuredForClauseB { get; set; }
        public decimal SumInsuredForClauseC { get; set; }
        public decimal SumInsuredForClauseD { get; set; }
        public decimal FullSumInsured { get; set; }
        public decimal? BasicPremiumRateForAdditionalSIClauseA { get; set; }
        public decimal? BasicPremiumRateForAdditionalSIClauseB { get; set; }
        public BankersBlanketDeductibleDetail DeductibleDetailClauseA { get; set; }
        public BankersBlanketDeductibleDetail DeductibleDetailClauseB { get; set; }
        public BankersBlanketDeductibleDetail DeductibleDetailClauseC { get; set; }
        public BankersBlanketDeductibleDetail DeductibleDetailClauseD { get; set; }
        public BankersBlanketDeductibleDetail DeductibleDetailCashOnCounter { get; set; }
        public BankersBlanketDeductibleDetail DeductibleDetailLimitOnATM { get; set; }
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        [Required]
        public int PolicyPeriodInDays { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsRSMDTSelected { get; set; }
        [Range(0.001,100,ErrorMessage = "Please enter a value within 0 and 100")]
        public decimal? SpecialDiscountRate { get; set; }
        public decimal? LoadingForDishonestyOfEmployees { get; set; }
        public string EndorsementDetails { get; set; }
        public string Deductibles { get; set; }
        public string BankList { get; set; }
        public bool IsAdditionalRisk { get; set; }
        public decimal AdditionalRiskPremium { get; set; }
        public decimal AdditionalRiskRate { get; set; }

    }
}
