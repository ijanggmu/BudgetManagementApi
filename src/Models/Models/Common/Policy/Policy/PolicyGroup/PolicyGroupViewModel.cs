using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.PolicyGroup
{
    public class PolicyGroupViewModel
    {
        [Required(ErrorMessage = "Please select a party")]
        public string ClientCode { get; set; }
        public string GPAPolicyNumber { get; set; }
        public string HIPPolicyNumber { get; set; }
        public string MEDPolicyNumber { get; set; }
        public string Id { get; set; }
    }
}
