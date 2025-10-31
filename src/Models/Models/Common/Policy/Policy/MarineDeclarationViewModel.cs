using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class MarineDeclarationViewModel
    {
        public int SN { get; set; }
        public string Id { get; set; }
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        [Required(ErrorMessage = "Please Select a Month.")]
        public string Month { get; set; }
        [Required(ErrorMessage = "Please Enter Declared Amount.")]
        //[Remote("DeclarationAmount", "Policy", AdditionalFields = "PolicyNumber,SumInsured")]
        public float DeclaredAmount { get; set; }
        [Required (ErrorMessage ="Remarks For Declaration is Required.")]
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; } = DateTime.UtcNow.ToShortDateString();
        public decimal SumInsured { get; set; }
        public decimal TotalSumInsured { get; set; }
        public string ClassId { get; set; }
        public string UpdatedBy { get; set; }
    }
}
