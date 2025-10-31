using System.Collections.Generic;

namespace Models.Common.Policy.MarketingSalesReport
{
    public class BranchUnderArea
    {
        public string AreaCode { get; set; }
        public decimal DailyTarget { get; set; }
        public decimal MonthlyTarget { get; set; }
        public List<string> Branches { get; set; }
        public string BranchCode { get; set; }
    }
}