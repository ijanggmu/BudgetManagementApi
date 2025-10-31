using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Enum
{
    public class EndorsementEnum
    {
        public enum EndorsementPayment
        {
            Extra = 1,
            Refund = 2,
            Nill = 3
        }

        public enum EndorsementClassType
        {
            [Display(Name = "Class Edit")]
            ClassEdit = 1,
            [Display(Name = "Asset Change")]
            AssetChange = 2,
            [Display(Name = "Update Party Information")]
            UpdatePartyInformation = 3,
            [Display(Name = "Risk Change")]
            RiskChange = 4,
            [Display(Name = "Date Extend")]
            DateExtend = 5,
            [Display(Name = "Date Reduction")]
            DateReduction = 6,
            [Display(Name = "Ownership Transfer")]
            OwnershipTransfer = 7,
            [Display(Name = "Cancellation")]
            Cancellation = 8,
            [Display(Name = "NCD Recovery/Refund")]
            NCDRecoveryRefund = 9
        }

    }
}
