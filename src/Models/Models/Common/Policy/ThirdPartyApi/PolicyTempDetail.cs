using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.ThirdPartyApi
{
    public class PolicyTempDetail
    {
        public int? DraftNo { get; set; }
        public string DraftNoFromThirdParty { get; set; }
        public string PartyCode { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}
