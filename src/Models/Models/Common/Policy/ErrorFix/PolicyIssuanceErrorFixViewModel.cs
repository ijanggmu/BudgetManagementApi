using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.ErrorFix
{
    public class PolicyIssuanceErrorFixViewModel
    {
        public string Portfolio { get; set; }
        public int? DraftNumber { get; set; }
        
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}", ApplyFormatInEditMode = true)]
        public decimal TransactionBasicPremium { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}", ApplyFormatInEditMode = true)]
        public decimal TransactionThirdParty { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}", ApplyFormatInEditMode = true)]
        public decimal TransactionRSMDST { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}", ApplyFormatInEditMode = true)]
        public decimal TransactionGrossPremium { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}", ApplyFormatInEditMode = true)]
        public decimal TransactionStampDuty { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}", ApplyFormatInEditMode = true)]
        public decimal TransactionTaxable { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}", ApplyFormatInEditMode = true)]
        public decimal TransactionTax { get; set; }
        [DisplayFormat(DataFormatString = "{0:F4}", ApplyFormatInEditMode = true)]
        public decimal Transaction { get; set; }



        public bool HasError { get; set; }
        public bool HasMultipleRows { get; set; }
        public decimal BasicPremium { get; set; }
        public decimal ThirdParty { get; set; }
        public decimal RSMDST { get; set; }
        public decimal AgricultureGovernmantSubsidy { get; set; }
        public decimal AgriculturePaAmount  { get; set; }
        public decimal GrossPremium { get; set; }
        public decimal Tax { get; set; }
        public decimal NetPremium { get; set; }
    }
}
