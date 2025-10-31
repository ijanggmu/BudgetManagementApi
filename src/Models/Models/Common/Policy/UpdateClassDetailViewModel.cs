using Models.Common.Policy.Policy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class UpdateClassDetailViewModel
    {
        public string PolicyId { get; set; }
        public CreatePolicyViewModel ClassDetail { get; set; }
    }
}
