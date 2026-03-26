using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.UserSignature;
using Business.Common.File;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class UserSignatureService(
    ApplicationDataContext db,
    IUserProfileService userProfileService,
    IFileService fileService)
    : IUserSignatureService
{
    public async Task<Result<UserSignatureResponseDto>> GetCurrentUserSignatureAsync(CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        var entity = await db.UserSignatures.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
        if (entity == null)
            return Result<UserSignatureResponseDto>.Failed("No signature found for current user.");
        return Result<UserSignatureResponseDto>.Success(await MapToResponseAsync(entity));
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
                TenantId = tenantId,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow
            };
            await db.UserSignatures.AddAsync(entity, cancellationToken);
        }
        await db.SaveChangesAsync(cancellationToken);
        return Result<UserSignatureResponseDto>.Success(await MapToResponseAsync(entity));
    }

    private async Task<UserSignatureResponseDto> MapToResponseAsync(UserSignature entity)
    {
        var displayUrl = await ToRenderableSignatureUrlAsync(entity.SignatureUrl);
        return new UserSignatureResponseDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            SignatureUrl = displayUrl,
            UploadedAt = entity.UploadedAt
        };
    }

    /// <summary>
    /// Stored value is typically a MinIO object key (FilePath). For rendering in the browser, return a presigned GET URL.
    /// If the stored value is already an absolute HTTP(S) URL, return it unchanged.
    /// </summary>
    private async Task<string> ToRenderableSignatureUrlAsync(string signatureUrl)
    {
        if (string.IsNullOrWhiteSpace(signatureUrl))
            return signatureUrl;

        if (Uri.TryCreate(signatureUrl, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            return signatureUrl;

        var signedUrl = await fileService.GetFilePresignedUrlAsync(signatureUrl);
        return string.IsNullOrWhiteSpace(signedUrl) ? signatureUrl : signedUrl;
    }
}
