using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Approval
{
    public class ApprovalContactViewModel
    {
        public int SN { get; set; }
        public string BranchId { get; set; }
        public string Id { get; set; }
        public string CourtesyTitle { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "Must be at most 50 characters long")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [StringLength(50, ErrorMessage = "Must be at most 50 characters long")]
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Must be at most 50 characters long")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Full Nepali Name is required")]
        public string FullName_Np { get; set; }
        public string FirstName_Np { get; set; }
        public string MiddleName_Np { get; set; }
        public string LastName_Np { get; set; }
        [RegularExpression(@"^[a-z0-9][-a-z0-9.!#$%&'*+-=?^_`{|}~\/]+@([-a-z0-9]+\.)+[a-z]{2,5}$", ErrorMessage = "Please enter a valid email address.")]
        [StringLength(50, ErrorMessage = "Must be at most 50 characters long")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; }
        public string CompanyName { get; set; }
        public string Gender { get; set; }
        public string MaritialStatus { get; set; }
        public string AddressType { get; set; }
        [Required(ErrorMessage = "Province is required")]
        public string PaProvince { get; set; }
        [Required]
        public string PaDistrict { get; set; }
        [Required]
        public string PaCity { get; set; }
        [Required]
        public string PaMunicipality { get; set; }
        [Required]
        public int? PaWard { get; set; }
        [Required]
        public string PaStreetAddress { get; set; }
        public string TmProvince { get; set; }
        public string TmDistrict { get; set; }
        public string TmMunicipality { get; set; }
        public int? TmWard { get; set; }
        public string TmStreetAddress { get; set; }
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Individual_Type { get; set; }
        [Required]
        public string SecondaryPhone { get; set; }
        [Required]
        public string Individual { get; set; }
        public ApprovalContactKYCViewModel ContactKYCViewModel { get; set; }

        //Signatories Fields
        public string SigTitle { get; set; }
        public string SigGender { get; set; }
        [Required]
        public string SigFirstName { get; set; }
        [Required]
        public string SigLastName { get; set; }
        public string SigMiddleName { get; set; }
        [Required]
        public DateTime? SigDOB { get; set; }
        [Required]
        public DateTime? SigDOBBS { get; set; }
        [Required]
        public string SigFathersName { get; set; }
        [Required]
        public string SigMothersName { get; set; }
        public string SigCitizenshipNo { get; set; }
        public string SigIssuedBy { get; set; }
        [Required]
        public string SigNationality { get; set; }
        [Required]
        public string SigCountryCode { get; set; }
        [Required]
        public string SigOccupation { get; set; }
        [Required]
        public string SigResidence { get; set; }
        [Required]
        public string SigProvince { get; set; }
        [Required]
        public string SigDistrict { get; set; }
        [Required]
        public string SigMunicipality { get; set; }
        [Required]
        public string SigWard { get; set; }
        [Required]
        public string SigStreetAddress { get; set; }
        [Required]
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        public string SigMobileNumber { get; set; }

        [Required]
        public string SigIdentificationType { get; set; }
        [Required]
        public string SigIdentificationNo { get; set; }
        [Required]
        public string SigIdentificationIssueDistrict { get; set; }
        [Required]
        public DateTime? SigIdentificationIssueDateBS { get; set; }
        [Required]
        public DateTime? SigIdentificationIssueDateAD { get; set; }
        [Required]
        public string SigIdentificationIssueBy { get; set; }
        [Required]
        public string SigBranch { get; set; }
        [RegularExpression(@"^[a-z0-9][-a-z0-9.!#$%&'*+-=?^_`{|}~\/]+@([-a-z0-9]+\.)+[a-z]{2,5}$", ErrorMessage = "Please enter a valid email address.")]
        [StringLength(50, ErrorMessage = "Must be at most 50 characters long")]
        [Required]
        public string SigEmail { get; set; }
        public bool SigAddressAsIndividual { get; set; }
        [Required]
        public string SigRoleType { get; set; }
    }

    public class ApprovalContactKYCViewModel
    {
        public string Id { get; set; }
        [Required]
        public DateTime DobAD { get; set; }
        [Required]
        public string DobBS { get; set; }
        public string CitizenshipNo { get; set; }
        public string CitizenshipIssueDistrict { get; set; }
        public DateTime? CitizenshipIssuedate { get; set; }
        public DateTime? CitizenshipIssuedateBS { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? PassportIssueDate { get; set; }
        public string PassportIssuePlace { get; set; }
        public DateTime? PassportExpiryDate { get; set; }
        public string LicenseNumber { get; set; }
        public string LicensePlace { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public DateTime? LicenseExpiryDateBS { get; set; }
        public string VisaType { get; set; }
        public DateTime? VisaIssueDate { get; set; }
        public DateTime? VisaExpiryDate { get; set; }
        public string PanVatNumber { get; set; }
        public string VoterIdNumber { get; set; }
        //Json Object
        public string Occupation { get; set; }
        [Required]
        public string[] OccupationJson { get; set; }
        public string OccupationInTextField { get; set; }
        //Json Object
        public string[] NatureOfBusinessJson { get; set; }
        public string NatureOfBusiness { get; set; }
        public string NatureOfBusinessInTextField { get; set; }
        public bool OwnBusiness { get; set; }
        //Json Object
        public WorkExperience[] WorkExperienceJson { get; set; }
        public string WorkExperiencedetails { get; set; }
        //Json Object
        public string[] EducationJson { get; set; }
        public string Education { get; set; }
        //Json Object
        public string[] ReligionJson { get; set; }
        public string Religion { get; set; }
        public string SpouseFullName { get; set; }
        [Required]
        public string FatherFullName { get; set; }
        [Required]
        public string MotherFullName { get; set; }
        [Required]
        public string ClientClassification { get; set; }
        public string ContactId { get; set; }

        //display date in string
        public string CitizenshipIssuedateBSString { get; set; }
        public string LicenseExpiryDateBSString { get; set; }
        // public string Name { get; set; }   /// For DropDown
        // public ContactViewModel ContactViewModel { get; set; }
        [Required]
        public string IdentificationType { get; set; }
        [Required]
        public string IdentificationNo { get; set; }
        [Required]
        public string IdentificationIssueDistrict { get; set; }
        [Required]
        public DateTime? IdentificationIssuedateBS { get; set; }
        [Required]
        public DateTime? IdentificationIssuedateAD { get; set; }
        [Required]
        public string IdentificationIssueBy { get; set; }
    }
    public class WorkExperience
    {
        public string Work_Experience_Name { get; set; }
        public string Work_Experience_Address { get; set; }
        public string Work_Experience_Designation { get; set; }
        public string Work_Experience_PR { get; set; }
    }
}
