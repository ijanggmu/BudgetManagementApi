namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class MasterPolicyGroupPersonalAccident:CommonRiskTypePartialViewModel
    {
        public decimal BasicPremium { get; set; }
        public decimal RSMDTPremium { get; set; }
        public decimal TPLPremium { get; set; }
        public decimal Suminsured { get; set; }
        public int PolicyPeriodInDays { get; set; }
        public decimal Vatpercent { get; set; }
        public decimal StampAmount { get; set; }
        public decimal EndorsedTransactionBasicPremium { get; set; }
        public decimal EndorsedTransactionRSMDTPremium { get; set; }
        public decimal EndorsedTransactionTPLPremium { get; set; }
        public decimal EndorsedTransactionSuminsured { get; set; }
    }
}