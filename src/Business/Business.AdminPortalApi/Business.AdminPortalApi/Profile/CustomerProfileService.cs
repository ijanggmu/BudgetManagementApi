using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.WebApi.Address;
using SharedKernel.Constant.ResponseConstant;
using SharedKernel.Operation;
using SharedKernel.SystemEnum;

namespace Business.AdminPortalApi.Profile;

public class AdminProfileService(
    ApplicationDataContext dbContext,
    UserManager<ApplicationUser> userManager,
    IUserProfileService ipersonAccessor)
    : IAdminProfileService
{
    public async Task<Result<UserProfileResponseModel>> GetProfileAsync()
    {
        var userId = ipersonAccessor.GetUserId();
        var Admin = await dbContext.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted);

        if (Admin == null)
            return Result<UserProfileResponseModel>.Failed(ResponseMessage.UserNotFound);
        var addressModels = Admin.Addresses?
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
        }).ToList() ?? new List<AddressResponseModel>();

        var profile = new UserProfileResponseModel
        {
            FullName = Admin.FullName,
            Email = Admin.User.Email,
            PhoneNumber = Admin.User.PhoneNumber,
            DateOfBirthAD = Admin.DobAD,
            DateOfBirthBS = Admin.DobBS,
            MaritalStatus = Admin.MaritalStatus,
            Gender = Admin.Gender,
            IndividualType = Admin.IndividualType,
            CitizenshipNumber = Admin.CitizenshipNo,
            Addresses = addressModels
        };

        return Result<UserProfileResponseModel>.Success(profile);
    }

    public async Task<Result<MessageResponseModel>> UpdateProfileAsync(UpdateProfileRequestModel requestModel)
    {
        var userId = ipersonAccessor.GetUserId();

        var Admin = await dbContext.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted);

        if (Admin == null)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        if (!string.IsNullOrWhiteSpace(requestModel.FullName))
            Admin.FullName = requestModel.FullName;

        Admin.Gender = requestModel.Gender ?? Admin.Gender;
        Admin.PanNo = requestModel.PanNo ?? Admin.PanNo;
        Admin.MaritalStatus = requestModel.MaritialStatus ?? Admin.MaritalStatus;
        Admin.DobAD = requestModel.DobAD;
        Admin.DobBS = requestModel.DobBS;
        Admin.CitizenshipNo = requestModel.CitizenshipNo ?? Admin.CitizenshipNo;
        Admin.CitizenshipIssueDistrict = requestModel.CitizenshipIssueDistrict ?? Admin.CitizenshipIssueDistrict;
        Admin.CitizenshipIssueDate = requestModel.CitizenshipIssueDate ?? Admin.CitizenshipIssueDate;
        Admin.PassportNumber = requestModel.PassportNumber ?? Admin.PassportNumber;
        Admin.PassportIssueDate = requestModel.PassportIssueDate ?? Admin.PassportIssueDate;
        Admin.PassportExpiryDate = requestModel.PassportExpiryDate ?? Admin.PassportExpiryDate;
        Admin.PassportIssuePlace = requestModel.PassportIssuePlace ?? Admin.PassportIssuePlace;
        Admin.VoterIdNumber = requestModel.VoterIdNumber ?? Admin.VoterIdNumber;
        Admin.LicenseNumber = requestModel.LicenseNumber ?? Admin.LicenseNumber;
        Admin.Occupation = requestModel.Occupation ?? Admin.Occupation;

        dbContext.Customers.Update(Admin);
        dbContext.Users.Update(Admin.User);
        await dbContext.SaveChangesAsync();

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Profile updated successfully."));
    }

}
