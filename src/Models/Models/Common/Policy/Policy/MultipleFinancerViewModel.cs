using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class MultipleFinancerViewModel
    {
        public string MasterPartyId { get; set; }
        public string Type { get; set; }
        public bool Status { get; set; }
        public string ReferenceId { get; set; }
        public string PartyName { get; set; }
        public string AddressDistrict { get; set; }
    }
}
