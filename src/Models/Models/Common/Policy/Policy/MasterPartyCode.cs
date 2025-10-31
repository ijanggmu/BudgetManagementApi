namespace Models.Common.Policy.Policy
{
    public class MasterPartyCode
    {
        public MasterPartyCode(string partyId, string partycode)
        {
            PartyId = partyId;
            PartyCode = partycode;
        }

        public string PartyId { get; set; }
        public string PartyCode { get; set; }
    }
}
