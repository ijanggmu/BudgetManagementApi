using Models.BeemaEdgeApi.Customer.Policy;

namespace Models.Common.Policy.Endorsement
{
    public class CommonPartyDetailsViewModel
    {
        public CorporateViewModel CorporatePartyDetails { get; set; }
        public ContactViewModel IndividualPartyDetails { get; set; }
        public string ContactType { get; set; }

    }
}
