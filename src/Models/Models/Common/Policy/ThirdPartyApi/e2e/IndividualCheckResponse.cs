using Models.BeemaEdgeApi.Customer.Policy;

namespace Models.Common.Policy.ThirdPartyApi.e2e
{
    public class IndividualCheckResponse
    {
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public ContactViewModel Individual { get; set; }
    }
}
