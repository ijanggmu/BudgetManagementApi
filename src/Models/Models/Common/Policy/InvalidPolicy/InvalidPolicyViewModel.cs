using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.InvalidPolicy
{
    public class InvalidPolicyViewModel
    {
        public int SN { get; set; }
        public string CreatedDate { get; set; }
        public string PolicyNumber { get; set; }
        public int? DraftNumber { get; set; }
        public string DocumentNumber { get; set; }
    }
}
