using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    //TPBMP
    public class TuneProtectTravelBaggageAssurancePartialViewModel
    {
        [Range(1, 366, ErrorMessage = "Please enter a value less than or equal to 366")]
        [RegularExpression("[0-9]{1,3}", ErrorMessage = "Please enter a value less than or equal to 366")]
        [Required(ErrorMessage = "Days is required")]
        public int PolicyPeriodInDays { get; set; }

        [Required(ErrorMessage = "Premium Amount is required")]
        public decimal PremiumAmount { get; set; }

        public string InsuranceType { get; set; }

        [Required(ErrorMessage = "Number of insured members is required.")]
        public int InsuredMembers { get; set; }

        public string Occupation { get; set; }
        public bool IsVatEnabled { get; set; }
        public decimal TransactionPremium { get; set; }
    }
}
