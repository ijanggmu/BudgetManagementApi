using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy;

public class SurveyorListViewModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Sn { get; set; }

    [Required(ErrorMessage = "Surveyor list is required.")]
    public string SurveyorName { get; set; }
    public string ContactPerson { get; set; }
    public string Address { get; set; }
    [DisplayName("City/State/Country(Address)")]
    public string StreetAddress { get; set; }
    [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = "Phone number should be a valid number of 7 to 15 characters.")]
    public string TelephoneNumber { get; set; }
    [DisplayName("Fax")]
    [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
    public string Fax { get; set; }
    [DisplayName("Email")]
    [RegularExpression(@"^[a-z0-9][-a-z0-9.!#$%&'*+-=?^_`{|}~\/]+@([-a-z0-9]+\.)+[a-z]{2,5}$", ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; }
    public string PFAlias { get; set; }
    public string CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsPublished { get; set; }
}
public class AgriculturePolicyFileViewModel
{
    public List<AgricultureFileUploadViewModel> AgricultureFileUpload { get; set; }
    public AgricultureTagListViewModel AgricultureTagList { get; set; }
    public string PolicyNumber { get; set; }
    public string DocumentNumber { get; set; }
    public string ClassId { get; set; }
    public string CreatedBy { get; set; }
    public List<AgricultureCattleTagPolicyViewModel> TagList { get; set; }
    public IEnumerable<AgricultureCattleTagPolicyViewModel> PagedTagList { get; set; }
    public bool HasEmptyClassDetails { get; set; } = false;
    public string ErrorMessage { get; set; }
}
//public class CorporateViewModel
//{

//    public int SN { get; set; }
//    public string Id { get; set; }
//    public string BranchId { get; set; }
//    [Required(ErrorMessage = "Name is required")]
//    //[StringLength(50, ErrorMessage = "Must be at most 50 characters long")]
//    [RegularExpression(@"^(?!.*<[^>]+>).*", ErrorMessage = "HTML tags not allowed.")]
//    public string Name { get; set; }
//    public string NameNp { get; set; }

//    [Remote(action: "ExistedPanNoValidation", controller: "Corporate", HttpMethod = "POST", AdditionalFields = "Id,IsNgo")]
//    [RegularExpression("(^[0-9]{9}$)|(^-$)", ErrorMessage = "Please enter valid pan number.")]

//    [Required(ErrorMessage = "PAN number is required")]

//    public string PanNo { get; set; }

//    public bool IsNgo { get; set; }


//    [Required(ErrorMessage = "Company Registration Number is required")]
//    [Remote("ExistedCompanyRegValidation", "Corporate", HttpMethod = "POST", AdditionalFields = "Id")]
//    public string CompanyRegistrationNo { get; set; }

//    [Required(ErrorMessage = "Registration Date is required")]
//    public DateTime? CompanyRegistrationDateBS { get; set; }

//    [Required(ErrorMessage = "Registration Date is required")]
//    public DateTime? CompanyRegistrationDateAD { get; set; }

//    [Required(ErrorMessage = "Registration Authority is required")]
//    public string RegistrationAuthority { get; set; }
//    public string EstablishDateAD { get; set; }
//    public string EstablishDateBS { get; set; }
//    public string TypeofCompany { get; set; }
//    public string TypeofCompanyText { get; set; }
//    public string RiskProfile { get; set; }
//    [Required(ErrorMessage = "Communication Type is required")]
//    public string CommunicationType { get; set; }
//    public string Website { get; set; }
//    [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]

//    public string Phone1 { get; set; }
//    [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]

//    public string Phone2 { get; set; }
//    [RegularExpression(@"^[a-zA-Z0-9][-a-zA-Z0-9.!#$%&'*+-=?^_`{|}~\/]+@([-a-zA-Z0-9]+\.)+[a-zA-Z]{2,5}$", ErrorMessage = "Please enter a valid email address.")]

//    public string Email1 { get; set; }
//    public string Province { get; set; }
//    public string District { get; set; }
//    public string City { get; set; }
//    public string Municipality { get; set; }
//    [RegularExpression("^[0-9+-]{1,30}$", ErrorMessage = ("Must be a valid number"))]
//    public int? Ward { get; set; }
//    public string StreetAddress { get; set; }
//    public string HouseNumber { get; set; }
//    public string AddressType { get; set; }
//    public string HOOName { get; set; }
//    public string HOOProvince { get; set; }
//    public string HOODistrict { get; set; }
//    public string HOOMunicipality { get; set; }
//    public string HOOWarkNo { get; set; }
//    public string HOOStreetTole { get; set; }
//    public string HOOHouseNo { get; set; }
//    public string HOOTelephone { get; set; }
//    [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
//    public string HOOMobile { get; set; }
//    public string HOOEmail { get; set; }
//    [RegularExpression("[0-9.]*[0-9]{1,20}", ErrorMessage = ("Must be a number with maximum length of 20"))]
//    public string Latitude { get; set; }
//    [RegularExpression("[0-9.]*[0-9]{1,20}", ErrorMessage = ("Must be a number with maximum length of 20"))]
//    public string Longitude { get; set; }
//    public string ParentCompanyId { get; set; }
//    public string Details { get; set; }
//    public string IndustryName { get; set; }
//    public string LogoUrl { get; set; }
//    public string Image { get; set; }

//    [Required(ErrorMessage = "Business is required")]
//    public string IndustryId { get; set; }
//    public double BoundingBox { get; set; }
//    public IFormFile Logo { get; set; }
//    public string CreatedDate { get; set; }
//    public string CreatedBy { get; set; }
//    public bool IsDeleted { get; set; }
//    public bool IsAssigned { get; set; }
//    public InternationalHeadquaterViewModel InternationalHeadquater { get; set; }
//    public List<CorporateMemberViewModel> CorporateMember { get; set; }
//    public string PartyCode { get; set; }
//    public string MultipleParty { get; set; }
//    public string MultipleCareOf { get; set; }
//    public string MultipleFinancer { get; set; }
//    public string SpecialAddressForTI { get; set; }
//    public string LocalFacultativeInwardChartOfAccountId { get; set; }
//    public bool IsFromUnderwriting { get; set; }

//    public string UpdatedDate { get; set; }
//    public string UpdatedBy { get; set; }

//    public string Percentage { get; set; }
//    public string BranchName { get; set; }



//    //Signatories Fields
//    public string SigTitle { get; set; }
//    public string SigGender { get; set; }
//    public string SigFirstName { get; set; }
//    public string SigLastName { get; set; }
//    public string SigMiddleName { get; set; }
//    public DateTime? SigDOBBS { get; set; }
//    public DateTime? SigDOB { get; set; }
//    public string SigFathersName { get; set; }
//    public string SigMothersName { get; set; }
//    [Required(ErrorMessage = "Signatories Identification Type is required")]
//    public string SigIdentificationType { get; set; }
//    public string SigCitizenshipNo { get; set; }
//    public string SigIdentificationNo { get; set; }
//    public string SigIssuedBy { get; set; }
//    public DateTime? SigIssuedDate { get; set; }
//    public DateTime? SigIssuedDateBS { get; set; }
//    public string SigNationality { get; set; }
//    public string SigOccupation { get; set; }
//    public string SigResidence { get; set; }
//    public string SigProvince { get; set; }
//    public string SigDistrict { get; set; }
//    public string SigMunicipality { get; set; }
//    public string SigWard { get; set; }
//    public string SigStreetAddress { get; set; }
//    [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
//    public string SigMobileNumber { get; set; }
//    public string UserType { get; set; }
//    public bool CreatePolicy { get; set; }
//    [RegularExpression(@"^[a-zA-Z0-9][-a-zA-Z0-9.!#$%&'*+-=?^_`{|}~\/]+@([-a-zA-Z0-9]+\.)+[a-zA-Z]{2,5}$", ErrorMessage = "Please enter a valid email address.")]
//    public string SigEmail { get; set; }
//    public string SigBranch { get; set; }
//    public string SigCountryCode { get; set; }
//    public string SigEntityPersonRoleType { get; set; }
//    public bool SigAddressAsCorporate { get; set; }
//    public string CoinsuranceInwardCOAId { get; set; }
//    public string DIANumber { get; set; }
//}
public class InternationalHeadquaterViewModel
{
    public string OrganizationName { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string StreetAddress { get; set; }
    public string PhoneFirst { get; set; }
    public string PhoneSecond { get; set; }
    public string EmailFirst { get; set; }
    public string EmailSecond { get; set; }
    public string Website { get; set; }
}

public class CorporateMemberViewModel
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string FullName { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public DateTime? DateOfBirthBS { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Occupation { get; set; }
    [Required(ErrorMessage = "Identification Type is required")]
    public string IdentificationType { get; set; }
    public string Province { get; set; }
    public string District { get; set; }
    public string Municipality { get; set; }
    public string Ward { get; set; }
    public string StreetName { get; set; }
    public string HouseNumber { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public string Landline { get; set; }
    public string Remarks { get; set; }
    public string CorporateId { get; set; }
    public string Nationality { get; set; }
    public string Residence { get; set; }
    public string Branch { get; set; }
    public string CountryCode { get; set; }
    public string FathersName { get; set; }
    public string MothersName { get; set; }
    public string IdentificationNo { get; set; }
    public string IdentificationIssuedBy { get; set; }
    public DateTime? IdentificationIssuedDateBS { get; set; }
    public DateTime? IdentificationIssuedDate { get; set; }
    public string EntityPersonRoleType { get; set; }
    public bool AddressAsCorporate { get; set; }
}

