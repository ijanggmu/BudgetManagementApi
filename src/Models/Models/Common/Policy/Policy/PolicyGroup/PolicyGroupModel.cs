using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.PolicyGroup
{
    public class PolicyGroupModel
    {
        public int SN { get; set; }
        public string Id { get; set; }
        public string ClientCode { get; set; }
        public string GpaPolicyNumber { get; set; }
        public string HipPolicyNumber { get; set; }
        public string MedPolicyNumber { get; set; }
        public string CreatedDate { get; set; }
        public bool IsPoliciesMerged { get; set; }
        public string StatusMessage { get; set; }
    }
}
