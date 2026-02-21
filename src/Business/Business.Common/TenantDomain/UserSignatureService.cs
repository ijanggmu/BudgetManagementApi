using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.UserSignature;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class UserSignatureService(
    ApplicationDataContext db,
    IUserProfileService userProfileService)
    : IUserSignatureService
{
    public async Task<Result<UserSignatureResponseDto>> GetCurrentUserSignatureAsync(CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        var entity = await db.UserSignatures.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
        if (entity == null)
            return Result<UserSignatureResponseDto>.Failed("No signature found for current user.");
        return Result<UserSignatureResponseDto>.Success(new UserSignatureResponseDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            SignatureUrl = entity.SignatureUrl,
            UploadedAt = entity.UploadedAt
        });
    }

    public async Task<Result<UserSignatureResponseDto>> SetSignatureUrlAsync(string signatureUrl, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var tenantId = db.CurrentTenantId;
        if (isSuperAdmin)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            tenantId = user?.TenantId;
        }
        var entity = await db.UserSignatures.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.UserId == userId && s.TenantId == tenantId, cancellationToken);
        if (entity != null)
        {
            entity.SignatureUrl = signatureUrl;
            entity.UploadedAt = DateTime.UtcNow;
            entity.LastModifiedBy = userId;
            entity.LastModifiedOn = DateTime.UtcNow;
            db.UserSignatures.Update(entity);
        }
        else
        {
            entity = new UserSignature
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                SignatureUrl = signatureUrl,
                UploadedAt = DateTime.UtcNow,
                TenantId = tenantId ?? "",
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow
            };
            await db.UserSignatures.AddAsync(entity, cancellationToken);
        }
        await db.SaveChangesAsync(cancellationToken);
        return Result<UserSignatureResponseDto>.Success(new UserSignatureResponseDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            SignatureUrl = entity.SignatureUrl,
            UploadedAt = entity.UploadedAt
        });
    }
}
