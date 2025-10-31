using System;

namespace Models.Common.Policy.e2e
{
    public class PolicyE2EViewModel
    {
        public string Id { get; set; }
        public int SN { get; set; }
        public string Data { get; set; }
        public string PartyCode { get; set; }
        public string Status { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaidDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PartyId { get; set; }
        public string Portfolio { get; set; }
        public string Class { get; set; }
        public bool IsIndividual { get; set; }
        public int DraftNumber { get; set; }
        public string DraftNumberE2E { get; set; }
    }
}
