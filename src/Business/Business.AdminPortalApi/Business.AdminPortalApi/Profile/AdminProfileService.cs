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
    public async Task<Result<AdminUserProfileResponseModel>> GetProfileAsync()
    {
        var userId = ipersonAccessor.GetUserId();
        var Admin = await dbContext.Admins
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted);

        if (Admin == null)
            return Result<AdminUserProfileResponseModel>.Failed(ResponseMessage.UserNotFound);

        var profile = new AdminUserProfileResponseModel
        {
            FullName = Admin.FullName,
            Email = Admin.User.Email,
            PhoneNumber = Admin.User.PhoneNumber,



        };

        return Result<AdminUserProfileResponseModel>.Success(profile);
    }

    public async Task<Result<MessageResponseModel>> UpdateProfileAsync(UpdateProfileRequestModel requestModel)
    {
        var userId = ipersonAccessor.GetUserId();

        var admin = await dbContext.Admins
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted);

        if (admin == null)
            return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

        if (!string.IsNullOrWhiteSpace(requestModel.FullName))
            admin.FullName = requestModel.FullName;


        dbContext.Admins.Update(admin);
        dbContext.Users.Update(admin.User);
        await dbContext.SaveChangesAsync();

        return Result<MessageResponseModel>.Success(new MessageResponseModel("Profile updated successfully."));
    }

}
