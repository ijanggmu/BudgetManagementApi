using Models.BeemaEdgeApi.Customer.Policy;

namespace Models.Common.Policy.Policy
{
    public class TPCounterPolicyViewModel
    {
        public ContactViewModel ContactDetails { get; set; }
        public CreatePolicyViewModel PolicyDetails { get; set; }
        public List<MultiplePartyViewModel> MultipleParties { get; set; }
        public List<MultiplePartyViewModel> MultipleCareofs { get; set; }
        public List<MultipleFinancerViewModel> MultipleFinancers { get; set; }
    }
}
