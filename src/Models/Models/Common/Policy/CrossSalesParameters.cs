using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class CrossSalesParameters
    {
        public string MasterPartyId { get; set; }
        public string PolicyNumber { get; set; }
        public int DocumentType { get; set; }
        public DateTime IssueDate { get; set; }
    }
}
