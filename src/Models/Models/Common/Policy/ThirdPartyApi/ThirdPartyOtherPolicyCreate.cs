using Models.BeemaEdgeApi.Customer.Policy;

namespace Models.Common.Policy.ThirdPartyApi
{
    public class ThirdPartyOtherPolicyCreate
    {
        public ContactViewModel ContactViewModel { get; set; }
        public string DraftNo { get; set; }
        public string PartyCode { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime PaidDate { get; set; }
        public string Portfolio { get; set; }
        public string Class { get; set; }


    }
}
