using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy
{
    public class PropertySubsidySILimitViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Sn { get; set; }
        [Required(ErrorMessage ="Subsidy Category Name is Required.")]
        public string SILabel { get; set; }
        [Required(ErrorMessage = "Subsidy Category Limit is Required.")]
        public decimal SILimit { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
