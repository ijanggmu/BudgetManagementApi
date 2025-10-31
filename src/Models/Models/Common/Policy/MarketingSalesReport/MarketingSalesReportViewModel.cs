namespace Models.Common.Policy.MarketingSalesReport
{
    public class MarketingSalesReportViewModel
    {
        public int FoCode { get; set; }
        public string FoName { get; set; }
        public decimal MonthlyCollection { get; set; }
        public decimal MonthlyTarget { get; set; }
        public decimal DailyTarget { get; set; }
        public decimal AchievementPercent { get; set; }
        public decimal DailyCollection { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        
    }
}