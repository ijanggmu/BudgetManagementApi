using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Risk
{
    public class RiskBaseViewModel
    {
        public string GroupName { get; set; }
        public string Title { get; set; }   
        public decimal? Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal ShortScaleAmount { get; set; }
        public decimal ProRataOrShortScaleAmount { get; set; }
        public decimal Total { get; set; }
    }
}
