using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class BifurcateDataProvideViewModel
    {
        public string ClassId { get; set; }
        public string PortfolioSubGroup { get; set; }
        public string ClassGroup { get; set; }
        public string PortfolioName { get; set; }
        public decimal TotalPremium { get; set; }
        public bool IsPortfolio { get; set; }
        public int TotalCount { get; set; }
    }
}
