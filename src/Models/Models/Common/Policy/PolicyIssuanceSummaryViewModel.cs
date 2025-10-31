namespace Models.Common.Policy
{
    public class PolicyIssuanceSummaryViewModel
    {
        public int Month { get; set; }
        public string Portfolio { get; set; }
        public int DocumentType { get; set; }
        public decimal Premium { get; set; }
        public int Count { get; set; }
    }
}