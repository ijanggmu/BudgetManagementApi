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

namespace Business.BeemaEdgeApi.Kyc;

public class CustomerKycService(
    ApplicationDataContext dbContext,
    IUserProfileService ipersonAccessor,
    IFileService fileService) : ICustomerKycService
{
    public async Task<Result<MessageResponseModel>> SetKycAsync(SetKycRequestModel requestModel)
    {
        var userId = ipersonAccessor.GetUserId();

        var customer = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            .Where(c => c.UserId == userId && !c.User.IsDeleted)
            .FirstOrDefaultAsync();

        if (customer == null)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        if (customer.KycStatus != KYCStatus.NotSubmitted)
            return Result<MessageResponseModel>.Failed("KYC is already submitted or approved. You cannot set it again.");

        if (!string.IsNullOrWhiteSpace(requestModel.CitizenshipNo))
        {
            var citizenshipExists = await dbContext.Customers
                .AnyAsync(c => c.CitizenshipNo == requestModel.CitizenshipNo && c.UserId != userId);
            if (citizenshipExists)
                return Result<MessageResponseModel>.Failed("Citizenship number already exists.");
        }

        if (!string.IsNullOrWhiteSpace(requestModel.LicenseNumber))
        {
            var licenseExists = await dbContext.Customers
                .AnyAsync(c => c.LicenseNumber == requestModel.LicenseNumber && c.UserId != userId);
            if (licenseExists)
                return Result<MessageResponseModel>.Failed("License number already exists.");
        }

        if (!string.IsNullOrWhiteSpace(requestModel.PassportNumber))
        {
            var passportExists = await dbContext.Customers
                .AnyAsync(c => c.PassportNumber == requestModel.PassportNumber && c.UserId != userId);
            if (passportExists)
                return Result<MessageResponseModel>.Failed("Passport number already exists.");
        }
        if (!string.IsNullOrWhiteSpace(requestModel.PanNo))
        {
            var panExists = await dbContext.Customers
                .AnyAsync(c => c.PanNo == requestModel.PanNo && c.UserId != userId);
            if (panExists)
                return Result<MessageResponseModel>.Failed("PAN number already exists.");
        }
        if (!string.IsNullOrWhiteSpace(requestModel.NIDNumber))
        {
            var nidExists = await dbContext.Customers
                .AnyAsync(c => c.NIDNumber == requestModel.NIDNumber && c.UserId != userId);
            if (nidExists)
                return Result<MessageResponseModel>.Failed("NID number already exists.");
        }
        // Parse full name parts
        string firstName = null, lastName = null;
        if (!string.IsNullOrWhiteSpace(requestModel.FullName))
        {
            var nameParts = requestModel.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            firstName = nameParts.FirstOrDefault();
            lastName = nameParts.Length > 1 ? string.Join(' ', nameParts.Skip(1)) : "";
        }
        // Update simple fields safely
        if (!string.IsNullOrWhiteSpace(requestModel.FullName))
            customer.FullName = requestModel.FullName.Trim();
        customer.FirstName = firstName;
        customer.Gender = requestModel.Gender ?? customer.Gender;
        if (!string.IsNullOrWhiteSpace(requestModel.PanNo))
            customer.PanNo = requestModel.PanNo.Trim();
        customer.MaritalStatus = requestModel.MaritialStatus ?? customer.MaritalStatus;
        customer.DobAD = requestModel.DobAD;
        customer.DobBS = requestModel.DobBS;
        customer.CitizenshipNo = requestModel.CitizenshipNo ?? customer.CitizenshipNo;
        customer.CitizenshipIssueDistrict = requestModel.CitizenshipIssueDistrict ?? customer.CitizenshipIssueDistrict;
        customer.CitizenshipIssueDate = requestModel.CitizenshipIssueDate ?? customer.CitizenshipIssueDate;
        customer.PassportNumber = requestModel.PassportNumber ?? customer.PassportNumber;
        customer.PassportIssueDate = requestModel.PassportIssueDate ?? customer.PassportIssueDate;
        customer.PassportExpiryDate = requestModel.PassportExpiryDate ?? customer.PassportExpiryDate;
        customer.PassportIssuePlace = requestModel.PassportIssuePlace ?? customer.PassportIssuePlace;
        customer.VoterIdNumber = requestModel.VoterIdNumber ?? customer.VoterIdNumber;
        customer.LicenseNumber = requestModel.LicenseNumber ?? customer.LicenseNumber;
        customer.NIDNumber = requestModel.NIDNumber ?? customer.NIDNumber;
        customer.LicenseIssueDate = requestModel.LicenseIssueDate ?? customer.LicenseIssueDate;
        customer.LicenseExpiryDate = requestModel.LicenseExpiryDate ?? customer.LicenseExpiryDate;
        customer.Occupation = requestModel.Occupation ?? customer.Occupation;
        customer.UserPhoto = requestModel.UserPhotoUrl;
        customer.IdFrontPhoto = requestModel.IdFrontPhotoUrl;
        customer.IdBackPhoto = requestModel.IdBackPhotoUrl;
        customer.LastName = lastName;
        customer.KycStatus = KYCStatus.Pending;
        customer.FatherName = requestModel.FatherName;
        customer.GrandFatherName = requestModel.GrandFatherName;
        customer.MotherName = requestModel.MotherName;
        customer.DigitalSignature = requestModel.DigitalSignatureUrl;
        customer.CourtesyTitle = requestModel.CourtesyTitle;
        customer.FullNameNepali = requestModel.FullNameNepali;
        customer.IndividualType = requestModel.IndividualType;
        // Update or Add addresses
        if (requestModel.Addresses != null && requestModel.Addresses.Any())
        {
            UpdateOrAddAddress(customer, AddressTypeEnums.Permanent.ToString(), requestModel.Addresses);
            UpdateOrAddAddress(customer, AddressTypeEnums.Temporary.ToString(), requestModel.Addresses);
        }

       

        // Save all changes atomically
        dbContext.Customers.Update(customer);
        dbContext.Users.Update(customer.User);
        await dbContext.SaveChangesAsync();


        return Result<MessageResponseModel>.Success(new MessageResponseModel("Kyc set successfully."));
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

    public async Task<Result<GetKycResponseModel>> GetKycAsync()
    {
        var userId = ipersonAccessor.GetUserId();

        var customer = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses) // Include addresses once here for re-use
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted);

        if (customer == null)
            return Result<GetKycResponseModel>.Failed(ResponseMessage.UserNotFound);

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

        var profile = new GetKycResponseModel
        {
            FullName = customer.FullName,
            MiddleName = customer.MiddleName,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.User.Email,
            PhoneNumber = customer.User.PhoneNumber,
            DateOfBirthAD = customer.DobAD,
            DateOfBirthBS = customer.DobBS,
            MaritalStatus = customer.MaritalStatus,
            Gender = customer.Gender,
            DocumentType = documentType,
            IndividualType = customer.IndividualType,
            CitizenshipNumber = customer.CitizenshipNo,
            Addresses = addressModels,
            CitizenshipIssueDistrict = customer.CitizenshipIssueDistrict,
            CitizenshipIssueDate = customer.CitizenshipIssueDate,
            CitizenshipNo = customer.CitizenshipNo,
            NidNumber = customer.NIDNumber,
            CourtesyTitle = customer.CourtesyTitle,
            DigitalSignature = customer.DigitalSignature,
            FullNameNepali = customer.FullNameNepali,
            VoterIdNumber = customer.VoterIdNumber,
            PassportNumber = customer.PassportNumber,
            LicenseNumber = customer.LicenseNumber,
            PassportExpiryDate = customer.PassportExpiryDate,
            PanNo = customer.PanNo,
            KycStatus = customer.KycStatus.ToString(),
            PassportIssuePlace = customer.PassportIssuePlace,
            Occupation = customer.Occupation,
            PassportIssueDate = customer.PassportIssueDate,
            GrandFatherName = customer.GrandFatherName,
            FatherName = customer.FatherName,
            MotherName = customer.MotherName,
            DIANumber = customer.DIANumber,
            PartyCode = customer.PartyCode,
            KycRejectReason = customer.KycRejectedReason,
            IdBackPhoto = customer.IdBackPhoto,
            IdFrontPhoto = customer.IdFrontPhoto,
            UserId = customer.UserId,
            UserPhoto = customer.UserPhoto,
            Username = customer.User.UserName,
            Id = customer.Id
        };

        if (!string.IsNullOrWhiteSpace(customer.UserPhoto))
            profile.UserPhotoUrl = await fileService.GetFilePresignedUrlAsync(customer.UserPhoto);
        if (!string.IsNullOrWhiteSpace(customer.IdFrontPhoto))
            profile.IdFrontPhotoUrl = await fileService.GetFilePresignedUrlAsync(customer.IdFrontPhoto);
        if (!string.IsNullOrWhiteSpace(customer.IdBackPhoto))
            profile.IdBackPhotoUrl = await fileService.GetFilePresignedUrlAsync(customer.IdBackPhoto);
        if (!string.IsNullOrWhiteSpace(customer.DigitalSignature))
            profile.DigitalSignatureUrl = await fileService.GetFilePresignedUrlAsync(customer.DigitalSignature);

        return Result<GetKycResponseModel>.Success(profile);
    }
}




