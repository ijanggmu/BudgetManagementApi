using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.RequestParameter
{
    public class PolicySearchRequestModel: CommonRequestModel
    {
        public List<string> PartyCode { get; set; }
        public List<string> PolicyClass { get; set; }
        public List<string> PolicyNumber { get; set; }
       
    }
}
