using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;

namespace Models.Common.Policy.Endorsement
{
    public class PrintEndorsementDetailViewModel
    {
        public string UserName { get; set; }
        public PolicyIssuanceViewModel PolicyIssuance { get; set; }
        public EndorsementViewModel ClassDetail { get; set; }
        public PremiumCalculationResultModel CalculationDetail { get; set; }
        public PartyDetailViewModel PartyDetail { get; set; }
    }
}
