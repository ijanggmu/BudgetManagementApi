using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.RequestParameter
{
   public class PartySearchRequestModel : CommonRequestModel
    {
        public string PartyName { get; set; }
        public string PanNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string PartyCode { get; set; }
       
    }
}
