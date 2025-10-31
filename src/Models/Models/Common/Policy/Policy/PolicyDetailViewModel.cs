using Models.Common.Policy.Calculation;
using Models.Common.Policy;
using Models.BeemaEdgeApi.Customer.Policy;

namespace Models.Common.Policy.Policy
{
    public class PolicyDetailViewModel
    {
        public PolicyIssuanceViewModel PolicyIssuance { get; set; }
        public CreatePolicyViewModel ClassDetail { get; set; }
        public PremiumCalculationResultModel CalculationDetail { get; set; }
        public ContactViewModel Individual { get; set; }
        public CorporateViewModel Corporate { get; set; }

        public List<PolicyIssuanceCoinsuranceViewModel> PolicyIssuanceCoinsurance { get; set; } =
            new List<PolicyIssuanceCoinsuranceViewModel>();
    }
}
