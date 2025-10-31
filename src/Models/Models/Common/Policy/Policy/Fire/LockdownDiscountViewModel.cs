using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Fire
{
    public class LockdownDiscountViewModel
    {
        public decimal BasicLockdownDiscountAmount { get; set; }
        public decimal RSMDTLockdownDiscountAmount { get; set; }
        public string Identifier { get; set; }
    }
}
