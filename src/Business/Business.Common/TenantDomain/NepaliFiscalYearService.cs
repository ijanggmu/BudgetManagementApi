using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Budget;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class NepaliFiscalYearService(
    ApplicationDataContext db,
    UserManager<Data.Entities.Identity.ApplicationUser> userManager,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension)
    : INepaliFiscalYearService
{
    public async Task<Result<List<NepaliFiscalYearResponseDto>>> GetAllAsync(NepaliFiscalYearListRequestModel? requestModel = null, CancellationToken cancellationToken = default)
    {
        requestModel ??= new NepaliFiscalYearListRequestModel();
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        IQueryable<NepaliFiscalYear> query = db.NepaliFiscalYears.AsQueryable();
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            query = query.IgnoreQueryFilters().Where(f => f.TenantId == requestModel.TenantId && !f.IsDeleted);
        else if (isSuperAdmin)
            query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);
        var list = await result.OrderBy(f => f.StartDateUtc).ToListAsync(cancellationToken);
        var dtos = list.Select(f => new NepaliFiscalYearResponseDto
        {
            Id = f.Id,
            Code = f.Code,
            StartDateUtc = f.StartDateUtc,
            EndDateUtc = f.EndDateUtc,
            Description = f.Description
        }).ToList();

        return Result<List<NepaliFiscalYearResponseDto>>.Success(dtos, new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        });
    }

    public async Task<Result<NepaliFiscalYearResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.NepaliFiscalYears.Where(f => f.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<NepaliFiscalYearResponseDto>.Failed("Fiscal year not found.");
        return Result<NepaliFiscalYearResponseDto>.Success(new NepaliFiscalYearResponseDto
        {
            Id = entity.Id,
            Code = entity.Code,
            StartDateUtc = entity.StartDateUtc,
            EndDateUtc = entity.EndDateUtc,
            Description = entity.Description
        });
    }

    public async Task<Result<NepaliFiscalYearResponseDto>> CreateAsync(CreateNepaliFiscalYearDto dto, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var userId = userProfileService.GetUserId();
        var user = await userManager.FindByIdAsync(userId);
        var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;
        if (string.IsNullOrEmpty(tenantId)) return Result<NepaliFiscalYearResponseDto>.Failed("Tenant not found.");
        var exists = await db.NepaliFiscalYears.AnyAsync(f => f.Code == dto.Code && f.TenantId == tenantId, cancellationToken);
        if (exists) return Result<NepaliFiscalYearResponseDto>.Failed("A fiscal year with this code already exists.");

        var startUtc = dto.StartDateUtc.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dto.StartDateUtc, DateTimeKind.Utc) : dto.StartDateUtc.ToUniversalTime();
        var endUtc = dto.EndDateUtc.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dto.EndDateUtc, DateTimeKind.Utc) : dto.EndDateUtc.ToUniversalTime();
        if (startUtc >= endUtc) return Result<NepaliFiscalYearResponseDto>.Failed("Start date must be before end date.");

        var othersQuery = db.NepaliFiscalYears.Where(f => f.TenantId == tenantId);
        var overlapping = await othersQuery.AnyAsync(f => startUtc < f.EndDateUtc && f.StartDateUtc < endUtc, cancellationToken);
        if (overlapping) return Result<NepaliFiscalYearResponseDto>.Failed("This fiscal year overlaps with an existing one. Dates cannot overlap.");

        var entity = new NepaliFiscalYear
        {
            Id = Guid.NewGuid().ToString(),
            Code = dto.Code,
            StartDateUtc = startUtc,
            EndDateUtc = endUtc,
            Description = dto.Description,
            TenantId = tenantId,
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        };
        await db.NepaliFiscalYears.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return Result<NepaliFiscalYearResponseDto>.Success(new NepaliFiscalYearResponseDto
        {
            Id = entity.Id,
            Code = entity.Code,
            StartDateUtc = entity.StartDateUtc,
            EndDateUtc = entity.EndDateUtc,
            Description = entity.Description
        });
    }

    public async Task<Result<NepaliFiscalYearResponseDto>> UpdateAsync(string id, UpdateNepaliFiscalYearDto dto, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.NepaliFiscalYears.Where(f => f.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<NepaliFiscalYearResponseDto>.Failed("Fiscal year not found.");
        if (dto.Code != null) entity.Code = dto.Code;
        var startUtc = entity.StartDateUtc;
        var endUtc = entity.EndDateUtc;
        if (dto.StartDateUtc.HasValue)
            startUtc = dto.StartDateUtc.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dto.StartDateUtc.Value, DateTimeKind.Utc) : dto.StartDateUtc.Value.ToUniversalTime();
        if (dto.EndDateUtc.HasValue)
            endUtc = dto.EndDateUtc.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dto.EndDateUtc.Value, DateTimeKind.Utc) : dto.EndDateUtc.Value.ToUniversalTime();
        if (startUtc >= endUtc) return Result<NepaliFiscalYearResponseDto>.Failed("Start date must be before end date.");
        var tenantId = entity.TenantId;
        var overlapping = await db.NepaliFiscalYears.AnyAsync(f => f.TenantId == tenantId && f.Id != id && startUtc < f.EndDateUtc && f.StartDateUtc < endUtc, cancellationToken);
        if (overlapping) return Result<NepaliFiscalYearResponseDto>.Failed("This fiscal year overlaps with an existing one. Dates cannot overlap.");
        entity.StartDateUtc = startUtc;
        entity.EndDateUtc = endUtc;
        if (dto.Description != null) entity.Description = dto.Description;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.NepaliFiscalYears.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<NepaliFiscalYearResponseDto>.Success(new NepaliFiscalYearResponseDto
        {
            Id = entity.Id,
            Code = entity.Code,
            StartDateUtc = entity.StartDateUtc,
            EndDateUtc = entity.EndDateUtc,
            Description = entity.Description
        });
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.NepaliFiscalYears.Where(f => f.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<bool>.Failed("Fiscal year not found.");
        entity.IsDeleted = true;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.NepaliFiscalYears.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
