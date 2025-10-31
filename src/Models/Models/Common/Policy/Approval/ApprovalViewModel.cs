namespace Models.Common.Policy.Approval
{
    public class ApprovalViewModel
    {
        public string PolicyId { get; set; }
        public bool ApprovalStatus { get; set; }
        public bool EnableBackDateEntry { get; set; }
    }
}
