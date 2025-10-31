using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class MasterPartyViewModel
    {
        public string Id { get; set; }

        public string PartyCode { get; set; }
        public string PartyType { get; set; }
        public string PartyTypeCode { get; set; }
        public string Status { get; set; }
        public string ReferenceId { get; set; }
        public string History { get; set; }
        public bool IsPaid { get; set; }
    }
}
