using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Engineering
{
    public class EEExcessAmountRate
    {
        public string Section { get; set; }
        public decimal SumInsured { get; set; }
        public decimal ExcessAmount { get; set; }
        public decimal ExcessRate { get; set; }
    }
}
