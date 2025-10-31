using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy.Fire
{
    public class FireRiskConfiguration
    {
        public string Id { get; set; }
        [Required]
        public string RateCode { get; set; }
        
        [Required]
        public string RiskType { get; set; }
        
        [Required]
        public string RiskCode { get; set; }
        
        [Required]
        public string PropertyDescription { get; set; }
        
        [Required]
        public decimal Rate { get; set; }
        public int SN { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
    public class GpaPlayerRiskConfiguration
    {
        public string GameCode { get; set; }
        public string RateCode { get; set; }
        public string RiskType { get; set; }
        public string GameDescription { get; set; }
    }
}
