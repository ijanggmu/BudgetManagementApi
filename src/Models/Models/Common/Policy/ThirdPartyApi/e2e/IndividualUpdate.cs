using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.ThirdPartyApi.e2e
{
    public class IndividualUpdateModel:IndividualCheck
    {
        [Required]
        public string ChangedPhoneNumber { get; set; }
    }
}
