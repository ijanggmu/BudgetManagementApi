using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.WebApi.Policy;
public class ThirdPartyPolicyResponseModel
{
    public string Id { get; set; }
    public string draftNo { get; set; }
    public string PortfolioAlias { get; set; }
    public string PartyCode { get; set; }
    public string policyNumber { get; set; }
    public string documentNumber { get; set; }
    public string receiptNumber { get; set; }
    public string invoiceNumber { get; set; }
}
