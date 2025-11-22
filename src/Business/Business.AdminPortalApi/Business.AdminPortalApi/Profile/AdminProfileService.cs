using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.AdminEntity;
using Data.Entities.Identity;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
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
    public async Task<Result<AdminUserProfileResponseModel>> GetProfileAsync()
    {
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<AdminUserProfileResponseModel>.Failed(ResponseMessage.UserNotFound);

            var admin = await dbContext.Admins
                .Include(a => a.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.UserId == userId && !a.User.IsDeleted);

            if (admin == null)
                return Result<AdminUserProfileResponseModel>.Failed(ResponseMessage.UserNotFound);

            var profile = new AdminUserProfileResponseModel
            {
                FullName = admin.FullName,
                Email = admin.User.Email,
                PhoneNumber = admin.User.PhoneNumber,
                Gender = null, // Add if Admin entity has these fields
                MaritalStatus = null // Add if Admin entity has these fields
            };

            return Result<AdminUserProfileResponseModel>.Success(profile);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving admin profile: {Message}", ex.Message);
            return Result<AdminUserProfileResponseModel>.Failed($"An error occurred while retrieving profile: {ex.Message}");
        }
    }

    public async Task<Result<MessageResponseModel>> UpdateProfileAsync(UpdateProfileRequestModel requestModel)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var userId = userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<MessageResponseModel>.Failed(ResponseMessage.UserNotFound);

            var admin = await dbContext.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == userId && !a.User.IsDeleted);

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
                    .AnyAsync(u => u.Email == requestModel.Email && u.Id != userId && !u.IsDeleted);

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
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<MessageResponseModel>.Success(new MessageResponseModel("Profile updated successfully."));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Error updating admin profile: {Message}", ex.Message);
            return Result<MessageResponseModel>.Failed($"An error occurred while updating profile: {ex.Message}");
        }
    }
}
