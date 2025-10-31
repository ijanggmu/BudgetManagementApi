using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SharedKernel.Attributes;

namespace Models.Common.Policy.Policy.Marine
{
    public class MarinePolicyParitalViewModel
    {
        public List<MaterialOfInsurance> MaterialsOfInsurance { get; set; }
        public List<MaterialOfInsurance> AddMaterialsOfInsurance { get; set; }
        public List<MaterialOfInsurance> UpdateMaterialsOfInsurance { get; set; }
        public List<MaterialOfInsurance> DeleteMaterialsOfInsurance { get; set; }

        public List<LetterOfCredit> LettersOfCredit { get; set; }
        public List<LetterOfCredit> UpdateLettersOfCredit { get; set; }
        public List<LetterOfCredit> DeleteLettersOfCredit { get; set; }
        public List<InvoiceDetail> InvoiceDetails { get; set; }

        [DisplayName("Tolerance (%)")]
        [Range(0, 100)]
        public decimal? ToleranceRate { get; set; }

        [DisplayName("Incremental Cost (%)")]
        [Range(0, 100)]
        public decimal? IncrementalCostRate { get; set; }

        [DisplayName("Duty (%)")]

        public decimal? DutyRate { get; set; }

        [DisplayName("Currency Of Value")]
        [Required]
        public string CurrencyOfValue { get; set; }

        [DisplayName("Is Exchange Rate Manual")]
        public bool IsManualExchangeRate { get; set; }

        [DisplayName("Enter Exchange Rate Manually")]
        public decimal? ExchangeRate { get; set; }

        [DisplayName("Mode Of Tansit")]
        [Required]
        public ModeOftransit ModeOfTransit { get; set; }

        [DisplayName("Inland Transit Type")]
        [RequiredIf(nameof(ModeOfTransit), ModeOftransit.GoodsInTransit)]
        public InlandTransitType InlandTransitType { get; set; }

        [DisplayName("Inland Transit Distance")]
        [RequiredIf(nameof(ModeOfTransit), ModeOftransit.GoodsInTransit)]
        public decimal? InlandTransitDistance { get; set; }

        [DisplayName("Container Used")]
        public bool IsContainerUsed { get; set; }

        [DisplayName("All Risks")]
        public bool IsAllRiskSelected { get; set; }

        [DisplayName("Non Delivery")]
        public bool IsNonDeliverySelected { get; set; }

        [DisplayName("Water Damage")]
        public bool IsWaterDamageSelected { get; set; }

        [DisplayName("TPND")]
        public bool IsTPNDSelected { get; set; }

        [DisplayName("SRCC")]
        public bool IsSRCCSelected { get; set; }
        [DisplayName("MinimumRisk")]
        public bool IsMinimumRiskSelected { get; set; }

        [DisplayName("Voyage From")]
        [Required]
        public string VoyageFrom { get; set; }

        [DisplayName("Voyage To")]
        [Required]
        public string VoyageTo { get; set; }

        [DisplayName("Ship/Conveyance")]
        public string ShipConveyance { get; set; }

        [DisplayName("Estimated Date of Departure")]
        public DateTime? EstimatedDateOfDeparture { get; set; }
        [DisplayName("Type of Invoice")]

        public string TypeOfInvoice { get; set; }
        public string OtherInvoice { get; set; }

        [DisplayName("Pro-forma Invoice")]
        public string ProFormaInvoice { get; set; }

        [DisplayName("Invoice Date")]
        public string ProFormaDate { get; set; }


        [DisplayName("B/L ,AWB,C/N,R/R No.")]
        public string BLAWBCNRRNumber { get; set; }

        [DisplayName("Subject Matter Insured/Interest (Marks and number as per B/L No Consignment/Invoice above)")]
        public string SubjectMatterOfInsurance { get; set; }
        public string Exclusion { get; set; }

        public Surveyor Surveyor { get; set; }
        [DisplayName("Excess/Deductible")]
        public string Deductible { get; set; }
        public string[] Clauses { get; set; }
        public string ClausesJson { get; set; }
        [DisplayName("Warranty Field")]

        public string WarrantyField { get; set; }
        [DisplayName("Per Consignment SumInsured")]

        public decimal? PerConsignmentSumInsured { get; set; }
        [Range(1, 366, ErrorMessage = "Please enter a value less than or equal to 366")]
        [RegularExpression("[0-9]{1,3}", ErrorMessage = "Please enter a value less than or equal to 366")]
        [Required(ErrorMessage = "Please enter number of days")]
        public string Days { get; set; }
        public decimal? LimitPerSending { get; set; }
        public decimal? LimitPerLocation { get; set; }
        [DisplayName("Change Rate")]
        public bool IsRateChanged { get; set; }
        public bool AddSuminsured { get; set; }
        public bool DecreaseSuminsured { get; set; }
        public decimal UpdatedPerConsignmentSI { get; set; }
        public decimal UpdateLimitPerLocation { get; set; }
        public decimal UpdateLimitPerSending { get; set; }
        public string InvoiceText { get; set; }
        public bool IsInstallmentPayment { get; set; }
        public decimal BasicPremiumInstallment { get; set; }
        public decimal SRCCPremiumInstallment { get; set; }
        public string InstallmentDate { get; set; }
        public string VehicleNumber { get; set; }
        public string CodeName { get; set; }
    }



    public class Surveyor
    {
        public string SurveyorId { get; set; }
        [DisplayName("Surveyor Name")]
        public string Name { get; set; }

        [DisplayName("Contact Person")]
        public string ContactPerson { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }

        [DisplayName("City/State/Country(Address)")]
        public string CityStateCountry { get; set; }

        [DisplayName("Telephone Number")]
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        public string PhoneNumber { get; set; }

        [DisplayName("Fax")]
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        public string Fax { get; set; }

        [DisplayName("Email")]
        [RegularExpression(@"^[a-z0-9][-a-z0-9.!#$%&'*+-=?^_`{|}~\/]+@([-a-z0-9]+\.)+[a-z]{2,5}$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

    }

    public class MaterialOfInsurance
    {
        public string PrimaryId { get; set; }
        public string SN { get; set; }
        public string Description { get; set; }
        public string ProductTypeCode { get; set; }
        public decimal UnitValue { get; set; }
        public decimal FullUnitValue { get; set; }
        public decimal Quantity { get; set; }
        public decimal ChangeInQuantity { get; set; }
        public decimal InvoiceValue { get; set; }
        public decimal SRCC { get; set; }
        public bool IsActive { get; set; }
        public bool IsExisting { get; set; }
        public bool IsUpdated { get; set; }
        public decimal BasicPremium { get; set; }
    }


    public class LetterOfCredit
    {
        public string LCCode { get; set; }
        public decimal LCAmount { get; set; }
        [DisplayName("L/C Date")]
        public DateTime? LCDate { get; set; }
    }
    public class InvoiceDetail
    {
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
    }

    public enum ModeOftransit
    {
        [Display(Name = "Marine Cargo")]
        MarineCargo,
        [Display(Name = "Goods In Transit")]
        GoodsInTransit,
        [Display(Name = "Air Cargo")]
        AirCargo
    }

    public enum InlandTransitType
    {
        [Display(Name = "Inland Transit Outside Nepal")]
        OutsideNepal,
        [Display(Name = "Inland Transit Within Nepal")]
        WithinNepal
    }
}
