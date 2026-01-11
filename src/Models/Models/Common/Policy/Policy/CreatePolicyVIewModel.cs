using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Models.Common.Policy.Policy.Agriculture;
using Models.Common.Policy.Policy.Aviation;
using Models.Common.Policy.Policy.Engineering;
using Models.Common.Policy.Policy.Fire;
using Models.Common.Policy.Policy.GPA;
using Models.Common.Policy.Policy.Marine;
using Models.Common.Policy.Policy.Medical;
using Models.Common.Policy.Policy.Miscellaneous;
using Models.Common.Policy.Policy.Miscellaneous.MBPI;
using Models.BeemaEdgeApi.Customer.Policy;
using SharedKernel.Attributes;
using Models.Common.Policy.Enum;

namespace Models.Common.Policy.Policy
{
    public class CreatePolicyViewModel
    {
        public ContactViewModel ContactViewModel { get; set; }
        public MBPIPartialViewModel MBPIPartial { get; set; }
        public MBPIEndorsementPartialViewModel MBPIEndorsementPartial { get; set; }
        public MotorPartialViewModel MotorPartial { get; set; }
        public ElectricMotorcyclePartialViewModel ElectricMotorcyclePartial { get; set; }
        public PrivateVehiclePartialViewModel PrivateVehiclePartial { get; set; }
        public ElectricVehicleParialViewModel ElectricVehiclePartial { get; set; }
        public TaxiPartialViewModel TaxiPartial { get; set; }
        public PassengerCarryingVehiclePartialViewModel PassengerCarryingVehiclePartial { get; set; }
        public AmbulancePartialViewModel AmbulancePartial { get; set; }
        public GoodsCarryingVehiclePartialViewModel GoodsCarryingVehiclePartial { get; set; }
        public AgricultureForestryVehiclePartialViewModel AgricultureForestryVehiclePartial { get; set; }
        public TractorPartialViewModel TractorPartial { get; set; }
        public ConstructionEquipmentPartialViewModel ConstructionEquipmentPartial { get; set; }
        public TankerPartialViewModel TankerPartial { get; set; }
        public TempoPartialViewModel TempoPartial { get; set; }
        public ElectricCommercialVehiclePartialViewModel ElectricCommercialVehiclePartial { get; set; }
        public FirePolicyPartialViewModel FirePartial { get; set; }
        public LOPPolicyPartialViewModel LOPPartial { get; set; }
        public GPAPolicyPartialViewModel GPAPartial { get; set; }
        public GPAEndorsementPartialViewModel GPAEndorsementPartial { get; set; }
        public MarinePolicyParitalViewModel MarinePartial { get; set; }
        public MarineMaterialsFileModel MarineMaterials { get; set; }
        public ElectricalEquipmentPolicyPartialViewModel ElectricalEquipmentPartial { get; set; }
        public ConstructionPlantAndMachineryPolicyPartialViewModel ConstructionPlantAndMachineryPartial { get; set; }
        public BoilerPolicyPartialViewModel BoilerPartial { get; set; }
        public BusinessMachineAndEquipmentPolicyPartialViewModel BusinessMachineAndEquipmentPartial { get; set; }
        public BurglaryPartialViewModel BurglaryPartial { get; set; }
        public MoneyPartialViewModel MoneyPartial { get; set; }
        public PAPartialViewModel PAPartial { get; set; }
        public PATPartialViewModel PATPartial { get; set; }
        public FidelityGuaranteePartialViewModel FidelityPartial { get; set; }
        public AllRiskPartialViewModel AllRiskPartial { get; set; }
        public GPATPartialViewModel GPATPartial { get; set; }
        public GPATEndorsementPartialViewModel GPATEndorsementPartial { get; set; }
        public GPARaftingPartialViewModel GPARaftingPartial { get; set; }
        public GPARaftingEndorsementPartialViewModel GPARaftingEndorsementPartial { get; set; }
        public PHIPartialViewModel PHIPartial { get; set; }
        public MicroInsuranceMedicalPartialViewModel MIMPartial { get; set; }

        public GPARPartialViewModel GPARPartial { get; set; }
        public GPAREndorsementPartialViewModel GPAREndorsementPartial { get; set; }
        public MedicalPolicyPartialViewModel MedicalPartial { get; set; }
        public CriticalIllnessPartialViewModel CriticalIllnessPartial { get; set; }
        public CriticalIllnessEndorsementPartialViewModel CriticalIllnessEndorsement { get; set; }
        public HospitalIndemnityPlanPolicyPartialViewModel HospitalIndemnityPlanPartial { get; set; }
        public HIPEndorsementPartialViewModel HIPEndorsementPartial { get; set; }
        public MachineryBreakdownPolicyPartialViewModel MachineryBreakdownPartial { get; set; }
        public ContractorAllRiskPolicyPartialViewModel ContractorAllRiskPartial { get; set; }
        public ErectionAllRiskPartialViewModel ErectionAllRiskPartial { get; set; }
        public BankersBlanketPolicyPartialViewModel BankersBlanketPartial { get; set; }
        public VegetablesPartialViewModel VegetablesPartial { get; set; }
        public CerealsPartialViewModel CerealsPartial { get; set; }
        public GrainPartialViewModel GrainPartial { get; set; }
        public PoultryPartialViewModel PoultryPartial { get; set; }
        public CattlePartialViewModel CattlePartial { get; set; }
        public List<MultiplePartyViewModel> MultipleParties { get; set; }
        public List<MultiplePartyViewModel> MultipleCareofs { get; set; }
        public List<MultipleFinancerViewModel> MultipleFinancers { get; set; }
        public List<PropertyListViewModel> PropertyList { get; set; }
        public PublicLiabilityViewModel PublicLiabilities { get; set; }
        public TravelInsurancePartialViewModel TravelInsurancePartial { get; set; }
        public ProfessionalIndemnityPartialViewModel ProfessionalIndemnityPartial { get; set; }
        public MicroHouseholdPartialViewModel MicroHouseholdPartial { get; set; }
        public MicroComprehensivePartialViewModel MicroComprehensivePartial { get; set; }
        public MicroPAPartialViewModel MicroPAPartial { get; set; }
        public ITIPartialViewModel InternationalTravelInsurancePartial { get; set; }
        public BUPAPartialViewModel BUPAPartial { get; set; }
        public ITIMPPartialViewModel ITIMPPartial { get; set; }

        public TuneProtectTravelAssurancePartialViewModel TPAMPPartial { get; set; }
        public TuneProtectTravelBaggageAssurancePartialViewModel TPBMPPartial { get; set; }
        public TuneProtectTravelCancellationPartialViewModel TPCMPPartial { get; set; }
        public TuneProtectTravelSafePartialViewModel TPSMPPartial { get; set; }
        public TuneProtectTravelVisitAssurancePartialViewModel TPVMPPartial { get; set; }
        public TuneProtectTravelAirArabiaInsurancePartialViewModel TPTAMPPartial { get; set; }

        public DepositorAndLoaneeOfBankPartialViewModel DNLPartial { get; set; }
        public COVIDPartialViewModel COVIDPartial { get; set; }
        public COVIDEndorsementPartialViewModel COVIDEndorsementPartial { get; set; }

        public MarineDeclarationViewModel MarineDeclaration { get; set; }
        public AviationHSLPartialViewModel AviationHSLPartial { get; set; }
        public AviationHallwarPartialViewModel AviationHallwarPartial { get; set; }
        public AviationLOLPartialViewModel AviationLOLPartial { get; set; }

        public TIBPPartialViewModel TIBPPartial { get; set; }
        public ProductLiabilityPartialViewModel PDLPartial { get; set; }
        public TrekkingRescuePartialViewModel TrekkingRescuePartial { get; set; }
        public TREndorsementPartialViewModel TREndorsementPartial { get; set; }
        public MarineCumErectionPartialViewModel MarineCumErectionPartial { get; set; }
        public StudentSafetyInsurancePartialViewModel StudentSafetyInsurancePartial { get; set; }
        public SSIEndorsementPartialViewModel SSIEndorsementPartial { get; set; }
        public CashInTransitPartialViewModel CashInTransitPartial { get; set; }
        public HimalayanEverestOverseasPartialViewModel HimalayanEverestOverseasPartial { get; set; }
        public MasterPolicyGroupPersonalAccident MasterPolicyGroupPersonalAccidentPartial { get; set; }

        public bool IsHospitalIndemnityPlanPartial => HospitalIndemnityPlanPartial != null;

        //agriculture and live stock
        public decimal GovernmentSubsidyRate { get; set; }
        public decimal PAOfInsured { get; set; }
        //end

        public string PortfolioAlias { get; set; }
        public int? DocumentTypeId { get; set; }
        public string BranchCode { get; set; } = "DIA";
        public string BranchMunicipality { get; set; }
        public string PortfolioId { get; set; }

        [Required(ErrorMessage = ("Please select a party"))]
        public virtual string PartyId { get; set; }

        public string TypeOfParty { get; set; }
        public string FiscalYear { get; set; }
        public string PartyCode { get; set; }
        public string PartyName { get; set; }
        public string PortfolioParent { get; set; }
        public string PortfolioName { get; set; }
        public string Class { get; set; }
        public string User { get; set; }
        public bool IsDraft { get; set; }
        public int DraftNumber { get; set; }
        public bool IsPrintPreview { get; set; }

        [Required]
        [DisplayName("Effective From")]
        public DateTime EffectiveDate { get; set; }

        public DateTime ExpiryDate { get; set; }
        public DateTime ExpiryDateBeforeDateExtend { get; set; }

        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int? PolicyPeriodInDays { get; set; }

        public bool IsAgentInvolved { get; set; }
        public string DOFO { get; set; }
        public string DOFOStaffId { get; set; }
        public string Agent { get; set; }
        public int? AgentId { get; set; }
        public string AgentName { get; set; }
        public string DOFOName { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public int? TechnicianCode { get; set; }
        public string TechnicianName { get; set; }
        public string TechnicianNiaCode { get; set; }
        public int level { get; set; }
        public string DOB { get; set; }
        public bool ApplyGovtConfig { get; set; }
        public bool NCDRequired { get; set; }
        public decimal ServiceCharge { get; set; }
        public bool IsNepaliLanguage { get; set; }
        public string VehicleNumber { get; set; }
        public bool IsDeclaration { get; set; }
        public string RiskTypeSelected { get; set; }

        [Required] public DateTime ProposedDate { get; set; }

        public int? EndorsementType { get; set; }
        public string EndorsementPrintMessage { get; set; }
        public decimal? ShortScale { get; set; }
        public bool IsSpecialAgencyDiscount { get; set; }
        public bool IsPoolInsurance { get; set; }
        public bool IsCoinsuranceInward { get; set; }

        public decimal ThirdPartyNetPremium { get; set; }

        //discount
        public decimal BasicDiscountAmount { get; set; }
        public decimal TPLDiscountAmount { get; set; }
        public decimal PoolDiscountAmount { get; set; }
        public decimal GrossDiscountAmount { get; set; }
        public bool IsCoinsurance { get; set; }

        [Required(ErrorMessage = "Please enter Co-Insurance Rate")]
        public decimal HGIShareRate { get; set; }

        public decimal FullSumInsured { get; set; }
        public decimal ExchangeRate { get; set; }
        public bool IsHGILead { get; set; }
        public bool IsTPPolicy { get; set; }
        public bool IsSumInsuredUpdate { get; set; }
        public string CommonText { get; set; }
        public AgriculturePolicyFileViewModel AgriculturePolicyFile { get; set; }

        public List<ShareCompanyList> ShareCompanyLists { get; set; } = new List<ShareCompanyList>();
        public bool NoServiceCharge { get; set; }

        public bool MinimumBasicPremium { get; set; }
        public bool MinimumRSMDT { get; set; }
        public decimal StampDuty { get; set; }
        public decimal MinimumBasicPremiumAmount { get; set; }

        public class ShareCompanyList
        {
            public string SN { get; set; }
            public string CompanyName { get; set; }

            [Required(ErrorMessage = "Please enter Company Share Rate")]

            public string Label { get; set; }

            public decimal? ShareRate { get; set; }
            public bool IsLead { get; set; }
        }

        public string PASubType { get; set; }

        public string DraftNoOfThirdParty { get; set; }
        public bool IsPolicyFromThirdParty { get; set; }
        public bool IsBancassuance { get; set; }

        [Required(ErrorMessage = ("Please select a bank"))]
        public string BancassuanceBankName { get; set; }

        public string BancassuanceBankId { get; set; }

        [Required(ErrorMessage = ("Please enter Bank Branch Name"))]
        public string BancassuanceBankBranch { get; set; }

        public string TimeSpend { get; set; }
        public bool IsNewPolicy { get; set; }
        public string ReturnUrl { get; set; }
        public string ReturnId { get; set; }
        public string UserType { get; set; }

        public bool IsOldPolicyFromEIC { get; set; }
        public string LocalDateTime { get; set; }

        public string GroupIdForBifurcation { get; set; }
        public string CoinsuranceNepaliText1 { get; set; }
        public string CoinsuranceNepaliText2 { get; set; }
        public string CoinsuranceEnglishText1 { get; set; }
        public string CoinsuranceEnglishText2 { get; set; }
        public bool HasSmartPolicy { get; set; }
        public bool HasExtendedWarrantyPolicy { get; set; }
        public string PolicyNumberForAgri { get; set; }
        public string ProjectType { get; set; }
    }

    public class PrivateCarInsuranceModel
    {
        public VehicleType VechileType { get; set; }
        public VechileEngineCapacityCategory EngineCapacity { get; set; }
        public KilowattRange KilloWattRange { get; set; }
        public decimal MarketValue { get; set; }
        public decimal SumInsuredAmount { get; set; }
        public int YearOfRegistration { get; set; }
        public List<int> ExcessOnOwnDamageOptions { get; set; }
        public bool IsDirectBusiness { get; set; }
        public bool CoversRiotOrTerrorism { get; set; }
        public bool IsUsedForPrivateHire { get; set; }
        public NoClaimDiscountType NoClaimDiscount { get; set; }
        public bool PersonalAccidentForPaidDriver { get; set; }
        public bool PersonalAccidentForPassengers { get; set; }
        public bool PersonalAccidentDirectDiscount { get; set; }
        public int SeatingCapacityIncludingDriver { get; set; }
    }

    public class ThirdPartyPrivateCarInsuranceModel
    {
        public VehicleType VechileType { get; set; }
        public VechileEngineCapacityCategory EngineCapacity { get; set; }
        public KilowattRange KilloWattRange { get; set; }
        public int NumberOfDrivers { get; set; }
        public int NumberOfPassengers { get; set; }
        public string ManufactureCompany { get; set; }
        public string Model { get; set; }
        public bool PurchasedNewOld { get; set; }
    }

    public class BikeFullInsuranceModel
    {
        [Required(ErrorMessage = "Vehicle type is required.")]
        public VehicleType VechileType { get; set; }

        [RequiredIf(nameof(VechileType), (int)VehicleType.Fuel, ErrorMessage = "EngineCapacity is required.")]
        public int EngineCapacity { get; set; }

        [RequiredIf(nameof(VechileType), (int)VehicleType.Electric, ErrorMessage = "KilloWattRange is required.")]
        public int KilloWattRange { get; set; }

        [Required(ErrorMessage = "Market Value is required.")]
        public string MarketValue { get; set; }
        public decimal SumInsuredAmount { get; set; }
        public string YearOfRegistrationAD { get; set; }

        [Required(ErrorMessage = "Year Of Registration is required.")]
        public string YearOfRegistrationBS { get; set; }
        public List<int> ExcessOnOwnDamageOptions { get; set; }
        public bool IsDirectBusiness { get; set; }
        public bool CoversRiotOrTerrorism { get; set; }
        public int AgeOfVechile { get; set; }
        public int NoClaimDiscount { get; set; }
        public string ManufactureCompany { get; set; }
        //[Required(ErrorMessage = "Manufacture Year is required.")]
        public string ManufactureYear { get; set; }

        [Required(ErrorMessage = "Model is required.")]
        public string Model { get; set; }
        public DateTime? DateOfPurchase { get; set; }
    }
    public class ThirdPartyBikeInsuranceModel
    {
        public VehicleType VechileType { get; set; }
        [RequiredIf(nameof(VechileType), (int)VehicleType.Fuel, ErrorMessage = "EngineCapacity is required.")]
        public decimal? EngineCapacity { get; set; }

        [RequiredIf(nameof(VechileType), (int)VehicleType.Electric, ErrorMessage = "KilloWattRange is required.")]
        public decimal? KilloWattRange { get; set; }
        public string ManufactureCompany { get; set; } = "Manufacture Company";
        public string Model { get; set; } = "Manufacture Model";
    }
    public enum KilowattRange
    {
        Range0To799 = 49,
        Range799To1199 = 850,
        Range1200Plus = 1221,
    }

    public class CommercialVehicleInsuranceModel
    {
        public VehicleType VechileType { get; set; }
        public CommercialVehicleClassEnum CommercialVehicleClassType { get; set; }
        public KilowattRange KilloWattRange { get; set; }
        public decimal MarketValue { get; set; }
        public decimal SumInsuredAmount { get; set; }

        public VechileEngineCapacityCategory EngineCapacity { get; set; }
        public int YearOfRegistration { get; set; }
        public decimal TrailorValue { get; set; }
        public List<int> ExcessOnOwnDamageOptions { get; set; }
        public bool IsPrivateUse { get; set; }
        public bool CoversRiotOrTerrorism { get; set; }
        public bool IsOwnUse { get; set; }
        public NoClaimDiscountType NoClaimDiscount { get; set; }
        public int SeatingCapacityIncludingDriver { get; set; }
    }

    public class TravelInsuranceRequestModel
    {
        public TravelAgeGroup AgeGroup { get; set; }
        public TravelRegion Region { get; set; }
        public string Country { get; set; }
        public TravelPlan Plan { get; set; }
        public TravelCoverageType CoverageType { get; set; }
        public int TenureInDays { get; set; }
        public string PassportNumber { get; set; }
        public string Phone { get; set; }
        public string Occupation { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNumber { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Municipality { get; set; }
        public string Ward { get; set; }
        public string StreetAddress { get; set; }
    }

    public class InsuranceQuoteResponse
    {
        public PremiumComponent PremiumDetails { get; set; }         // For comprehensive policies
        public ThirdPartyPremiumComponent ThirdPartyDetails { get; set; } // For third-party policies
    }

    public class PremiumComponent
    {
        public decimal OwnDamagePremium { get; set; }
        public decimal RsmdtCover { get; set; }
        public decimal TplPremium { get; set; }
        public decimal PersonalAccidentPremium { get; set; }
        public decimal TotalPremium { get; set; }
        public decimal StampDuty { get; set; }
        public decimal Vat { get; set; }
    }

    public class ThirdPartyPremiumComponent
    {
        public decimal BasePremium { get; set; }
        public decimal Vat { get; set; }
        public decimal StampDuty { get; set; }
        public decimal TotalPremium { get; set; }
    }
    public enum VechileEngineCapacityCategory
    {
        Under1000,
        From1000To1500,
        Above1500
    }

    public enum BikeEngineCapacityRange
    {
        Cc0To149 = 120,
        Cc150To249 = 230,
        Cc250AndAbove = 269
    }

    public enum NoClaimDiscountType
    {
        Year0_0Percent,
        Year1_15Percent,
        Year2_25Percent,
        Year3OrMore_35Percent
    }
    public enum TravelAgeGroup
    {
        Age18To65
    }

    public enum TravelRegion
    {
        AsiaPacific
    }

    public enum TravelCoverageType
    {
        Individual,
        Family
    }

    public enum TravelPlan
    {
        Plan1,
        Plan2,
        Plan3
    }
    public enum RiskType
    {
        SimpleRisk,
        ComplexRisk
    }
    public enum VehicleType
    {
        Fuel,
        Electric
    }
    /// <summary>
    /// Enum to differentiate between Fire Insurance types
    /// </summary>
    public enum FireInsuranceType
    {
        Household,
        Property,
        LossOfProfit
    }

    /// <summary>
    /// Unified Fire Insurance Request Model for Household, Property, and LossOfProfit
    /// Use FireInsuranceType enum to differentiate between the three types
    /// </summary>
    public class FireInsuranceRequestModel
    {
        /// <summary>
        /// Type of Fire Insurance: Household, Property, or LossOfProfit
        /// </summary>
        [Required(ErrorMessage = "Fire insurance type is required")]
        public FireInsuranceType FireInsuranceType { get; set; }

        // Common fields for Household and Property
        /// <summary>
        /// Sum Insured Amount - Required for Household and Property
        /// </summary>
        public decimal? SumInsuredAmount { get; set; }
        
        /// <summary>
        /// Risk Code - Required for Household and Property calculations
        /// </summary>
        public string RiskCode { get; set; }
        
        /// <summary>
        /// Risk Type - Used for Property insurance (SimpleRisk or ComplexRisk)
        /// </summary>
        public RiskType? RiskType { get; set; }
        
        /// <summary>
        /// Include RSMDT coverage - Optional for Household and Property
        /// </summary>
        public bool IsRSMDT { get; set; } = false;
        
        /// <summary>
        /// Building Composition - Optional, defaults to FirstClass
        /// </summary>
        public BuildingComposition BuildingComposition { get; set; } = BuildingComposition.FirstClass;
        
        /// <summary>
        /// Loading Multiplier - Optional, used for second class buildings
        /// </summary>
        public decimal LoadingMultiplier { get; set; } = 0;
        
        /// <summary>
        /// Is Direct Discount Applicable - Used for Property insurance
        /// </summary>
        public bool IsDirectDiscountApplicable { get; set; } = false;
        
        /// <summary>
        /// Provide Government Subsidy - Optional for Property
        /// </summary>
        public bool ProvideSubsidy { get; set; } = false;
        
        /// <summary>
        /// Subsidy Class ID - Required if ProvideSubsidy is true
        /// </summary>
        public string SubsidyClassID { get; set; }
        
        /// <summary>
        /// Subsidy Rate - Required if ProvideSubsidy is true
        /// </summary>
        public decimal SubsidyRate { get; set; } = 0;
        
        /// <summary>
        /// Provide Lockdown Discount - Optional for Property
        /// </summary>
        public bool ProvideLockdownDiscount { get; set; } = false;

        // Content fields for Household and Property
        /// <summary>
        /// Equipment value - Optional content field
        /// </summary>
        public decimal Equipment { get; set; } = 0;
        
        /// <summary>
        /// Raw Materials value - Optional content field
        /// </summary>
        public decimal RawMaterials { get; set; } = 0;
        
        /// <summary>
        /// Work In Progress value - Optional content field
        /// </summary>
        public decimal WorkInProgress { get; set; } = 0;
        
        /// <summary>
        /// Finished Goods value - Optional content field
        /// </summary>
        public decimal FinishedGoods { get; set; } = 0;
        
        /// <summary>
        /// Semi Finished Goods value - Optional content field
        /// </summary>
        public decimal SemiFinishedGoods { get; set; } = 0;
        
        /// <summary>
        /// Money and Jewellery value - Optional content field
        /// </summary>
        public decimal MoneyAndJewellery { get; set; } = 0;
        
        /// <summary>
        /// Furniture Fixture or Fitting value - Optional content field
        /// </summary>
        public decimal FurnitureFixtureOrFitting { get; set; } = 0;
        
        /// <summary>
        /// Other Items value - Optional content field
        /// </summary>
        public decimal OtherItems { get; set; } = 0;
        
        /// <summary>
        /// Art value - Optional content field
        /// </summary>
        public decimal Art { get; set; } = 0;

        // LossOfProfit specific fields
        /// <summary>
        /// Annual Revenue - Required for LossOfProfit
        /// </summary>
        public decimal? AnnualRevenue { get; set; }
        
        /// <summary>
        /// Insured Amount - Required for LossOfProfit
        /// </summary>
        public decimal? InsuredAmount { get; set; }
        
        /// <summary>
        /// Annual Premium - Required for LossOfProfit
        /// </summary>
        public decimal? AnnualPremium { get; set; }
        
        /// <summary>
        /// Policy Period In Days - Required for LossOfProfit (1-365)
        /// </summary>
        public int? PolicyPeriodInDays { get; set; }
        
        /// <summary>
        /// Is RSMDT Selected - Optional for LossOfProfit
        /// </summary>
        public bool IsRSMDTSelected { get; set; } = false;
        
        /// <summary>
        /// RSMDT Premium - Optional for LossOfProfit
        /// </summary>
        public decimal? RSMDTPremium { get; set; }
        
        /// <summary>
        /// Risk Address - Optional for LossOfProfit
        /// </summary>
        public string RiskAddress { get; set; }
        
        /// <summary>
        /// Selected Policy Number - Optional for LossOfProfit
        /// </summary>
        public string SelectedPolicyNumber { get; set; }
        
        /// <summary>
        /// Deductibles - Optional for LossOfProfit
        /// </summary>
        public string Deductibles { get; set; }
        
        /// <summary>
        /// Maximum Indemnity Period - Optional for LossOfProfit
        /// </summary>
        public string MaximumIndemnityPeriod { get; set; }
    }

    public class HomeInsuranceRequestModel
    {
        public decimal SumInsuredAmount { get; set; }
        public string RiskCode { get; set; }
        public bool IsRSMDT { get; set; }
        public BuildingComposition BuildingComposition { get; set; } = BuildingComposition.FirstClass;
        public decimal LoadingMultiplier { get; set; } = 0;
        public decimal Equipment { get; set; } = 0;
        public decimal RawMaterials { get; set; } = 0;
        public decimal WorkInProgress { get; set; } = 0;
        public decimal FinishedGoods { get; set; } = 0;
        public decimal SemiFinishedGoods { get; set; } = 0;
        public decimal MoneyAndJewellery { get; set; } = 0;
        public decimal FurnitureFixtureOrFitting { get; set; } = 0;
        public decimal OtherItems { get; set; } = 0;
        public decimal Art { get; set; } = 0;
    }

    public class PropertyInsuranceRequestModel
    {
        public decimal SumInsuredAmount { get; set; }
        public RiskType RiskType { get; set; }
        public bool IsDirectDiscountApplicable { get; set; }
        public string RiskCode { get; set; }
        public bool IsRSMDT { get; set; }
        public BuildingComposition BuildingComposition { get; set; } = BuildingComposition.FirstClass;
        public decimal LoadingMultiplier { get; set; } = 0;
        public bool ProvideSubsidy { get; set; } = false;
        public string SubsidyClassID { get; set; }
        public decimal SubsidyRate { get; set; } = 0;
        public bool ProvideLockdownDiscount { get; set; } = false;
        public decimal Equipment { get; set; } = 0;
        public decimal RawMaterials { get; set; } = 0;
        public decimal WorkInProgress { get; set; } = 0;
        public decimal FinishedGoods { get; set; } = 0;
        public decimal SemiFinishedGoods { get; set; } = 0;
        public decimal MoneyAndJewellery { get; set; } = 0;
        public decimal FurnitureFixtureOrFitting { get; set; } = 0;
        public decimal OtherItems { get; set; } = 0;
        public decimal Art { get; set; } = 0;
    }
    public class PropertyInsuranceResponseModel
    {
        public decimal ComprehensiveCover { get; set; }   // e.g., 40.00
        public decimal RsdCover { get; set; }             // e.g., 8.00
        public decimal TerrorismCover { get; set; }       // e.g., 2.00
        public decimal TotalTariffAmount { get; set; }    // e.g., 50.00
        public decimal DirectDiscount { get; set; }       // e.g., 47.50
        public decimal StampDuty { get; set; }             // e.g., 20.00
        public decimal Vat { get; set; }                    // e.g., 6.17
    }
    public class MarineInsuranceRequestModel
    {
        public MarineType MarineType { get; set; }                // e.g., Leather and Leather Goods (Finished Goods)
        public decimal InvoiceValueNpr { get; set; }              // e.g., 259600
        public decimal TolerancePercent { get; set; }             // e.g., 0
        public bool IsIncremental10Percent { get; set; }          // true = Yes, false = No
        public decimal DutyPercent { get; set; }                   // e.g., 0
        public Currency Currency { get; set; }                     // e.g., Nepalese Rupee
        public TransitDiscount TransitDiscount { get; set; }      // e.g., Air Transit Discount (20%)
        public SrccPool SrccPool { get; set; }                     // e.g., Inland Only (0.02%)
        public VolumeDiscount VolumeDiscount { get; set; }         // e.g., None
        public bool HasContainerDiscount { get; set; }             // true = Yes, false = No
        public bool HasNonDelivery { get; set; }                    // true = Yes, false = No
        public bool HasWaterDamage { get; set; }                    // true = Yes, false = No
        public bool HasTpnd { get; set; }                           // true = Yes, false = No
        public bool HasDirectDiscount { get; set; }                 // true = Yes, false = No
    }

    public enum MarineType
    {
        LeatherAndLeatherGoodsFinishedGoods,
        // Add other marine types as needed
    }

    public enum Currency
    {
        //NepaleseRupee,
        NPR
        // Add other currencies if applicable
    }

    public enum TransitDiscount
    {
        None,
        AirTransitDiscount20Percent
        // Add other discount types as needed
    }

    public enum SrccPool
    {
        None,
        InlandOnly_0_02Percent
        // Add other SRCC types as needed
    }

    public enum VolumeDiscount
    {
        None,
        LargeVolumeDiscount20Percent
        // Add other volume discount types
    }


    public class MarineInsurancePremiumResponse
    {
        public decimal SumInsured { get; set; }                   // e.g., 285560.00
        public decimal BasicPremium { get; set; }                 // e.g., 571.12
        public decimal TransitDiscount { get; set; }              // e.g., 114.22
        public decimal ContainerDiscount { get; set; }            // e.g., 0.00
        public decimal SrccPool { get; set; }                      // e.g., 57.11
        public decimal NonDelivery { get; set; }                   // e.g., 0.00
        public decimal WaterDamage { get; set; }                   // e.g., 0.00
        public decimal Tpnd { get; set; }                          // e.g., 0.00
        public decimal DirectDiscount { get; set; }                // e.g., 0.00
        public decimal SubTotal { get; set; }                       // e.g., 514.01
        public decimal LargeVolumeDiscount { get; set; }           // e.g., 0.00
        public decimal Vat { get; set; }                   // e.g., 66.82
        public decimal StampDuty { get; set; }                      // e.g., 20.00
        public decimal GrandTotal { get; set; }                     // e.g., 600.83

    }


    public enum ITITripType //insurance trip type in partial view
    {
        [Display(Name = "Single Trip")]
        SingleTrip = 1,
        [Display(Name = "Annual Trip")]
        AnnualTrip = 2,
        [Display(Name = "Return Trip")]
        ReturnTrip = 3
    }
    public enum ITIPlanType //insurance type in partial view
    {
        [Display(Name = "Plan 1")]
        PlanOne = 1,
        [Display(Name = "Plan 2")]
        PlanTwo = 2
    }
    public enum ITIInsuranceType //type of insured in partial view
    {
        [Display(Name = "Individual")]
        Individual = 1,
        [Display(Name = "Family")]
        Family = 2,
        [Display(Name = "Group")]
        Group = 3
    }
}



