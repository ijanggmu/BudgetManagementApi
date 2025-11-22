using Models.WebApi.Address;

namespace Models.BeemaEdgeApi.Customer.CustomerIdentity;

public class UserProfileResponseModel
{
    public string FullName { get; set; }
    public string FullNameNepali { get; set; }
    public string CourtesyTitle { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public int DocumentType { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public string DateOfBirthAD { get; set; }
    public string DateOfBirthBS { get; set; }

    public string MaritalStatus { get; set; }
    public string Gender { get; set; }
    public string IndividualType { get; set; }

    public string CitizenshipNumber { get; set; }
    public string NIDNumber { get; set; }

    public string PassportNumber { get; set; }

    public string VoterIdNumber { get; set; }
    public string LicenseNumber { get; set; }
    public string UserPhoto { get; set; }
    public string UserPhotoUrl { get; set; }

    public string IdFrontPhoto { get; set; }
    public string IdFrontPhotoUrl { get; set; }

    public string IdBackPhoto { get; set; }
    public string IdBackPhotoUrl { get; set; }

    public string DIANumber { get; set; }
    public string KycStatus { get; set; }
    public string KycRejectReason { get; set; }

    public List<AddressResponseModel> Addresses { get; set; }

}

public class AdminUserProfileResponseModel
{
    public string FullName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public int DocumentType { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public string MaritalStatus { get; set; }
    public string Gender { get; set; }
}

public class GetKycResponseModel
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PartyId { get; set; }
    public string PhoneNumber { get; set; }
    public string DateOfBirthAD { get; set; }
    public string DateOfBirthBS { get; set; }
    public string MaritalStatus { get; set; }
    public string Gender { get; set; }
    public string IndividualType { get; set; }
    public string CitizenshipNumber { get; set; }
    public string UserPhotoUrl { get; set; }
    public string IdFrontPhotoUrl { get; set; }
    public string IdBackPhotoUrl { get; set; }
    public string UserId { get; set; }
    public string CourtesyTitle { get; set; }
    public string FullNameNepali { get; set; }
    public string PanNo { get; set; }
    public string CitizenshipNo { get; set; }
    public string CitizenshipIssueDistrict { get; set; }
    public string CitizenshipIssueDate { get; set; }
    public string PassportNumber { get; set; }
    public string PassportIssueDate { get; set; }
    public string PassportExpiryDate { get; set; }
    public string PassportIssuePlace { get; set; }
    public string VoterIdNumber { get; set; }
    public string LicenseNumber { get; set; }
    public string LicenseIssueDate { get; set; }
    public string LicenseExpiryDate { get; set; }
    public string NidNumber { get; set; }
    public string Occupation { get; set; }
    public string UserPhoto { get; set; }
    public string IdFrontPhoto { get; set; }
    public string IdBackPhoto { get; set; }
    public string DigitalSignature { get; set; }
    public string DigitalSignatureUrl { get; set; }
    public string KycStatus { get; set; }
    public string GrandFatherName { get; set; }
    public string FatherName { get; set; }
    public string MotherName { get; set; }
    public string DIANumber { get; set; }
    public string PartyCode { get; set; }
    public int DocumentType { get; set; }
    public string KycRejectReason { get; set; }
    public string CreatedOn { get; set; }
    public List<AddressResponseModel> Addresses { get; set; }

}

