using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Memo;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class MemoTemplateService(ApplicationDataContext db, IUserProfileService userProfileService) : IMemoTemplateService
{
    public async Task<Result<List<MemoTemplateResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.MemoTemplates.AsQueryable();
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var list = await query.OrderBy(t => t.Name).ToListAsync(cancellationToken);
        var dtos = list.Select(t => new MemoTemplateResponseDto { Id = t.Id, Name = t.Name, Description = t.Description, BodyTemplate = t.BodyTemplate }).ToList();
        return Result<List<MemoTemplateResponseDto>>.Success(dtos);
    }

    public async Task<Result<MemoTemplateResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.MemoTemplates.Where(t => t.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<MemoTemplateResponseDto>.Failed("Memo template not found.");
        return Result<MemoTemplateResponseDto>.Success(new MemoTemplateResponseDto { Id = entity.Id, Name = entity.Name, Description = entity.Description, BodyTemplate = entity.BodyTemplate });
    }

    public async Task<Result<MemoTemplateResponseDto>> CreateAsync(CreateMemoTemplateDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        var tenantId = db.CurrentTenantId;
        var exists = await db.MemoTemplates.AnyAsync(t => t.Name == dto.Name && t.TenantId == tenantId, cancellationToken);
        if (exists) return Result<MemoTemplateResponseDto>.Failed("A memo template with this name already exists.");
        var entity = new MemoTemplate { Id = Guid.NewGuid().ToString(), Name = dto.Name, Description = dto.Description, BodyTemplate = dto.BodyTemplate, TenantId = tenantId, CreatedBy = userId, CreatedOn = DateTime.UtcNow };
        await db.MemoTemplates.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return Result<MemoTemplateResponseDto>.Success(new MemoTemplateResponseDto { Id = entity.Id, Name = entity.Name, Description = entity.Description, BodyTemplate = entity.BodyTemplate });
    }

    public async Task<Result<MemoTemplateResponseDto>> UpdateAsync(string id, UpdateMemoTemplateDto dto, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.MemoTemplates.Where(t => t.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<MemoTemplateResponseDto>.Failed("Memo template not found.");
        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.BodyTemplate != null) entity.BodyTemplate = dto.BodyTemplate;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.MemoTemplates.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<MemoTemplateResponseDto>.Success(new MemoTemplateResponseDto { Id = entity.Id, Name = entity.Name, Description = entity.Description, BodyTemplate = entity.BodyTemplate });
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.MemoTemplates.Where(t => t.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<bool>.Failed("Memo template not found.");
        entity.IsDeleted = true;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.MemoTemplates.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
