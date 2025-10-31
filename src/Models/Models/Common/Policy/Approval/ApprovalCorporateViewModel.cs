using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Models.Common.Policy.Approval
{
    public class ApprovalCorporateViewModel

    {
        public int SN { get; set; }
        public string Id { get; set; }
        public string BranchId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        //[StringLength(50, ErrorMessage = "Must be at most 50 characters long")]
        [RegularExpression(@"^(?!.*<[^>]+>).*", ErrorMessage = "HTML tags not allowed.")]
        public string Name { get; set; }
        [Required]
        public string NameNp { get; set; }
        [Required]
        public string PanNo { get; set; }
        [Required(ErrorMessage = "Company Registration Number is required")]
        public string CompanyRegistrationNo { get; set; }

        [Required(ErrorMessage = "Registration Date is required")]
        public DateTime? CompanyRegistrationDateBS { get; set; }

        [Required(ErrorMessage = "Registration Date is required")]
        public DateTime? CompanyRegistrationDateAD { get; set; }

        [Required(ErrorMessage = "Registration Authority is required")]
        public string RegistrationAuthority { get; set; }
        [Required]
        public string EstablishDateAD { get; set; }
        [Required]
        public string EstablishDateBS { get; set; }
        [Required]
        public string TypeofCompany { get; set; }
        public string TypeofCompanyText { get; set; }
        [Required]
        public string RiskProfile { get; set; }
        [Required(ErrorMessage = "Communication Type is required")]
        public string CommunicationType { get; set; }
        public string Website { get; set; }
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        [Required]
        public string Phone1 { get; set; }
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        [Required]
        public string Phone2 { get; set; }
        [RegularExpression(@"^[a-z0-9][-a-z0-9.!#$%&'*+-=?^_`{|}~\/]+@([-a-z0-9]+\.)+[a-z]{2,5}$", ErrorMessage = "Please enter a valid email address.")]
        [Required]
        public string Email1 { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public string District { get; set; }
        public string City { get; set; }
        [Required]
        public string Municipality { get; set; }
        [RegularExpression("^[0-9+-]{1,30}$", ErrorMessage = ("Must be a valid number"))]
        [Required]
        public int? Ward { get; set; }
        public string StreetAddress { get; set; }
        public string HouseNumber { get; set; }
        [Required]
        public string AddressType { get; set; }
        public string HOOName { get; set; }
        public string HOOProvince { get; set; }
        public string HOODistrict { get; set; }
        public string HOOMunicipality { get; set; }
        public string HOOWarkNo { get; set; }
        public string HOOStreetTole { get; set; }
        public string HOOHouseNo { get; set; }
        public string HOOTelephone { get; set; }
        public string HOOMobile { get; set; }
        public string HOOEmail { get; set; }
        [RegularExpression("[0-9.]*[0-9]{1,20}", ErrorMessage = ("Must be a number with maximum length of 20"))]
        public string Latitude { get; set; }
        [RegularExpression("[0-9.]*[0-9]{1,20}", ErrorMessage = ("Must be a number with maximum length of 20"))]
        public string Longitude { get; set; }
        public string ParentCompanyId { get; set; }
        public string Details { get; set; }
        public string IndustryName { get; set; }
        public string LogoUrl { get; set; }
        public string Image { get; set; }

        [Required(ErrorMessage = "Business is required")]
        public string IndustryId { get; set; }
        public double BoundingBox { get; set; }
        public IFormFile Logo { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAssigned { get; set; }
        public InternationalHeadquaterViewModel InternationalHeadquater { get; set; }
        public List<ApprovalCorporateMemberViewModel> CorporateMember { get; set; }
        public string PartyCode { get; set; }
        public string MultipleParty { get; set; }
        public string MultipleCareOf { get; set; }
        public string MultipleFinancer { get; set; }
        public string SpecialAddressForTI { get; set; }
        public string LocalFacultativeInwardChartOfAccountId { get; set; }
        public bool IsFromUnderwriting { get; set; }

        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public string Percentage { get; set; }
        public string BranchName { get; set; }



        //Signatories Fields
        [Required]
        public string SigTitle { get; set; }
        [Required]
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
        public string SigMothersName { get; set; }
        [Required(ErrorMessage = "Signatories Identification Type is required")]
        public string SigIdentificationType { get; set; }
        public string SigCitizenshipNo { get; set; }
        [Required]
        public string SigIdentificationNo { get; set; }
        [Required]
        public string SigIssuedBy { get; set; }
        public DateTime? SigIssuedDate { get; set; }
        public DateTime? SigIssuedDateBS { get; set; }
        [Required]
        public string SigNationality { get; set; }
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
        public string SigStreetAddress { get; set; }
        [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = ("Must be a valid number of 7 to 15 characters"))]
        [Required]
        public string SigMobileNumber { get; set; }
        public string UserType { get; set; }
        [RegularExpression(@"^[a-z0-9][-a-z0-9.!#$%&'*+-=?^_`{|}~\/]+@([-a-z0-9]+\.)+[a-z]{2,5}$", ErrorMessage = "Please enter a valid email address.")]
        [Required]
        public string SigEmail { get; set; }
        public string SigBranch { get; set; }
        [Required]
        public string SigCountryCode { get; set; }
        [Required]
        public string SigEntityPersonRoleType { get; set; }
        public bool SigAddressAsCorporate { get; set; }
    }

    public class ApprovalCorporateMemberViewModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public DateTime? DateOfBirthBS { get; set; }
        [Required]
        public string Occupation { get; set; }
        [Required(ErrorMessage = "Identification Type is required")]
        public string IdentificationType { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string Municipality { get; set; }
        [Required]
        public string Ward { get; set; }
        public string StreetName { get; set; }
        public string HouseNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Landline { get; set; }
        public string Remarks { get; set; }
        public string CorporateId { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required]
        public string Residence { get; set; }
        public string Branch { get; set; }
        [Required]
        public string CountryCode { get; set; }
        [Required]
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        [Required]
        public string IdentificationNo { get; set; }
        [Required]
        public string IdentificationIssuedBy { get; set; }
        public DateTime? IdentificationIssuedDate { get; set; }
        public DateTime? IdentificationIssuedDateBS { get; set; }
        [Required]
        public string EntityPersonRoleType { get; set; }
        public bool AddressAsCorporate { get; set; }
    }

}
