using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.PolicyGroup
{
    public class PolicyGroupManualMergeModel
    {
        public List<PolicyGroupEmployeeModel> EmployeesToMerge { get; set; }
        public string PolicyGroupid { get; set; }
        public bool UpdatePolicyGroup { get; set; }
    }
}
