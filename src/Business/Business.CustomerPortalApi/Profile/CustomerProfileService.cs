using System.Xml.Linq;
using Business.Common.File;
using Business.Common.JobHelper;
using Business.BeemaEdgeApi.HangFireJob.CustomerProfileJob;
using Data.Context;
using Data.Entities.CustomerEntity;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.WebApi.Address;
using Models.WebApi.Customer.Policy;
using Models.WebApi.Individual;
using SharedKernel.Constant.ResponseConstant;
using SharedKernel.Operation;
using SharedKernel.SystemEnum;

namespace Business.BeemaEdgeApi.Profile;

public class CustomerProfileService(
    ApplicationDataContext dbContext,
    IUserProfileService ipersonAccessor,
    IFileService fileService) : ICustomerProfileService
{
    public async Task<Result<UserProfileResponseModel>> GetProfileAsync()
    {
        var userId = ipersonAccessor.GetUserId();

        var customer = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted);

        if (customer == null)
            return Result<UserProfileResponseModel>.Failed(ResponseMessage.UserNotFound);

        var addressModels = customer.Addresses?
            .Where(a =>
                a.AddressType.Equals(AddressTypeEnums.Permanent.ToString(), StringComparison.OrdinalIgnoreCase) ||
                a.AddressType.Equals(AddressTypeEnums.Temporary.ToString(), StringComparison.OrdinalIgnoreCase))
            .Select(a => new AddressResponseModel
            {
                AddressType = a.AddressType,
                Province = a.Province,
                District = a.District,
                Municipality = a.Municipality,
                Ward = a.Ward,
                StreetAddress = a.StreetAddress
            })
            .ToList() ?? new List<AddressResponseModel>();
        int documentType = 0;

        if (!string.IsNullOrWhiteSpace(customer.CitizenshipNo))
            documentType = (int)DocumentType.Citizenship;
        else if (!string.IsNullOrWhiteSpace(customer.PassportNumber))
            documentType = (int)DocumentType.Passport;
        else if (!string.IsNullOrWhiteSpace(customer.LicenseNumber))
            documentType = (int)DocumentType.License;
        else if (!string.IsNullOrWhiteSpace(customer.VoterIdNumber))
            documentType = (int)DocumentType.VoterID;
        else if (!string.IsNullOrWhiteSpace(customer.NIDNumber))
            documentType = (int)DocumentType.NID;

        var profile = new UserProfileResponseModel
        {
            // Basic Info
            FullName = customer.FullName,
            FullNameNepali = customer.FullNameNepali,
            CourtesyTitle = customer.CourtesyTitle,
            FirstName = customer.FirstName,
            MiddleName = customer.MiddleName,
            LastName = customer.LastName,

            // Contact
            Email = customer.User?.Email,
            PhoneNumber = customer.User?.PhoneNumber,

            // DOB
            DateOfBirthAD = customer.DobAD,
            DateOfBirthBS = customer.DobBS,

            // Personal Info
            MaritalStatus = customer.MaritalStatus,
            Gender = customer.Gender,
            IndividualType = customer.IndividualType,
            DocumentType = documentType,
            CitizenshipNumber = customer.CitizenshipNo,
            PassportNumber = customer.PassportNumber,
            VoterIdNumber = customer.VoterIdNumber,
            LicenseNumber = customer.LicenseNumber,
            NIDNumber = customer.NIDNumber,
            // Images (file URLs)
            UserPhoto = customer.UserPhoto,
            IdFrontPhoto = customer.IdFrontPhoto,
            IdBackPhoto = customer.IdBackPhoto,

            DIANumber = customer.DIANumber,
            KycStatus = customer.KycStatus.ToString(),
            KycRejectReason = customer.KycRejectedReason,

            Addresses = addressModels
        };

        if (!string.IsNullOrWhiteSpace(customer.UserPhoto))
            profile.UserPhotoUrl = await fileService.GetFilePresignedUrlAsync(customer.UserPhoto);

        if (!string.IsNullOrWhiteSpace(customer.IdFrontPhoto))
            profile.IdFrontPhotoUrl = await fileService.GetFilePresignedUrlAsync(customer.IdFrontPhoto);

        if (!string.IsNullOrWhiteSpace(customer.IdBackPhoto))
            profile.IdBackPhotoUrl = await fileService.GetFilePresignedUrlAsync(customer.IdBackPhoto);

        return Result<UserProfileResponseModel>.Success(profile);
    }


    public async Task<Result<MessageResponseModel>> UpdateProfileAsync(UpdateProfileRequestModel requestModel)
    {
        var userId = ipersonAccessor.GetUserId();

        var customer = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted);

        if (customer == null)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);
        // Parse full name parts
        string firstName = null, lastName = null, middleName = null;
        if (!string.IsNullOrWhiteSpace(requestModel.FullName))
        {
            var nameParts = requestModel.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            firstName = nameParts.FirstOrDefault();
            lastName = nameParts.Length > 1 ? string.Join(' ', nameParts.Skip(1)) : "";
        }
        // Update simple fields safely
        if (!string.IsNullOrWhiteSpace(requestModel.FullName))
            customer.FullName = requestModel.FullName.Trim();
        customer.CourtesyTitle = requestModel.CourtesyTitle;
        customer.FirstName = firstName;
        customer.FullNameNepali = requestModel.FullNameNepali;
        customer.Gender = requestModel.Gender ?? customer.Gender;
        customer.PanNo = string.IsNullOrWhiteSpace(requestModel.PanNo)? null: requestModel.PanNo;
        customer.User.Email = requestModel.Email ?? customer.User.Email;
        customer.MaritalStatus = requestModel.MaritialStatus ?? customer.MaritalStatus;
        customer.DobAD = requestModel.DobAD;
        customer.DobBS = requestModel.DobBS;
        customer.FatherName = requestModel.FatherName;
        customer.GrandFatherName = requestModel.GrandFatherName;
        customer.User.Email = requestModel.Email;
        customer.MotherName = requestModel.MotherName;
        customer.IndividualType = requestModel.IndividualType;
        customer.CitizenshipNo = requestModel.CitizenshipNo ?? customer.CitizenshipNo;
        customer.NIDNumber = requestModel.NIDNumber ?? customer.NIDNumber;
        customer.CitizenshipIssueDistrict = requestModel.CitizenshipIssueDistrict ?? customer.CitizenshipIssueDistrict;
        customer.CitizenshipIssueDate = requestModel.CitizenshipIssueDate ?? customer.CitizenshipIssueDate;
        customer.PassportNumber = requestModel.PassportNumber ?? customer.PassportNumber;
        customer.PassportIssueDate = requestModel.PassportIssueDate ?? customer.PassportIssueDate;
        customer.PassportExpiryDate = requestModel.PassportExpiryDate ?? customer.PassportExpiryDate;
        customer.PassportIssuePlace = requestModel.PassportIssuePlace ?? customer.PassportIssuePlace;
        customer.VoterIdNumber = requestModel.VoterIdNumber ?? customer.VoterIdNumber;
        customer.LicenseNumber = requestModel.LicenseNumber ?? customer.LicenseNumber;
        customer.LicenseExpiryDate = requestModel.LicenseExpiryDate ?? customer.LicenseExpiryDate;
        customer.LicenseIssueDate = requestModel.LicenseIssueDate ?? customer.LicenseIssueDate;
        customer.Occupation = requestModel.Occupation ?? customer.Occupation;
        customer.UserPhoto = requestModel.UserPhoto;
        customer.IdFrontPhoto = requestModel.IdFrontPhoto;
        customer.IdBackPhoto = requestModel.IdBackPhoto;
        customer.LastName = lastName;
        customer.MiddleName = middleName;
        customer.DigitalSignature = requestModel.DigitalSignature;
        customer.MaritalStatus = requestModel.MaritialStatus;
        customer.Gender = requestModel.Gender;

        // Update or Add addresses
        customer.KycStatus = KYCStatus.Pending;
        if (requestModel.Addresses != null && requestModel.Addresses.Any())
        {
            UpdateOrAddAddress(customer, AddressTypeEnums.Permanent.ToString(), requestModel.Addresses);
            UpdateOrAddAddress(customer, AddressTypeEnums.Temporary.ToString(), requestModel.Addresses);
        }

      
        dbContext.Customers.Update(customer);
        dbContext.Users.Update(customer.User);
        await dbContext.SaveChangesAsync();

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Profile updated successfully."));
    }

    private void UpdateOrAddAddress(Customer customer, string addressType, IList<AddressResponseModel> addresses)
    {
        var newAddress = addresses.FirstOrDefault(a => a.AddressType == addressType);
        if (newAddress == null) return;

        var existing = customer.Addresses.FirstOrDefault(a => a.AddressType == addressType);
        if (existing != null)
        {
            existing.Province = newAddress.Province;
            existing.District = newAddress.District;
            existing.Municipality = newAddress.Municipality;
            existing.Ward = newAddress.Ward;
            existing.StreetAddress = newAddress.StreetAddress;
        }
        else
        {
            var mapped = MapCustomerAddress(newAddress);
            mapped.CustomerId = customer.Id;
            dbContext.Addresses.Add(mapped);
        }
    }

    private CustomerAddress MapCustomerAddress(AddressResponseModel address)
    {
        if (address == null) return null;

        return new CustomerAddress
        {
            AddressType = address.AddressType,
            Province = address.Province,
            District = address.District,
            Municipality = address.Municipality,
            Ward = address.Ward,
            StreetAddress = address.StreetAddress
        };
    }
}




