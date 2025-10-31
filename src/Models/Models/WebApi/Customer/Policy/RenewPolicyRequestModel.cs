using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.BeemaEdgeApi.Customer.Policy;
public class RenewPolicyRequestModel
{
    public string PolicyId { get; set; }
    public DateTime NewEndDate { get; set; }
}
