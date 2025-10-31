using System.ComponentModel.DataAnnotations;
using Models.WebApi.Customer.Policy;

namespace Models.WebApi.Individual;
public class CreateIndividualRequestModel
{
    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
    public string FullName { get; set; }
    public string MiddleName { get; set; }

    [Required(ErrorMessage = "Individual Type is required.")]
    [StringLength(50)]
    public string IndividualType { get; set; }

    [StringLength(20, ErrorMessage = "Courtesy Title cannot exceed 20 characters.")]
    public string CourtesyTitle { get; set; }

    [StringLength(100, ErrorMessage = "Full Name (Nepali) cannot exceed 100 characters.")]
    public string FullNameNepali { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    [RegularExpression("Male|Female|Other", ErrorMessage = "Gender must be Male, Female, or Other.")]
    public string Gender { get; set; }

    [StringLength(20, ErrorMessage = "PAN No cannot exceed 20 characters.")]
    public string PanNo { get; set; }

    [Required(ErrorMessage = "Marital Status is required.")]
    [RegularExpression("Single|Married|Divorced|Widowed", ErrorMessage = "Invalid Marital Status.")]
    public string MaritialStatus { get; set; }

    [Required(ErrorMessage = "Date of Birth (BS) is required.")]
    public string DobBS { get; set; }

    [Required(ErrorMessage = "Date of Birth (AD) is required.")]
    public DateTime DobAD { get; set; }

    [StringLength(50, ErrorMessage = "Citizenship Number cannot exceed 50 characters.")]
    public string CitizenshipNo { get; set; }

    [StringLength(100, ErrorMessage = "Citizenship Issue District cannot exceed 100 characters.")]
    public string CitizenshipIssueDistrict { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Invalid Citizenship Issue Date.")]
    public string CitizenshipIssueDate { get; set; } // Consider changing to DateTime?

    [StringLength(50, ErrorMessage = "Passport Number cannot exceed 50 characters.")]
    public string PassportNumber { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Invalid Passport Issue Date.")]
    public string PassportIssueDate { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Invalid Passport Expiry Date.")]
    public string PassportExpiryDate { get; set; } // Consider changing to DateTime?

    [StringLength(100, ErrorMessage = "Passport Issue Place cannot exceed 100 characters.")]
    public string PassportIssuePlace { get; set; }

    [StringLength(50, ErrorMessage = "Voter ID Number cannot exceed 50 characters.")]
    public string VoterIdNumber { get; set; }

    [StringLength(50, ErrorMessage = "License Number cannot exceed 50 characters.")]
    public string LicenseNumber { get; set; }
    [StringLength(50, ErrorMessage = "NID Number cannot exceed 50 characters.")]
    public string NidNumber { get; set; }
    public string IdentificationType { get; set; }
    public string IdentificationNo { get; set; }

    //[Url(ErrorMessage = "User Photo URL must be a valid URL.")]
    public string UserPhotoUrl { get; set; }

    //[Url(ErrorMessage = "ID Front Photo URL must be a valid URL.")]
    public string IdFrontPhotoUrl { get; set; }

    //[Url(ErrorMessage = "ID Back Photo URL must be a valid URL.")]
    public string IdBackPhotoUrl { get; set; }

    // Assuming OccupationJson contains JSON strings; validate presence if required
    [Required(ErrorMessage = "Occupation data is required.")]
    public List<string> OccupationJson { get; set; }

    [Required(ErrorMessage = "At least one address is required.")]
    [MinLength(1, ErrorMessage = "At least one address is required.")]
    public string TmProvince { get; set; }
    public string TmDistrict { get; set; }
    public string TmMunicipality { get; set; }
    public int? TmWard { get; set; }
    public string TmStreetAddress { get; set; }

    public string PaProvince { get; set; }
    public string PaDistrict { get; set; }
    public string PaMunicipality { get; set; }
    public int? PaWard { get; set; }
    public string PaStreetAddress { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FirstNameNepali { get; set; }
    public string LastNameNepali { get; set; }
    public string MiddleNameNepali { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public ContactKYCViewModel ContactKYCViewModel { get; set; }
}
