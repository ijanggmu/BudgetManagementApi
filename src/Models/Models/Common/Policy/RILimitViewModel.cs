using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.ViewModels
{
   public class RILimitViewModel
    {
        public string Id { get; set; }
        public int Sn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string PortfolioAlias { get; set; }
        public string PortfolioName { get; set; }
        public string ClassCode { get; set; }
        [Required]
        public string EffectiveFrom { get; set; }
        [Required]
        public string EffectiveTo { get; set; }
        public string OriginalFrom { get; set; }
        public string OriginalTo { get; set; }
        [Required]
        public decimal LimitAmount { get; set; }
        public bool IsConfigured { get; set; }
        public string ChangeRequestedBy { get; set; }
        public string ApprovedBy { get; set; }
        public int Status { get; set; }
    }
}
