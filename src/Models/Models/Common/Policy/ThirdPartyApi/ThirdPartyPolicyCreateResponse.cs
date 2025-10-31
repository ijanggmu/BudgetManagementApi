using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.ThirdPartyApi
{
    public class ThirdPartyPolicyCreateResponse
    {
        public string Id { get; set; }
        public string draftNo { get; set; }
        public string PortfolioAlias { get; set; }
        public string PartyCode { get; set; }
        public string policyNumber { get; set; }
        public string documentNumber { get; set; }
        public string receiptNumber { get; set; }
        public string invoiceNumber { get; set; }
    }
}
