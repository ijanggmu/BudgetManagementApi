using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Policy
{
    public class PrintPolicyDetailViewModel : PolicyDetailViewModel
    {
        public string UserName { get; set; }
        public string StaffName { get; set; }
        public string UnderwritingApprovedBy { get; set; }
        public bool IsPreview { get; set; }
        public string Preview { get; set; }
        public string InsurerDetails { get; set; }
        public string InsurerDetailsNP { get; set; }
        public string CovidInsuredName { get; set; }
        public string ReceiptDetails { get; set; }
        public string DofoAgent { get; set; }
        public DateTime? FormattedEffectiveDate { get; set; }
        public string PremiumTypeDetails { get; set; }
        public string ConfirmationDetails { get; set; }
        public string OccupationDetails { get; set; }
        public EndorsementListEnum EndorsementType { get; set; }
        public PartyDetailViewModel PartyDetail { get; set; }
        public BranchDetailViewModel BranchDetail { get; set; }
        public AgentPolicyDetailViewModel AgentDetail { get; set; }
        public DateTime InitialPolicyIssueDate { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? ProposedDate { get; set; }
        public bool isNewPolicy { get; set; }
        public string QRCodeUrl { get; set; }
        public string QRCodeString { get; set; }
        public bool isNepaliLanguage { get; set; }
        public string EndorseementReferenceNumberForPrint { get; set; }
        public string OfficialSignatureEnglish { get; set; }
        public string OfficialSignatureNepali { get; set; }
        public string OfficialHeaderNepali { get; set; }
        public string OfficialHeaderEnglish { get; set; }
        public bool isNewMerger { get; set; }
        public string TechnicianContactNumber { get; set; }
        public string IdentificationLabelEnglish { get; set; }
        public string IdentificationLabelNepali { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime? IdentificationIssueDate { get; set; }
        public string IdentificationIssueDistrict { get; set; }
        public string FullAddress { get; set; }
        public string CreditNoteNumber { get; set; }

        //for Tune protect policies endorsement schedule
        public DateTime? DateOfIssue { get; set; }
        public string UnderWriterStamp { get; set; }
        public string UnderWriterSignature { get; set; }
        public string CompanyStamp { get; set; }
        public string Ticket10 { get; set; }
        public string Ticket20 { get; set; }

    }
    public enum EndorsementListEnum
    {
        [Display(Name = "Class Edit")]
        ClassEdit = 1,
        [Display(Name = "Asset Change")]
        AssetChange = 2,
        [Display(Name = "Update Party Information")]
        UpdatePartyInformation = 3,
        [Display(Name = "Risk Change")]
        RiskChange = 4,
        [Display(Name = "Date Extend")]
        DateExtend = 5,
        [Display(Name = "Date Reduction")]
        DateReduction = 6,
        [Display(Name = "Ownership Transfer")]
        OwnershipTransfer = 7,
        [Display(Name = "Cancellation")]
        Cancellation = 8,
        [Display(Name = "NCD Recovery/Refund")]
        NCDRecoveryRefund = 9,
        [Display(Name = "Total Loss Cancellation")]
        TotalLossCancellation = 10,
        [Display(Name = "Cession Adjustment")]
        CessionAdjustment = 11,
        [Display(Name = "Endorsement Cancellation")]
        EndorsementCancellation = 12,
        [Display(Name = "Subsidy Recovery")]
        Subsidy = 13,
        [Display(Name = "Cancellation Without Service Charge")]
        CancellationWithoutServiceCharge = 14,
        [Display(Name = "Date Change (Null)")]
        DateChange = 15,
        [Display(Name = "HIP Unnamed Child Assignment")]
        HIPUnnamedChildAssignment = 16,
        [Display(Name = "Aviation Installment Payment")]
        AviationInstallmentPayment = 17,
        [Display(Name = "Installment Payment")]
        CARInstallmentPayment = 18,
        [Display(Name = "Agriculture Subsidy Refund")]
        AgricultureSubsidyRefund = 19,
        [Display(Name = "Full Cancellation")]
        FullCancellation = 111

    }
    public class BranchDetailViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class AgentPolicyDetailViewModel
    {
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public int? AgentCode { get; set; }
        public decimal AgentCommissionRate { get; set; }
        public decimal TDSPercentage { get; set; }
        public string License { get; set; }
    }

}
