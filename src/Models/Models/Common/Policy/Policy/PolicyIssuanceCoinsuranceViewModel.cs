
namespace Models.Common.Policy.Policy
{
    public class PolicyIssuanceCoinsuranceViewModel
    {
        public int SN { get; set; }
        public string PolicyIssuanceId { get; set; }
        public string ClassId { get; set; }
        public int? DraftNo { get; set; }
        public string CoinsuranceChartOfAccountId { get; set; }
        public string CoinsuranceChartOfAccountName { get; set; }
        public decimal? ShareRate { get; set; }
        public decimal BasicPremium { get; set; }
        public decimal RSMDTPremium { get; set; }
        public decimal TPLPremium { get; set; }
        public decimal VAT { get; set; }
        public decimal NetPremium { get; set; }
        public decimal SumInsured { get; set; }
        public decimal GrossPremium { get; set; }
    }
}