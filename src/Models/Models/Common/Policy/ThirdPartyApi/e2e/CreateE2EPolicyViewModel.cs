using System;

namespace Models.Common.Policy.ThirdPartyApi.e2e
{
    public class CreateE2EPolicyViewModel
    {
        public string DraftNo { get; set; }
        public string Data { get; set; }
        public string PartyCode { get; set; }
        public string PartyId { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime PaidDate { get; set; }
        public string Portfolio { get; set; }
        public string Class { get; set; }
        public bool IsIndividual { get; set; } = true;
        public string Status { get; set; }
    }
}
