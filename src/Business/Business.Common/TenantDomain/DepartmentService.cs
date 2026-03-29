using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Department;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class DepartmentService(
    ApplicationDataContext db,
    UserManager<Data.Entities.Identity.ApplicationUser> userManager,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension)
    : IDepartmentService
{
    public async Task<Result<List<DepartmentResponseDto>>> GetAllAsync(DepartmentListRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();

        IQueryable<Department> query = db.Departments;
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            query = query.IgnoreQueryFilters().Where(d => d.TenantId == requestModel.TenantId);
        else if (isSuperAdmin)
            query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);
        var list = await result.ToListAsync(cancellationToken);

        var dtos = list.Select(d => new DepartmentResponseDto
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            IsActive = d.IsActive,
            TenantId = d.TenantId ?? string.Empty,
            CreatedOn = d.CreatedOn
        }).ToList();

        return Result<List<DepartmentResponseDto>>.Success(dtos, new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        });
    }

    public async Task<Result<DepartmentResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();

        var query = db.Departments.Where(d => d.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<DepartmentResponseDto>.Failed("Department not found.");

        return Result<DepartmentResponseDto>.Success(new DepartmentResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            TenantId = entity.TenantId ?? string.Empty,
            CreatedOn = entity.CreatedOn
        });
    }

    public async Task<Result<DepartmentResponseDto>> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var userId = userProfileService.GetUserId();
        var user = await userManager.FindByIdAsync(userId);
        var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;

        var nameExists = await db.Departments.AnyAsync(d => d.Name == dto.Name && d.TenantId == tenantId, cancellationToken);
        if (nameExists) return Result<DepartmentResponseDto>.Failed("Department name already exists.");

        var entity = new Department
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            IsActive = true,
            TenantId = tenantId,
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        };
        await db.Departments.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result<DepartmentResponseDto>.Success(new DepartmentResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            TenantId = entity.TenantId ?? string.Empty,
            CreatedOn = entity.CreatedOn
        });
    }

    public async Task<Result<DepartmentResponseDto>> UpdateAsync(string id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.Departments.Where(d => d.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<DepartmentResponseDto>.Failed("Department not found.");

        if (dto.Name != entity.Name && await db.Departments.AnyAsync(d => d.TenantId == entity.TenantId && d.Name == dto.Name && d.Id != id, cancellationToken))
            return Result<DepartmentResponseDto>.Failed("Department name already exists.");

        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description?.Trim();
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.Departments.Update(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Result<DepartmentResponseDto>.Success(new DepartmentResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            TenantId = entity.TenantId ?? string.Empty,
            CreatedOn = entity.CreatedOn
        });
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var isSuperAdmin = userProfileService.IsSuperAdmin();
        var query = db.Departments.Where(d => d.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<bool>.Failed("Department not found.");

        entity.IsDeleted = true;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.Departments.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
