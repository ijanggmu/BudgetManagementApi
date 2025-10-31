using Models.BeemaEdgeApi.Customer.Policy;

namespace Models.Common.Policy.Policy
{
    public class IndividualCheckResponseModel
    {
        public bool Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public ContactViewModel Individual { get; set; }
    }
}



