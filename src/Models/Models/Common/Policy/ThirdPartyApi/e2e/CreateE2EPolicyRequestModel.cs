using System;

namespace Models.Common.Policy.ThirdPartyApi.e2e
{
    public class CreateE2EPolicyRequestModel
    {
        public string Data { get; set; }
        public string PartyCode { get; set; }       
        public decimal PaidAmount { get; set; }
        public DateTime PaidDate { get; set; }
        public string Portfolio { get; set; }
        public string Class { get; set; }       
    }
}
