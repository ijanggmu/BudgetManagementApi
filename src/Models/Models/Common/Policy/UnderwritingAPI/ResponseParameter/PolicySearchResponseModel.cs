using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.ResponseParameter
{
    public class PolicySearchResponseModel
    {
        public string PolicyNumber { get; set; }
        public string PolicyClass { get; set; }
        public string PartyCode { get; set; }
        public string PartyName { get; set; }
        public string FiscalYear { get; set; }
        public string DocumentNumber { get; set; }
        public string Type { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
