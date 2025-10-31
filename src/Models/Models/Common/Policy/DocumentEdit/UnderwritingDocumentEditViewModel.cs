using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.DocumentEdit
{
    public class UnderwritingDocumentEditViewModel
    {
        public EditedProperties Before { get; set; }
        public EditedProperties After { get; set; }
    }
    public class EditedProperties
    {
        public decimal SumInsured { get; set; }
        public decimal TransactionSumInsured { get; set; }
        public string RiskTypeSelected { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiryDate { get; set; }
        public string EndorsementDate { get; set; }
        public int EndorsementType { get; set; }
        public bool IsSumInsuredUpdate { get; set; }
        public bool HasSmartPolicy { get; set; }
    }
}
