using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Models.Common.Policy.Policy
{
    public class OldEICPolicyTempViewModel
    {
        public string Id { get; set; }
        
        public int DraftNo {get;set;}
        
        [Required(ErrorMessage = "Policy Number is required")]
        [Remote("ExistedPolicyNumberValidation", "OldEICPolicyDraft", HttpMethod = "POST", AdditionalFields = "policyNumber")]
        public string PolicyNumber { get; set; }
        public string ClassId { get; set; }
        public string InsuredPartyName { get; set; }
        
        [Required(ErrorMessage = "Fiscal Year is required")]

        public string FiscalYear { get; set; }
        
        [Required(ErrorMessage = "Effective Date is required")]
        public DateTime EffectiveDate { get; set; }
        
        [Required(ErrorMessage = "Expiry Date is required")]
        public DateTime ExpiryDate { get; set; }
    }
}