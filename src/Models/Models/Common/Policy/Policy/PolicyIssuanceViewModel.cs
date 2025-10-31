namespace Models.Common.Policy.Policy
{
    public class PolicyIssuanceViewModel
    {
        public string Id { get; set; }
        public int SN { get; set; }
        public string PortfolioId { get; set; }
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string DocNo { get; set; }
        public string BillNo { get; set; }
        public string ReceiptNo { get; set; }
        public string CreditNoteNo { get; set; }
        //Class and Doc Info
        public string Class { get; set; }
        public string ClassId { get; set; }
        public int DocumentType { get; set; }
        public string DocumentTypeText { get; set; }
        public int EndorsementType { get; set; }
        public string EndorsementTypeText { get; set; }
        public string PolicyType { get; set; }
        //Policy Dates
        public string ProposedDate { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
        public string EffectiveDate { get; set; }
        public string EffectiveDateTime { get; set; }
        public string PolicyPeriod { get; set; }
        public int EndorseDays { get; set; }
        public DateTime EndorsementDate { get; set; }
        //Party Info
        public string InsuredParty { get; set; }
        public string InsuredCompanyName { get; set; }
        public string AddressInfo { get; set; }
        public string PhoneNumber { get; set; }
        public int? DraftNo { get; set; }
        public decimal BasicPremiumamount { get; set; }
        public decimal PoolPremiumAmount { get; set; }
        public decimal ThirdPartyPremium { get; set; }
        public decimal SumInsured { get; set; }
        public decimal ThirdPartySumInsured { get; set; }
        public decimal RsmdtSumInsured { get; set; }
        public decimal NetPremium { get; set; }
        public decimal Stampduty { get; set; }
        public string BranchCode { get; set; }
        public string VATBillNo { get; set; }
        public string Agent { get; set; }
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public string InsuredPartyName { get; set; }
        public decimal TotalPremium { get; set; }
        public string RiskTypeSelected { get; set; }
        //DO/AgentInfo
        public string FieldOfficier { get; set; }
        public string Status { get; set; }// Draft/New/Issued
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string PartyType { get; set; }
        public DateTime PolicyEndDate { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsDraftDeleted { get; set; }

        public bool IsInward { get; set; }
        public string PrintedBy { get; set; }
        public DateTime PrintedDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string Remarks { get; set; }
        public bool Language { get; set; }
        public int PolicyStatus { get; set; }

        public string Portfolio { get; set; }
        public decimal Vat { get; set; }
        public decimal VatAmount { get; set; }
        public decimal GrossPremium { get; set; }
        public string BranchDetail { get; set; }
        public string AgentDetail { get; set; }
        public string PartyDetail { get; set; }
        public int ApprovalStatus { get; set; }
        public string FiscalYear { get; set; }

        public string PartyCode { get; set; }

        public decimal TransactionBasicPremium { get; set; }
        public decimal TransactionPoolPremium { get; set; }
        public decimal TransactionThirdPartyPremium { get; set; }
        public decimal TransactionBasicPremum { get; set; }
        public decimal TransactionRSMDST { get; set; }
        public decimal TransactionThirdpartyPreimum { get; set; }
        public decimal TransactionTaxableAmount { get; set; }
        public decimal TransactionTax { get; set; }
        public decimal Transaction { get; set; }
        public decimal TransactionSumInsured { get; set; }
        public bool IsSumInsuredUpdate { get; set; }

        public string OrganizationDetail { get; set; }
        public bool ApplyGovtConfig { get; set; }
        public ClassDetailViewModel ClassDetail { get; set; }

        public string EndorsementReferenancenNumber { get; set; }
        public string EndorsementReferenancenNumberForEnglishPrint { get; set; }

        #region Used In GPA Rescue
        public decimal SIAmountForMedicalBenefitUptoBaseCamp { get; set; }
        public decimal PremiumAmountForMedicalBenefitUptoBaseCamp { get; set; }
        public decimal SIAmountForMedicalBenefitAbove1900ft { get; set; }
        public decimal PremiumAmountForMedicalBenefitAbove1900ft { get; set; }
        #endregion

        public int? AgentCode { get; set; }
        public string DoFoCode { get; set; }
        public string DoFoName { get; set; }
        public decimal AgriculturePAPremium { get; set; }
        public bool IsDeclaration { get; set; }
        public NepaliMonthEnum NepaliMonthEnum { get; set; }

        public string CalculationJson { get; set; }
        public bool IsSpecialAgencyDiscount { get; set; }
        public string CessionId { get; set; }

        public bool IsRIBifurcationSuccess { get; set; }

        public bool IsLatestPolicy { get; set; }

        public string CountInput { get; set; }

        public decimal SubsidyPercentage { get; set; }

        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public bool SelectRenewedPolicies { get; set; }
        public string TechnicianCode { get; set; }
        public string TechnicianDetail { get; set; }
        public string AgricultureFileUploadStatus { get; set; }
        public string UserType { get; set; }
        public bool IsOldEICPolicy { get; set; }
        public bool IsClaimOldEICPolicy { get; set; }
        public bool IsHGIlead { get; set; }

        public int PreviousEndorsementType { get; set; }
        public string UnderwritingApprovedBy { get; set; }

        public string GroupIdForBifurcation { get; set; }

        public bool IsCreditNoteNotApprovable { get; set; }
        public decimal TransactionVat { get; set; }
        public decimal TransactionTaxable { get; set; }


        public string PolicyCounter { get; set; }
        public string DocumentCounter { get; set; }
        public DateTime ExpiryDateTime { get; set; }
        public bool IsNewPolicy { get; set; }

        public bool IsPoolInsurance { get; set; }

        public string BancassuanceBankName { get; set; }
        public string BancassuanceBankBranch { get; set; }
        public string ProjectType { get; set; }

        public bool HasSmartPolicy { get; set; }
        public bool HasExtendedWarrantyPolicy { get; set; }
        public string MasterPolicyNumber { get; set; }
        public bool IsComprehensive { get; set; }
        public AgentPolicyDetailViewModel AgentPolicyDetail { get; set; }
        public List<string> DocumentNumberList { get; set; }
    }
    public enum NepaliMonthEnum
    {
        Baisakh = 1,
        Jestha = 2,
        Ashadh = 3,
        Shrawan = 4,
        Bhadra = 5,
        Ashoj = 6,
        Kartik = 7,
        Mangshir = 8,
        Poush = 9,
        Magh = 10,
        Falgun = 11,
        Chaitra = 12
    }
}
