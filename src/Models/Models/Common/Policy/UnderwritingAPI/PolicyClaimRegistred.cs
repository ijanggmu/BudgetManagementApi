using System;

namespace Models.Common.Policy.UnderwritingAPI
{
    public class PolicyClaimRegistred
    {
        public bool IsClaimExists { get; set; }
        public DateTime ClaimRegisteredDate { get; set; }
    }
}