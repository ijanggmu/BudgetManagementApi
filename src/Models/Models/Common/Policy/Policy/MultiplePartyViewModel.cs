using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class MultiplePartyViewModel
    {
        public string MasterPartyId { get; set; }
        public string Type { get; set; }
        public bool Status { get; set; }
        public string ReferenceId { get; set; }
        public string PartyName { get; set; }
        public string PartyNameNepali { get; set; }
    }
}
