using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.ResponseParameter
{
    public class PartySearchResponseModel
    {
        public string PartyName { get; set; }
        public string PanNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string PartyCode { get; set; }
        public string Id { get; set; }
    }
}
