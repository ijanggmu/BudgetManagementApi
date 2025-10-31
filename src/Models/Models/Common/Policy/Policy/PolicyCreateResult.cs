using Models.Common.Policy.Calculation;

namespace Models.Common.Policy.Policy
{
    public class PolicyCreateResult
    {
        public string Id { get; set; }
        public string PortfolioAlias { get; set; }
        public PremiumCalculationResultModel CalculationDetail { get; set; }
        public PolicyIssuanceViewModel PolicyDetail { get; set; }
        public string PolicyCreateMessage { get; set; }
        public int? ApprovalStatus { get; set; }

        public string PartyCode { get; set; }
        public E2EPolicyLog E2EPolicyLog { get; set; }
    }
    public class E2EPolicyLog
    {
        public string Id { get; set; }
        public string PayLoadJson { get; set; }
        public string Status { get; set; }
        public string DraftNumber { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
