using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Budget;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class BudgetHeadingService(
    ApplicationDataContext db,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension) : IBudgetHeadingService
{
    public async Task<Result<List<BudgetHeadingResponseDto>>> GetAllAsync(BudgetHeadingListRequestModel? requestModel = null, CancellationToken cancellationToken = default)
    {
        requestModel ??= new BudgetHeadingListRequestModel();
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.BudgetHeadings.AsQueryable();
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            query = query.IgnoreQueryFilters().Where(h => h.TenantId == requestModel.TenantId);
        else if (isSuperAdmin)
            query = query.IgnoreQueryFilters();

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);
        var list = await result.OrderBy(h => h.Code).ToListAsync(cancellationToken);
        var dtos = list.Select(h => new BudgetHeadingResponseDto { Id = h.Id, Name = h.Name, Code = h.Code, Description = h.Description }).ToList();
        return Result<List<BudgetHeadingResponseDto>>.Success(dtos, new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        });
    }

    public async Task<Result<BudgetHeadingResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.BudgetHeadings.Where(h => h.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<BudgetHeadingResponseDto>.Failed("Budget heading not found.");
        return Result<BudgetHeadingResponseDto>.Success(new BudgetHeadingResponseDto { Id = entity.Id, Name = entity.Name, Code = entity.Code, Description = entity.Description });
    }

    public async Task<Result<BudgetHeadingResponseDto>> CreateAsync(CreateBudgetHeadingDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        var tenantId = db.CurrentTenantId;
        var exists = await db.BudgetHeadings.AnyAsync(h => h.Code == dto.Code && h.TenantId == tenantId, cancellationToken);
        if (exists) return Result<BudgetHeadingResponseDto>.Failed("A budget heading with this code already exists.");
        var entity = new BudgetHeading { Id = Guid.NewGuid().ToString(), Name = dto.Name, Code = dto.Code, Description = dto.Description, TenantId = tenantId, CreatedBy = userId, CreatedOn = DateTime.UtcNow };
        await db.BudgetHeadings.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return Result<BudgetHeadingResponseDto>.Success(new BudgetHeadingResponseDto { Id = entity.Id, Name = entity.Name, Code = entity.Code, Description = entity.Description });
    }

    public async Task<Result<BudgetHeadingResponseDto>> UpdateAsync(string id, UpdateBudgetHeadingDto dto, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.BudgetHeadings.Where(h => h.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<BudgetHeadingResponseDto>.Failed("Budget heading not found.");
        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.Code != null) entity.Code = dto.Code;
        if (dto.Description != null) entity.Description = dto.Description;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.BudgetHeadings.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<BudgetHeadingResponseDto>.Success(new BudgetHeadingResponseDto { Id = entity.Id, Name = entity.Name, Code = entity.Code, Description = entity.Description });
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.BudgetHeadings.Where(h => h.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<bool>.Failed("Budget heading not found.");
        entity.IsDeleted = true;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.BudgetHeadings.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
