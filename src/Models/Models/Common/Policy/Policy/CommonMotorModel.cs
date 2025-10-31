using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SharedKernel.Attributes;

namespace Models.Common.Policy.Policy
{
    public class CommonMotorModel
    {
        public string AgeForPrint { get; set; }
        public string PartyId { get; set; }
        public string PortfolioId { get; set; }
        public string TypeOfVehicle { get; set; }
        public decimal? CompulsoryExcess { get; set; }
        public decimal? TotalExcess { get; set; }
        public bool IsVehicleDutyFree { get; set; }
        public string Type { get; set; }
        public string AgeForPrintEnglish { get; set; }
        public bool IsLayup { get; set; }
        public bool IsDirectDiscountPA { get; set; } = false;
        public DateTime LayupDaysStart { get; set; }
        public DateTime LayupDaysEnd { get; set; }
        public int LayupDays { get; set; }

        public string ManufactureYear { get; set; }
        [Required(ErrorMessage = "Please enter a manufacturer company")]
        public string ManufactureCompany { get; set; }
        [Required(ErrorMessage = "Please enter a model")]
        public string Model { get; set; }
        public string SubModel { get; set; }
        public bool PurchasedNewOld { get; set; }//cehange datatype to string
        [RequiredIf("PurchasedNewOld", true, ErrorMessage = "Please enter the date you purchased the vehicle")]
        public DateTime? DateOfPurchase { get; set; }
        [Required(ErrorMessage = "Please enter chasis number")]
        public string ChasisNumber { get; set; }
        [Required(ErrorMessage = "Please enter engine number")]
        public string EngineNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string RegistrationNumberNepali { get; set; }
        public string ProposerName { get; set; }
        public RiskEnum TypeOfInsurance { get; set; }
        [Required(ErrorMessage = "Please enter the cubic capacity(CC)")]
        public decimal CubicCapacity { get; set; }
        [Required(ErrorMessage = "Please enter the cubic capacity(CC)")]
        public decimal? CCForPrint { get; set; }
        [Required(ErrorMessage = "Please enter voluntary excess")]
        public decimal VoluntaryExcess { get; set; }
        public bool RiotStrikeAndTerrorism { get; set; }
        [RequiredIf("PurchasedNewOld", false, ErrorMessage = "Please enter the date you registered the vehicle")]
        public string YearsFromRegistrationDateYearsBS { get; set; }
        public DateTime YearsFromRegistrationDateYears { get; set; }
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,4})?", ErrorMessage = ("Must be a number with 4 decimal points and maximum length of 25"))]
        public string CurrentMarketPrice { get; set; }
        public decimal? AgeOfVehicle { get; set; }
        public bool IsHelperInsured { get; set; }
        public decimal? RateOfDepreciation { get; set; }
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,4})?", ErrorMessage = ("Must be a number with 4 decimal points and maximum length of 25"))]
        public decimal? ValueOfAccessories { get; set; }
        public decimal ValueWithoutAccessories { get; set; }
        [RegularExpression("[0-9]{0,13}[.]?([0-9]{1,4})?", ErrorMessage = ("Must be a number with 4 decimal points and maximum length of 15"))]
        public decimal SumInsuredAmount { get; set; }
        //Additional Fields
        public string VehicleForHireOrReward { get; set; }
        public string ParkingPlaceGarage { get; set; }
        public string ParkingGarageOpen { get; set; }
        public string Maintenance { get; set; }
        public string PurposedVehicleUsedOtherThanThePurposer { get; set; }
        public PurposerDetailViewModel PurposerDetailViewModel { get; set; }
        public string AnyDisabilityOfEyeOrEarOfDriverCrimeAccustaion { get; set; }
        public string AnyOtherInsuranceProposedVehicle { get; set; }      ///  bool datatype
        public string InsuranceCompanyName { get; set; }
        public bool IsEntitledForNoClaimDiscountFromOtherInsuranceCompany { get; set; }
        public string EntitledForNoClaimDiscountNCDFromOtherInsuranceCompany { get; set; }   ///  bool datatype
        public string RenewalNoticeNCD { get; set; }
        public string HasAnyComputerOrInsurer { get; set; }
        public string AccidentOrLossInThreeYears { get; set; }
        public string HasProposersOrAnyOtherPersonsDrivingLicenseEverBeenCancelled { get; set; }   /// bool datatype
        public string CopyOfPolicyIfOtherVehicleAreInsuredInThisCompany { get; set; }
        public string RiskType { get; set; }

        public bool IsIssued { get; set; }
        public bool IsAgentInvolved { get; set; }
        public string AccessoriesDetail { get; set; }
        public int PreviousPolicyIssuedYear { get; set; }//have to check from db. 
        public int NCDYears { get; set; }
        public bool IsProRataOrShortScale { get; set; }
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        [RegularExpression("[0-9]{1,3}", ErrorMessage = "Please enter a value less than or equal to 365")]
        [Required(ErrorMessage = "Please enter number of days")]
        public string Days { get; set; }

        public bool IsComprehensive { get; set; }
        public bool IsRiotStrike { get; set; }
        public bool IsThirdParty { get; set; } = true;
        public bool EnterSumInsured { get; set; }
        [RequiredIf("IsHelperInsured", true, ErrorMessage = "Please enter the number of helpers")]
        public int? NumberOfHelpers { get; set; }
        public bool ISRecoveryCharge { get; set; }
        public string MasterPolicyNumber { get; set; }
    }
    public enum RiskEnum
    {
        [Display(Name = "OWN DAMAGE PREMIUM")]
        Comprehensive,
        [Display(Name = "Third Party")]
        ThirdParty,
        [Display(Name = "RSMDT")]
        RSMDT
    }
}
