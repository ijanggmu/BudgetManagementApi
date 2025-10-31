using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Policy
{
    public class PolicyDetailRequestViewModel
    {
        public string PolicyNumber { get; set; }
        public string DocumentNummber { get; set; }
        public string ReinsuranceId { get; set; }
    }
}
