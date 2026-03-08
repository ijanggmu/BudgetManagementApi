using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using Models.Common;
using SharedKernel.Constant.ResponseConstant;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Profile;

public class AdminProfileService(
    ApplicationDataContext dbContext,
    UserManager<ApplicationUser> userManager,
    IUserProfileService userProfileService,
    ILogger<AdminProfileService> logger)
    : IAdminProfileService
{
    public async Task<Result<AdminUserProfileResponseModel>> GetProfileAsync(CancellationToken ct)
    {
        var userId = userProfileService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Result<AdminUserProfileResponseModel>.Failed(ResponseMessage.UserNotFound);

        //// Old-style LINQ query syntax with joins + projection
        //var profile = await
        //    (from admin in dbContext.Admins.AsNoTracking()
        //     join user in dbContext.Users.AsNoTracking()
        //         on admin.UserId equals user.Id
        //     join userRole in dbContext.UserRoles.AsNoTracking()
        //         on user.Id equals userRole.UserId into urGroup
        //     from ur in urGroup.DefaultIfEmpty()
        //     join role in dbContext.Roles.AsNoTracking()
        //         on ur.RoleId equals role.Id into rGroup
        //     from r in rGroup.DefaultIfEmpty()
        //     where admin.UserId == userId && !user.IsDeleted
        //     group r by new { admin.FullName, user.Email, user.PhoneNumber } into grp
        //     select new AdminUserProfileResponseModel
        //     {
        //         FullName = grp.Key.FullName,
        //         Email = grp.Key.Email,
        //         PhoneNumber = grp.Key.PhoneNumber,
        //         Roles = grp.Where(x => x != null).Select(x => x.Name).ToList()
        //     })
        //    .FirstOrDefaultAsync(ct);

        var profile = await
             (from user in dbContext.Users.AsNoTracking()
             join userRole in dbContext.UserRoles.AsNoTracking()
                 on user.Id equals userRole.UserId into urGroup
             from ur in urGroup.DefaultIfEmpty()
             join role in dbContext.Roles.AsNoTracking()
                 on ur.RoleId equals role.Id into rGroup
             from r in rGroup.DefaultIfEmpty()
             where user.Id == userId && !user.IsDeleted
             group r by new { user.UserName, user.Email, user.PhoneNumber, user.DepartmentId } into grp
             select new AdminUserProfileResponseModel
             {
                 FullName = grp.Key.UserName,
                 Email = grp.Key.Email,
                 PhoneNumber = grp.Key.PhoneNumber,
                 DepartmentId = grp.Key.DepartmentId ?? "",
                 Roles = grp.Where(x => x != null).Select(x => x.Name).ToList(),
                 RoleType = grp.Where(x => x != null).Select(x => x.RoleType ?? x.Name).FirstOrDefault() ?? ""
             })
            .FirstOrDefaultAsync(ct);

        if (profile == null)
            return Result<AdminUserProfileResponseModel>.Failed(ResponseMessage.UserNotFound);

        if (string.IsNullOrEmpty(profile.RoleType) && profile.Roles?.Count > 0)
            profile.RoleType = profile.Roles[0];

        return Result<AdminUserProfileResponseModel>.Success(profile);
    }


    public async Task<Result<MessageResponseModel>> UpdateProfileAsync(UpdateProfileRequestModel requestModel,CancellationToken ct)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

            var admin = await dbContext.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == userId && !a.User.IsDeleted, ct);

            if (admin == null)
                return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

            // Update admin properties
            if (!string.IsNullOrWhiteSpace(requestModel.FullName))
                admin.FullName = requestModel.FullName;

            // Update user properties
            if (!string.IsNullOrWhiteSpace(requestModel.Email) && requestModel.Email != admin.User.Email)
            {
                // Check if email already exists
                var emailExists = await userManager.Users
                    .AnyAsync(u => u.Email == requestModel.Email && u.Id != userId && !u.IsDeleted, ct);

                if (emailExists)
                    return Result<MessageResponseModel>.Failed("Email already exists.");

                admin.User.Email = requestModel.Email;
            }

            if (!string.IsNullOrWhiteSpace(requestModel.PhoneNumber))
                admin.User.PhoneNumber = requestModel.PhoneNumber;

            var updateResult = await userManager.UpdateAsync(admin.User);
            if (!updateResult.Succeeded)
                return Result<MessageResponseModel>.Failed(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

            dbContext.Admins.Update(admin);
            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result<MessageResponseModel>.Success(new MessageResponseModel("Profile updated successfully."));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
