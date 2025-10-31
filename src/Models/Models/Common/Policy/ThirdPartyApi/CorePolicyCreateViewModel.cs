using Models.Common.Policy.Policy;
using Models.BeemaEdgeApi.Customer.Policy;

namespace Models.Common.Policy.ThirdPartyApi
{
    public class CorePolicyCreateViewModel
    {
        public string DraftNo { get; set; }
        public decimal NetPremium { get; set; }
        public decimal PayablePremium { get; set; }
        public string GateWay { get; set; }
        public string TransactionId { get; set; }
        public string StaffEmail { get; set; } = "dia@hei.com.np";
        public string PolicyNumber { get; set; }
        public string User { get; set; } = "c1091cf9-37dc-4331-935c-e9d7e3b920fb";
        public PolicyIssuanceViewModel PolicyIssuanceViewModel { get; set; }
        public ContactViewModel ContactViewModel { get; set; }
        public CreatePolicyViewModel CreatePolicyViewModel { get; set; }
    }
}
