using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Fire
{
    public class LOPPolicyPartialViewModel
    {
        [Required]
        public decimal AnnualRevenue { get; set; }
        public decimal FullAnnualRevenue { get; set; }
        [Required]
        public decimal InsuredAmount { get; set; }
        public decimal FullInsuredAmount { get; set; }
        [Required]
        public decimal AnnualPremium { get; set; }
        public decimal FullAnnualPremium { get; set; }
        [Required]
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public decimal PolicyPeriodInDays { get; set; }
        public DateTime ProposedDate { get; set; }
        [Required]
        public DateTime EffectiveDate { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsRSMDTSelected { get; set; }
        public decimal? RSMDTPremium { get; set; }
        public decimal FullRSMDTPremium { get; set; }
        public bool IsFirePolicySelected { get; set; }
        public string RiskAddress { get; set; }
        public string LOPEndorsement { get; set; }
        public string SelectedPolicyNumber { get; set; }
        public string Deductibles { get; set; }
        public string MaximumIndemnityPeriod { get; set; }
    }

    public class LOPEndorsementPartialViewModel : LOPPolicyPartialViewModel
    {
        public decimal TransactionBasicPremium { get; set; }
    }
}
