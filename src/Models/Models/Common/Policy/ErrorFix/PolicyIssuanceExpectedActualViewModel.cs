namespace Models.Common.Policy.ErrorFix
{
    public class PolicyIssuanceExpectedActualViewModel
    {
        public PolicyIssuanceExpectedActualViewModel()
        {
            ExpectedPolicyIssuanceErrorFixViewModel = new PolicyIssuanceErrorFixViewModel();
            ActualPolicyIssuanceErrorFixViewModel = new PolicyIssuanceErrorFixViewModel();
        }
        public PolicyIssuanceErrorFixViewModel ExpectedPolicyIssuanceErrorFixViewModel{ get; set; }
        public PolicyIssuanceErrorFixViewModel ActualPolicyIssuanceErrorFixViewModel{ get; set; }
    }
}
