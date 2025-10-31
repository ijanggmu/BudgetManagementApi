namespace Models.Common.Policy.Policy
{
    public class FiscalYearViewModel
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string YearOFOperation { get; set; }
        public string ReportYear { get; set; }
        public bool IsSelected { get; set; }

    }
}
