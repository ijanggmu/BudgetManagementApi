using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;

namespace Models.Common.Policy.Approval
{
    public class ApprovalDetailViewModel
    {
        public PolicyIssuanceViewModel PolicyIssuance { get; set; }
        public CreatePolicyViewModel ClassDetail { get; set; }
        public PremiumCalculationResultModel CalculationDetail { get; set; }
        public string PolicyId { get; set; }
        public int? DraftNo { get; set; }
        public bool ApprovalStatus { get; set; }
        public bool EnableBackDateEntry { get; set; }
        public ApprovalContactViewModel Individual { get; set; }
        public ApprovalCorporateViewModel Corporate { get; set; }
        public decimal BasicPremiumamount { get; set; }
        public string UserId { get; set; }
        public string RoleLevel { get; set; }
    }
}
