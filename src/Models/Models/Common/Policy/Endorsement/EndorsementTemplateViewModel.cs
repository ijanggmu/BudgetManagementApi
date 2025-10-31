using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Endorsement
{
    public class EndorsementTemplateViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Sn { get; set; }

        [Required(ErrorMessage = "Template title is required.")]
        public string TemplateTitle { get; set; }
        [Required(ErrorMessage = "Template description is required.")]
        public string TemplateDescription { get; set; }
        [Required(ErrorMessage = "Endorsement type is required.")]
        public string EndorsementType { get; set; }
        public string EndorsementTypeText { get; set; }
        [Required(ErrorMessage = "Choose a Portfolio.")]
        public string Portfolio { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
