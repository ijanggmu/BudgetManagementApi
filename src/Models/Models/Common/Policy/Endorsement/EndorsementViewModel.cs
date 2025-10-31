using System.ComponentModel.DataAnnotations;
using Models.Common.Policy.Calculation;
using Models.Common.Policy.Policy;
using Models.Common.Policy.Policy.Agriculture;
using Models.Common.Policy.Policy.Fire;
using Models.Common.Policy.Policy.GPA;
using Models.Common.Policy.Policy.Medical;
using Models.Common.Policy.Policy.Miscellaneous;

namespace Models.Common.Policy.Endorsement
{
    public class EndorsementViewModel : CreatePolicyViewModel
    {
        public CommonPartyDetailsViewModel PartyDetail { get; set; }
        public GPAEndorsementPartialViewModel GPAEndorsementPartial { get; set; }
        public GPARaftingEndorsementPartialViewModel GPARaftingEndorsementPartial { get; set; }
        public GPAREndorsementPartialViewModel GPAREndorsementPartial { get; set; }
        //  public GPAPolicyPartialViewModel GPAPartial { get; set; }
        public HIPEndorsementPartialViewModel HIPEndorsementPartial { get; set; }
        public MedicalEndorsementPartialViewModel MedicalEndorsementPartial { get; set; }
        public GPATEndorsementPartialViewModel GPATEndorsementPartial { get; set; }
        public CattleEndorsementPartialViewModel CattleEndorsementPartial { get; set; }
        public CerealEndorsementPartialViewModel CerealEndorsementPartial { get; set; }
        public VegetableEndorsementPartialViewModel VegetableEndorsementPartial { get; set; }
        public PoultryEndorsementPartialViewModel PoultryEndorsementPartial { get; set; }
        public MicroHouseHoldEndorsementPartialViewModel MicroHouseHoldEndorsementPartial { get; set; }
        public MicroPAEndorsementPartialViewModel MicroPAEndorsementPartial { get; set; }
        public MicroInsuranceMedicalEndorsePartialViewModel MicroMedicalEndorsementPartial { get; set; }
        public FidelityGuaranteeEndorsePartialViewModel FidelityGuaranteeEndorsementPartial { get; set; }
        public ProfessionalIndemnityEndorsementPartialViewModel ProfessionalIndemnityEndorsementPartial { get; set; }
        public LOPEndorsementPartialViewModel LOPEndorsementPartial { get; set; }
        //public FirePolicyEndorsementPartialViewModel FirePolicyEndorsementPartial { get; set; }
        public List<PolicyIssuanceViewModel> PolicyIssuanceViewModel { get; set; }
        public PremiumCalculationResultModel PreviousCalculation { get; set; }
        public RiskCovers PreviousRiskCovers { get; set; }
        public string policyId { get; set; }
        [RegularExpression(@"[A-Z0-9_/\-]*", ErrorMessage = "Policy Number cannot have spaces and must have letters in uppercase only.")]
        public string PolicyNumber { get; set; }
        public int DraftNo { get; set; }
        public string DocumentNumber { get; set; }
        public EndorsementListEnum EndorsementType { get; set; }
        public int EndorsementTypeInt { get; set; }
        public int? ExtendedDays { get; set; }
        public int? ReductionDays { get; set; }
        public int Level { get; set; }
        public decimal BasicPremium { get; set; }
        public decimal SumInsured { get; set; }
        public decimal ThirdPartyPremium { get; set; }
        public decimal RSMDPremium { get; set; }
        public decimal TerriorismPremium { get; set; }
        public decimal NetPremium { get; set; }
        public decimal GrossPremium { get; set; }
        public decimal VatAmount { get; set; }
        public bool IsCalculationFromUI { get; set; }
        public string EndorsementNCDMessage { get; set; }
        public string NCD { get; set; }
        public int NCDYear { get; set; }
        public PremiumCalculationResultModel PremiumCalculation { get; set; }
        public decimal FuturePremium { get; set; }
        public decimal FutureSumInsured { get; set; }
        public DateTime EndorsementDate { get; set; }
        public DateTime IssueDate { get; set; }

        public bool IsClassDetailJSONEmpty { get; set; }
        public bool IsFirstAssetChange { get; set; }
        public bool IsIncompleteEndorsement { get; set; }
        public decimal ExchangeRate { get; set; }
        public bool IsExpiredPortfolioEndorsable { get; set; }
        public bool PendingEndorsement { get; set; }
        public decimal Transaction { get; set; }
        public bool IsSumInsuredUpdate { get; set; }
    }

    public class RiskCovers
    {
        public bool IsDPremiumSelected { get; set; }
        public bool IsEPremiumSelected { get; set; }
        public bool IsRSMDTPremiumSelected { get; set; }
    }
    public class FullCancellationEndorsementVm
    {
        public string PolicyNumber { get; set; }
        public string PolicyId { get; set; }
        public string BranchCode { get; set; }
        public int DocumentTypeId { get; set; }
        public string User { get; set; }
        public string PortfolioAlias { get; set; }
        public int Level { get; set; }
        public string Class { get; set; }
        public string ClassId { get; set; }
        public string UserType { get; set; }
    }
}
