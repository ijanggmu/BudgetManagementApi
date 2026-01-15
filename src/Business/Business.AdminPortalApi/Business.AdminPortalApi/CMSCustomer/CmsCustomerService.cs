using Business.AdminPortalApi.Claim;
using Business.Common.File;
using Business.Common.JobHelper;
using Business.Common.Sms;
using Data.Context;
using Data.Entities.CustomerEntity;
using Data.Entities.Log;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Infrastructure.CoreApi.CoreApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.Common;
using Models.WebApi.Address;
using Models.WebApi.Individual;
using SharedKernel.Operation;
using SharedKernel.SystemEnum;

namespace Business.AdminPortalApi.CMSCustomer;

public class CmsCustomerService(ApplicationDataContext context, ILogger<CmsCustomerService> logger,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension,
    ISmsService smsService,
    IFileService fileService) : ICmsCustomerService
{

    public async Task<Result<List<CustomerResponseModel>>> GetAllCustomersAsync(CommonPaginationRequestModel requestModel, CancellationToken ct)
    {
        var query = context.Customers
                            .Include(x => x.User)
                            .Where(x => !x.IsDeleted && (x.KycStatus == KYCStatus.Pending || x.KycStatus == KYCStatus.Rejected))
                            .AsNoTracking();

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);

        var customers = (await result.ToListAsync(ct))
                                    .Select(c => new CustomerResponseModel
                                    {
                                        Id = c.Id,
                                        FullName = c.FullName,
                                        Email = c.User.Email,
                                        KycStatus = c.KycStatus.ToString(),
                                        KycRejectedReason = c.KycRejectedReason,
                                        CreatedOn = c.CreatedOn.ToString(),
                                        Phonenumber = c.User.PhoneNumber,
                                        Username = c.User.UserName

                                    }).ToList();

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };

        return Result<List<CustomerResponseModel>>.Success(customers, pagination);
    }
    public async Task<Result<GetKycResponseModel>> GetCustomerByIdAsync(string id, CancellationToken ct)
    {
        var customer = await context.Customers
                            .Include(x => x.User)
                            .Include(x => x.Addresses)
                            .Where(x => !x.IsDeleted && x.Id == id)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(ct);

        if (customer == null)
            return Result<GetKycResponseModel>.Failed("Customer not found.");

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
           .ToList() ?? [];

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
            Id = customer.Id,
            Username = customer.User.UserName,
            FullName = customer.FullName,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.User.Email,
            PartyId = customer.PartyID,
            PhoneNumber = customer.User.PhoneNumber,
            DateOfBirthAD = customer.DobAD,
            DateOfBirthBS = customer.DobBS,
            MaritalStatus = customer.MaritalStatus,
            Gender = customer.Gender,
            IndividualType = customer.IndividualType,
            CitizenshipNumber = customer.CitizenshipNo,
            CitizenshipIssueDistrict = customer.CitizenshipIssueDistrict,
            CitizenshipIssueDate = customer.CitizenshipIssueDate,
            CitizenshipNo = customer.CitizenshipNo,
            CourtesyTitle = customer.CourtesyTitle,
            FullNameNepali = customer.FullNameNepali,
            VoterIdNumber = customer.VoterIdNumber,
            PassportNumber = customer.PassportNumber,
            LicenseNumber = customer.LicenseNumber,
            LicenseIssueDate = customer.LicenseIssueDate,
            LicenseExpiryDate = customer.LicenseExpiryDate,
            PassportExpiryDate = customer.PassportExpiryDate,
            NidNumber = customer.NIDNumber,
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
            DocumentType = documentType,
            CreatedOn = customer.CreatedOn.ToString(),
            Addresses = addressModels,


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
    public enum DocumentType
    {
        Citizenship,
        Passport,
        PAN,
        VoterID,
        NID,
        License
    }

    public async Task<Result<MessageResponseModel>> ApproveCustomerKycAsync(string customerId, CancellationToken ct)
    {
        var approvedBy = userProfileService.GetUserId();

        var isAdmin = await context.Admins.Include(x => x.User).Where(x => x.Id == approvedBy).AnyAsync();
        if (!isAdmin || string.IsNullOrEmpty(approvedBy))
        {
            var superadmin = await context.Users
                                            .Where(x => x.UserName == "superadmin")
                                            .Select(x => new { x.Id, x.UserName })
                                            .FirstOrDefaultAsync(cancellationToken: ct);
            approvedBy = superadmin?.Id;
            userProfileService.SetUser(superadmin?.Id, superadmin?.UserName);
        }
        var customer = await context.Customers.Include(x => x.User)
                                                .Where(x => x.Id == customerId)
                                                .FirstOrDefaultAsync(cancellationToken: ct);

        if (customer == null)
            return Result<MessageResponseModel>.Failed("Customer not found.");

        if (customer.KycStatus == KYCStatus.Approved)
        {
            return Result<MessageResponseModel>.Failed("Kyc already approved .");
        }

        if (customer.KycStatus != KYCStatus.Pending && customer.KycStatus != KYCStatus.Rejected)
            return Result<MessageResponseModel>.Failed($"Cannot approved kyc. Your kyc is in {customer.KycStatus} status.");

        customer.KycStatus = KYCStatus.Approved;
        customer.KycRejectedReason = null;
        customer.KycReviewedAt = DateTime.UtcNow;
        customer.KycReviewedByUserId = approvedBy;

        var individualRequest = new CreateIndividualRequestModel
        {
            FullName = customer.FullName,
            MiddleName = customer.MiddleName,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            IndividualType = customer.IndividualType,
            CourtesyTitle = customer.CourtesyTitle,
            FullNameNepali = customer.FullNameNepali,
            Gender = customer.Gender,
            PanNo = customer.PanNo,
            MaritialStatus = customer.MaritalStatus,
            DobAD = DateTime.TryParse(customer.DobAD, out var dob) ? dob : DateTime.MinValue,
            DobBS = customer.DobBS,
            CitizenshipNo = customer.CitizenshipNo,
            CitizenshipIssueDistrict = customer.CitizenshipIssueDistrict,
            CitizenshipIssueDate = customer.CitizenshipIssueDate,
            PassportNumber = customer.PassportNumber,
            PassportIssueDate = customer.PassportIssueDate,
            PassportExpiryDate = customer.PassportExpiryDate,
            PassportIssuePlace = customer.PassportIssuePlace,
            VoterIdNumber = customer.VoterIdNumber,
            LicenseNumber = customer.LicenseNumber,
            TmProvince = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Temporary.ToString())?.Province,
            TmDistrict = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Temporary.ToString())?.District,
            TmMunicipality = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Temporary.ToString())?.Municipality,
            TmStreetAddress = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Temporary.ToString())?.StreetAddress,
            PaProvince = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Permanent.ToString())?.Province,
            PaDistrict = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Permanent.ToString())?.District,
            PaMunicipality = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Permanent.ToString())?.Municipality,
            PaStreetAddress = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Permanent.ToString())?.StreetAddress,
            PaWard = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Permanent.ToString())?.Ward,
            TmWard = customer.Addresses.FirstOrDefault(a => a.AddressType == AddressTypeEnums.Temporary.ToString())?.Ward,
            Email = customer.User.EmailConfirmed ? customer.User.Email : string.Empty,
            FirstNameNepali = customer.FirstName,
            LastNameNepali = customer.LastName,
            PhoneNumber = customer.User.PhoneNumber,
            OccupationJson = [customer.Occupation],
            MiddleNameNepali = customer.MiddleName,
        };

        context.Customers.Update(customer);
        await context.SaveChangesAsync(ct);
        if (string.IsNullOrEmpty(customer.DIANumber))
        {
            //Enqueue background job in a named queue "coreapi"
            //hangfireJobHelper.EnqueueWithLogging<ICustomerProfileJobService>(
            //     job => job.CreateIndividualAsync(individualRequest, customer.UserId),
            //     $"CreateIndividual job for {individualRequest.FullName}"
            // );

        }
        else
        {
            //Enqueue background job in a named queue "coreapi"
            //hangfireJobHelper.EnqueueWithLogging<ICustomerProfileJobService>(
            //     job => job.UpdateIndividualAsync(individualRequest, customer.UserId),
            //     $"CreateIndividual job for {individualRequest.FullName}"
            // );
        }
        var smsReqeuest = new SmsRequest
        {
            Body = "Congratulations! Your KYC has been successfully verified with Himalayan Everest Insurance. You're now fully onboarded. Thank you for trusting HEI.",
            To = [customer.User.PhoneNumber],
            SmsType = SmsType.CustomerKycApproval,

        };

        smsService.QueueSms(smsReqeuest);



        logger.LogInformation("KYC approved for Customer {CustomerId} by {ApprovedBy}", customerId, approvedBy);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Kyc approved successfully."));
    }

    public async Task<Result<MessageResponseModel>> RejectCustomerKycAsync(string customerId, string reason, CancellationToken ct)
    {
        var rejectedBy = userProfileService.GetUserId();

        var isAdmin = await context.Admins.Include(x => x.User).Where(x => x.Id == rejectedBy).AnyAsync();

        if (isAdmin || string.IsNullOrEmpty(rejectedBy))
        {
            var superadmin = await context.Users
                                            .Where(x => x.UserName == "superadmin")
                                            .Select(x => new { x.Id, x.UserName })
                                            .FirstOrDefaultAsync(cancellationToken: ct);
            rejectedBy = superadmin?.Id;
            userProfileService.SetUser(superadmin?.Id, superadmin?.UserName);
        }

        var customer = await context.Customers.Include(x => x.User)
                                                .Where(x => x.Id == customerId)
                                                .FirstOrDefaultAsync(cancellationToken: ct);

        if (customer == null)
            return Result<MessageResponseModel>.Failed("Customer not found.");

        if (customer.KycStatus != KYCStatus.Pending && customer.KycStatus != KYCStatus.Rejected)
            return Result<MessageResponseModel>.Failed($"Cannot reject kyc. Your kyc is in {customer.KycStatus} status.");

        customer.KycStatus = KYCStatus.Rejected;
        customer.KycRejectedReason = reason;
        customer.KycReviewedAt = DateTime.UtcNow;
        customer.KycReviewedByUserId = rejectedBy;
        context.Customers.Update(customer);

        await context.SaveChangesAsync();

        var smsReqeuest = new SmsRequest
        {
            Body = $"Your KYC verification with Himalayan Everest Insurance has been rejected. Reason: {reason}. Please update your information and try again. – Team HEI",
            To = [customer.User.PhoneNumber],
            SmsType = SmsType.CustomerKycRejection,

        };
        smsService.QueueSms(smsReqeuest);

        logger.LogWarning("KYC rejected for Customer {CustomerId} by {RejectedBy}. Reason: {Reason}", customerId, rejectedBy, reason);

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Kyc reject successfully."));
    }
}

