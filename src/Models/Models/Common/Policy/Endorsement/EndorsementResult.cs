using Models.Common.Policy.Calculation;

namespace Models.Common.Policy.Endorsement
{
    public class EndorsementResult
    {
        public EndorsementViewModel PreviousClassDetail { get; set; }
        public EndorsementViewModel CurrentClassDetail { get; set; }
        public PremiumCalculationResultModel PremiumCalculationResult { get; set; }

        public string EndorsementMessage { get; set; }


    }
}
