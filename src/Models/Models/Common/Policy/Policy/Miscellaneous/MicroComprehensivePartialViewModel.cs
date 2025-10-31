using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class MicroComprehensivePartialViewModel
    {
        public decimal SumInsured { get; set; }
        public decimal BasicPremium { get; set; }
        public bool IsBasicPremiumSelected { get; set; }
        public bool IsVATEnabled { get; set; }
        public bool IsStampSelected { get; set; }
    }
}
