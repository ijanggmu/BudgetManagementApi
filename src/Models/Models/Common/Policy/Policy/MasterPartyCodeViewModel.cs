using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class MasterPartyCodeViewModel
    {
        public MasterPartyCodeViewModel(string partyId, string partycode, string masterPartyId)
        {
            PartyId = partyId;
            PartyCode = partycode;
            MasterPartyId = masterPartyId;
        }
        public string PartyId { get; set; }
        public string PartyCode { get; set; }
        public string MasterPartyId { get; set; }
    }
}
