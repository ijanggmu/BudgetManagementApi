using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.RequestParameter
{
    public class EmployeeSearchRequestModel: CommonRequestModel
    {
        public string PolicyNumber { get; set; }
        public string PartyCode { get; set; }
        public string PolicyClass { get; set; }
        public string DocumentNumber { get; set; }
       
    }
}
