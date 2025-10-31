using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.BeemaEdgeApi.Customer.Policy;
public class BuyPolicyRequestModel
{
    public string PolicyId { get; set; }
    public string PolicyHolder { get; set; }
    public DateTime StartDate { get; set; }
}
