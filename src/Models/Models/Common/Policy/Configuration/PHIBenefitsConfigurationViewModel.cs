using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Configuration
{
   public class PHIBenefitsConfigurationViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Sn { get; set; }
        [Required(ErrorMessage= "Label is required")]
        public string Label { get; set; }
        [Required(ErrorMessage = "Benefits is required")]
        public string Benifits { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

    }
}
