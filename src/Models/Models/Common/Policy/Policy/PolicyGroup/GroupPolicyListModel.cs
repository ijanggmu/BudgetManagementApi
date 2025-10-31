using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.PolicyGroup
{
    public class GroupPolicyListModel
    {
        public List<string> GPAPolicies { get; set; } = new List<string>();
        public List<string> HIPPolicies { get; set; } = new List<string>();
        public List<string> MEDPolicies { get; set; } = new List<string>();
    }
}
