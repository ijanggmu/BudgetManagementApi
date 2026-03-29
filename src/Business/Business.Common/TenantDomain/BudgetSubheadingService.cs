using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Budget;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class BudgetSubheadingService(
    ApplicationDataContext db,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension)
    : IBudgetSubheadingService
{
    public async Task<Result<List<BudgetSubheadingResponseDto>>> GetAllAsync(BudgetSubheadingListRequestModel? requestModel = null, CancellationToken cancellationToken = default)
    {
        requestModel ??= new BudgetSubheadingListRequestModel();
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.BudgetSubheadings.AsQueryable();
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            query = query.IgnoreQueryFilters().Where(s => s.TenantId == requestModel.TenantId);
        else if (isSuperAdmin)
            query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        if (!string.IsNullOrEmpty(requestModel.BudgetHeadingId))
            query = query.Where(s => s.BudgetHeadingId == requestModel.BudgetHeadingId);

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);
        var list = await result.OrderBy(s => s.Code).ToListAsync(cancellationToken);
        var dtos = list.Select(s => new BudgetSubheadingResponseDto
        {
            Id = s.Id,
            BudgetHeadingId = s.BudgetHeadingId,
            Name = s.Name,
            Code = s.Code,
            Description = s.Description
        }).ToList();
        return Result<List<BudgetSubheadingResponseDto>>.Success(dtos, new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        });
    }

    public async Task<Result<BudgetSubheadingResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.BudgetSubheadings.Where(s => s.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<BudgetSubheadingResponseDto>.Failed("Budget subheading not found.");
        return Result<BudgetSubheadingResponseDto>.Success(new BudgetSubheadingResponseDto
        {
            Id = entity.Id,
            BudgetHeadingId = entity.BudgetHeadingId,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description
        });
    }

    public async Task<Result<BudgetSubheadingResponseDto>> CreateAsync(CreateBudgetSubheadingDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        var tenantId = db.CurrentTenantId;

        var exists = await db.BudgetSubheadings.AnyAsync(s => s.Code == dto.Code && s.TenantId == tenantId, cancellationToken);

        if (exists) return Result<BudgetSubheadingResponseDto>.Failed("A budget subheading with this code already exists.");

        var entity = new BudgetSubheading
        {
            Id = Guid.NewGuid().ToString(),
            BudgetHeadingId = dto.BudgetHeadingId,
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            TenantId = tenantId,
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        };

        await db.BudgetSubheadings.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result<BudgetSubheadingResponseDto>.Success(new BudgetSubheadingResponseDto
        {
            Id = entity.Id,
            BudgetHeadingId = entity.BudgetHeadingId,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description
        });
    }

    public async Task<Result<BudgetSubheadingResponseDto>> UpdateAsync(string id, UpdateBudgetSubheadingDto dto, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.BudgetSubheadings.Where(s => s.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<BudgetSubheadingResponseDto>.Failed("Budget subheading not found.");
        if (dto.BudgetHeadingId != null) entity.BudgetHeadingId = dto.BudgetHeadingId;
        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.Code != null) entity.Code = dto.Code;
        if (dto.Description != null) entity.Description = dto.Description;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.BudgetSubheadings.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<BudgetSubheadingResponseDto>.Success(new BudgetSubheadingResponseDto
        {
            Id = entity.Id,
            BudgetHeadingId = entity.BudgetHeadingId,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description
        });
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.BudgetSubheadings.Where(s => s.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<bool>.Failed("Budget subheading not found.");
        entity.IsDeleted = true;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.BudgetSubheadings.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
