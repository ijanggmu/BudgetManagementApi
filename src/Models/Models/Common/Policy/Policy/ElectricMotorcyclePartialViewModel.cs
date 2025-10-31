using System.ComponentModel.DataAnnotations;
using SharedKernel.Attributes;

namespace Models.Common.Policy.Policy
{
    public class ElectricMotorcyclePartialViewModel
    {
        public bool IsComprehensive { get; set; }
        public bool IsThirdParty { get; set; }
        public bool EnterSumInsured { get; set; }
        public bool IsProRataOrShortScale { get; set; }
        [Range(1, 365, ErrorMessage = "Please enter a value less than or equal to 365")]
        public int Days { get; set; }
        public bool IsLayup { get; set; }
        public DateTime LayupDaysStart { get; set; }
        public DateTime LayupDaysEnd { get; set; }
        public int LayupDays { get; set; }
        public string PartyId { get; set; }
        public string PortfolioId { get; set; }
        public string Type { get; set; }
        [RegularExpression("[0-9]{0,15}", ErrorMessage = ("Must be a number"))]
        public string ManufactureYear { get; set; }
        [Required(ErrorMessage = "Please enter a manufacturer company")]
        public string ManufactureCompany { get; set; }
        [Required(ErrorMessage = "Please enter a model of motorcycle")]
        public string Model { get; set; }
        public string SubModel { get; set; }
        public bool IsVehicleDutyFree { get; set; }
        public bool PurchasedNewOld { get; set; }//change datatype to string
        //[Required(ErrorMessage = "Please enter the date you purchased the motorcycle")]
        [RequiredIf("PurchasedNewOld", true, ErrorMessage = "Please enter the date you purchased the vehicle")]
        public DateTime? DateOfPurchase { get; set; }
        [Required(ErrorMessage = "Please enter chasis number")]
        public string ChasisNumber { get; set; }
        [Required(ErrorMessage = "Please enter engine number")]
        public string EngineNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string RegistrationNumberNepali { get; set; }
        public string ProposerName { get; set; }
        public string MyProperty { get; set; }
        public string AgeForPrint { get; set; }
        public string AgeForPrintEnglish { get; set; }
        public RiskEnum[] TypeOfInsurance { get; set; }
        [Required(ErrorMessage = "Please enter the KiloWatt of motorcycle")]
        public decimal? KiloWatt { get; set; }
        [Required(ErrorMessage = "Please enter voluntary excess")]
        public decimal VoluntaryExcess { get; set; }
        public decimal CompulsoryExcess { get; set; }
        public decimal? TotalExcess { get; set; }

        public bool RiotStrike { get; set; }
        public bool Terrorism { get; set; }
        public bool PaToRiderAndOnePillionRider { get; set; }
        //public bool RiotStrikeAndTerrorism { get; set; }

        public string YearsFromRegistrationDateYears { get; set; }
        [RequiredIf("PurchasedNewOld", false, ErrorMessage = "Please enter the date you registered the vehicle")]
        public string YearsFromRegistrationDateYearsBS { get; set; }
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,4})?", ErrorMessage = ("Must be a number with 4 decimal points and maximum length of 25"))]
        // [RegularExpression("d{1,3}([,]{3})*$", ErrorMessage = ("Must be a number with 2 decimal points and maximum length of 15"))]
        public string CurrentMarketPrice { get; set; }
        public decimal? AgeOfVehicle { get; set; }
        public decimal? RateOfDepreciation { get; set; }
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,4})?", ErrorMessage = ("Must be a number with 4 decimal points and maximum length of 25"))]
        // [RegularExpression("[0-9]{0,13}[.]?([0-9]{1,2})?", ErrorMessage = ("Must be a number with 2 decimal points and maximum length of 15"))]
        public string ValueOfAccessories { get; set; }
        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,4})?", ErrorMessage = ("Must be a number with 4 decimal points and maximum length of 25"))]
        // [RegularExpression("[0-9]{0,13}[.]?([0-9]{1,2})?", ErrorMessage = ("Must be a number with 2 decimal points and maximum length of 15"))]
        public string ValueWithoutAccessories { get; set; }
        public decimal SumInsuredAmount { get; set; }
        public int NCDYears { get; set; }
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
        public bool? IsEntitledForNoClaimDiscountFromOtherInsuranceCompany { get; set; }
        public string EntitledForNoClaimDiscountNCDFromOtherInsuranceCompany { get; set; }   ///  bool datatype
        public string RenewalNoticeNCD { get; set; }
        public string HasAnyComputerOrInsurer { get; set; }
        public string AccidentOrLossInThreeYears { get; set; }
        public string HasProposersOrAnyOtherPersonsDrivingLicenseEverBeenCancelled { get; set; }   /// bool datatype
        public string CopyOfPolicyIfOtherVehicleAreInsuredInThisCompany { get; set; }
        public string RiskType { get; set; }
        public bool IsIssued { get; set; }
        //new fields
        public bool IsAgentInvolved { get; set; }
        public int? PreviousPolicyIssuedYear { get; set; }//have to check from db.
        [RegularExpression("[0-9]{0,13}[.]?([0-9]{1,2})?", ErrorMessage = ("Must be a number with 2 decimal points and maximum length of 15"))]
        public decimal? PaToRiderAndOnePillionRiderSumInsuredAmount { get; set; }
        public string AccessoriesDetail { get; set; }
        public bool IsDifferentlyAble { get; set; }
        public decimal? SpecialDiscountRate { get; set; }

        public string MasterPolicyNumber { get; set; }
        public bool Transportation { get; set; }
        public bool Replacement { get; set; }
        public bool Depreciation { get; set; }
        public decimal TransportationRate { get; set; }
        public decimal TransportationAmount { get; set; }
        public decimal ReplacementRate { get; set; }
        public decimal ReplacementAmount { get; set; }
        public decimal DepreciationRate { get; set; }
        public decimal DepreciationAmount { get; set; }
        public bool HasSmartPolicy { get; set; }
    }
}
