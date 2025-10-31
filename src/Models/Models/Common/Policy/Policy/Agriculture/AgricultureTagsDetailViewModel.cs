namespace Models.Common.Policy.Policy.Agriculture
{
    public class AgricultureTagsDetailViewModel
    {
        public string PolicyIssuanceId { get; set; }
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string IdentificationCodes { get; set; }
        public string RemovedIdentificationCodes { get; set; }
        public int NumberOfIdentificationCodes { get; set; }
        public int NumberOfRemovedIdentificationCodes { get; set; }
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public string Branch { get; set; }
        public string IssueDate { get; set; }
        public string DocumentType { get; set; }
        public string EndorsementType { get; set; }
        public string FiscalYear { get; set; }
        public int SN { get; set; }
    }
}
