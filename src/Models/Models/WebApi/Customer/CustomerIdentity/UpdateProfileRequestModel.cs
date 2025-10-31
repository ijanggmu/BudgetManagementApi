using System.ComponentModel.DataAnnotations;
using Models.Common.Policy.Policy;
using Models.WebApi.Address;
using SharedKernel.Attributes;

namespace Models.BeemaEdgeApi.Customer.CustomerIdentity;

public class UpdateProfileRequestModel
{
    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
    public string FullName { get; set; }

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
    [EmailAddress]
    public string Email { get; set; }

    [StringLength(20, ErrorMessage = "PAN No cannot exceed 9 characters.")]
    public string PanNo { get; set; }

    [Required(ErrorMessage = "Marital Status is required.")]
    public string MaritialStatus { get; set; }

    [Required(ErrorMessage = "Date of Birth (BS) is required.")]
    public string DobBS { get; set; }

    [Required(ErrorMessage = "Date of Birth (AD) is required.")]
    public string DobAD { get; set; }

    [StringLength(50, ErrorMessage = "Citizenship Number cannot exceed 50 characters.")]
    public string CitizenshipNo { get; set; }
    [StringLength(15, ErrorMessage = "NID Number cannot exceed 15 characters.")]
    public string NIDNumber { get; set; }

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

    [DataType(DataType.Date, ErrorMessage = "Invalid license Issue Date.")]
    public string LicenseIssueDate { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Invalid license Expiry Date.")]
    public string LicenseExpiryDate { get; set; } // Consider changing to DateTime?
    public string UserPhotoUrl { get; set; }
    public string IdFrontPhotoUrl { get; set; }
    public string IdBackPhotoUrl { get; set; }
    public string UserPhoto { get; set; }

    [Required(ErrorMessage = "ID Front Photo URL must be a valid URL.")]
    public string IdFrontPhoto { get; set; }
    public string IdBackPhoto { get; set; }

    [Required(ErrorMessage = "Occupation data is required.")]
    public string Occupation { get; set; }

    [Required(ErrorMessage = "At least one address is required.")]
    [MinLength(1, ErrorMessage = "At least one address is required.")]
    public List<AddressResponseModel> Addresses { get; set; }
    public string DigitalSignatureUrl { get; set; }
    public string DigitalSignature { get; set; }
    public string FatherName { get; set; }
    public string GrandFatherName { get; set; }
    public string MotherName { get; set; }
}
public class UsernameValidationRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = default!;
}
public class EmailValidationRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; } = default!;
}

public class PhoneNumberValidationRequest
{
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = default!;
}

public class SetKycRequestModel
{
    [Required(ErrorMessage = "Document type is required")]
    public DocumentType DocumentType { get; set; }
    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Individual Type is required.")]
    [StringLength(50, ErrorMessage = "Individual Type cannot exceed 50 characters.")]
    public string IndividualType { get; set; }

    [Required(ErrorMessage = "Courtesy Title is required.")]
    [StringLength(20, ErrorMessage = "Courtesy Title cannot exceed 20 characters.")]
    public string CourtesyTitle { get; set; }

    //[Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, ErrorMessage = "Full Name (Nepali) cannot exceed 100 characters.")]
    public string FullNameNepali { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    [RegularExpression("Male|Female|Other", ErrorMessage = "Gender must be Male, Female, or Other.")]
    public string Gender { get; set; }

    [StringLength(9, ErrorMessage = "PAN No cannot exceed 9 characters.")]
    public string PanNo { get; set; }

    [Required(ErrorMessage = "Marital Status is required.")]
    //[RegularExpression("Single|Married|Divorced|Widowed", ErrorMessage = "Marital Status must be Single, Married, Divorced, or Widowed.")]
    public string MaritialStatus { get; set; }

    [Required(ErrorMessage = "Date of Birth (BS) is required.")]
    public string DobBS { get; set; }

    [Required(ErrorMessage = "Date of Birth (AD) is required.")]
    public string DobAD { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.Citizenship, ErrorMessage = "Citizenship Number is required.")]

    [StringLength(50, ErrorMessage = "Citizenship Number cannot exceed 50 characters.")]
    public string CitizenshipNo { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.Citizenship, ErrorMessage = "Citizenship issue district is required.")]
    [StringLength(100, ErrorMessage = "Citizenship Issue District cannot exceed 100 characters.")]
    public string CitizenshipIssueDistrict { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.Citizenship, ErrorMessage = "Citizenship issue date is required.")]

    [DataType(DataType.Date, ErrorMessage = "Invalid Citizenship Issue Date.")]
    public string CitizenshipIssueDate { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.Passport, ErrorMessage = "Passport Number is required.")]
    [StringLength(50, ErrorMessage = "Passport Number cannot exceed 50 characters.")]
    public string PassportNumber { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.Passport, ErrorMessage = "Passport issue date is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid Passport Issue Date.")]
    public string PassportIssueDate { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.Passport, ErrorMessage = "Passport expiry date is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid Passport Expiry Date.")]
    public string PassportExpiryDate { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.Passport, ErrorMessage = "Passport issue place is required.")]
    [StringLength(100, ErrorMessage = "Passport Issue Place cannot exceed 100 characters.")]
    public string PassportIssuePlace { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.VoterID, ErrorMessage = "Voter Id Number is required.")]
    [StringLength(50, ErrorMessage = "Voter ID Number cannot exceed 50 characters.")]
    public string VoterIdNumber { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.License, ErrorMessage = "License Number is required.")]
    [StringLength(50, ErrorMessage = "License Number cannot exceed 50 characters.")]
    public string LicenseNumber { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.NID, ErrorMessage = "National identity number is required.")]
    [StringLength(15, ErrorMessage = "NID cannot exceed 50 characters.")]
    public string NIDNumber { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.License, ErrorMessage = "Passport issue date is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid Passport Issue Date.")]
    public string LicenseIssueDate { get; set; }

    [RequiredIf(nameof(DocumentType), (int)DocumentType.License, ErrorMessage = "Passport expiry date is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid Passport Expiry Date.")]
    public string LicenseExpiryDate { get; set; }

    [Required(ErrorMessage = "User Photo URL is required.")]
    public string UserPhotoUrl { get; set; }

    [Required(ErrorMessage = "ID Front Photo URL is required.")]
    public string IdFrontPhotoUrl { get; set; }
    public string IdBackPhotoUrl { get; set; }

    [Required(ErrorMessage = "Occupation is required.")]
    [StringLength(100, ErrorMessage = "Cannot exceed 100 characters.")]
    public string Occupation { get; set; }

    [Required(ErrorMessage = "At least one address is required.")]
    [MinLength(1, ErrorMessage = "At least one address must be provided.")]
    public List<AddressResponseModel> Addresses { get; set; }

    [Required(ErrorMessage = "Digital Signature URL is required.")]
    public string DigitalSignatureUrl { get; set; }
    [Required(ErrorMessage = "Grandfather's Name is required.")]
    [StringLength(50, ErrorMessage = "Cannot exceed 50 characters.")]
    public string GrandFatherName { get; set; }

    [Required(ErrorMessage = "Father's Name is required.")]
    [StringLength(50, ErrorMessage = "Cannot exceed 50 characters.")]
    public string FatherName { get; set; }

    [Required(ErrorMessage = "Mother's Name is required.")]
    [StringLength(50, ErrorMessage = "Cannot exceed 50 characters.")]
    public string MotherName { get; set; }

}

public enum DocumentType
{
    Citizenship,
    Passport,
    PAN,
    VoterID,
    NID,
    License
}


