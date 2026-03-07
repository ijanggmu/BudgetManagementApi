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

public class BudgetService(
    ApplicationDataContext db,
    UserManager<Data.Entities.Identity.ApplicationUser> userManager,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension,
    IBudgetMemoAuditService auditService)
    : IBudgetService
{
    public async Task<Result<List<BudgetResponseDto>>> GetAllAsync(BudgetListRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        IQueryable<Budget> query = db.Budgets.AsQueryable();
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            query = query.IgnoreQueryFilters().Where(b => b.TenantId == requestModel.TenantId);
        else if (isSuperAdmin)
            query = query.IgnoreQueryFilters();
        if (!string.IsNullOrEmpty(requestModel.DepartmentId))
            query = query.Where(b => b.DepartmentId == requestModel.DepartmentId);
        if (!string.IsNullOrEmpty(requestModel.BudgetHeadingId))
            query = query.Where(b => b.BudgetHeadingId == requestModel.BudgetHeadingId);
        if (!string.IsNullOrEmpty(requestModel.BudgetSubheadingId))
            query = query.Where(b => b.BudgetSubheadingId == requestModel.BudgetSubheadingId);
        if (requestModel.Year.HasValue)
            query = query.Where(b => b.Year == requestModel.Year.Value);
        if (requestModel.Quarter.HasValue)
            query = query.Where(b => b.Quarter == requestModel.Quarter.Value);

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);
        var list = await result.ToListAsync(cancellationToken);
        var deptIds = list.Select(b => b.DepartmentId).Distinct().ToList();
        var headingIds = list.Select(b => b.BudgetHeadingId).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
        var subheadingIds = list.Select(b => b.BudgetSubheadingId).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
        var departments = await db.Departments.Where(d => deptIds.Contains(d.Id)).ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);
        var headings = headingIds.Count > 0 ? await db.BudgetHeadings.Where(h => headingIds.Contains(h.Id)).ToDictionaryAsync(h => h.Id, h => h.Name, cancellationToken) : new Dictionary<string, string>();
        var subheadings = subheadingIds.Count > 0 ? await db.BudgetSubheadings.Where(s => subheadingIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id, s => s.Name, cancellationToken) : new Dictionary<string, string>();

        var dtos = list.Select(b => new BudgetResponseDto
        {
            Id = b.Id,
            TenantId = b.TenantId ?? string.Empty,
            DepartmentId = b.DepartmentId,
            DepartmentName = departments.GetValueOrDefault(b.DepartmentId),
            BudgetHeadingId = b.BudgetHeadingId,
            BudgetHeadingName = b.BudgetHeadingId != null ? headings.GetValueOrDefault(b.BudgetHeadingId) : null,
            BudgetSubheadingId = b.BudgetSubheadingId,
            BudgetSubheadingName = b.BudgetSubheadingId != null ? subheadings.GetValueOrDefault(b.BudgetSubheadingId) : null,
            Year = b.Year,
            Quarter = b.Quarter,
            TotalAmount = b.TotalAmount,
            AllocatedAmount = b.AllocatedAmount,
            RemainingAmount = b.RemainingAmount,
            IsLocked = b.IsLocked,
            Version = b.Version,
            CreatedBy = b.CreatedBy,
            CreatedOn = b.CreatedOn
        }).ToList();

        return Result<List<BudgetResponseDto>>.Success(dtos, new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        });
    }

    public async Task<Result<BudgetResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.Budgets.Where(b => b.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<BudgetResponseDto>.Failed("Budget not found.");
        var deptName = await db.Departments.Where(d => d.Id == entity.DepartmentId).Select(d => d.Name).FirstOrDefaultAsync(cancellationToken);
        var headingName = entity.BudgetHeadingId != null ? await db.BudgetHeadings.Where(h => h.Id == entity.BudgetHeadingId).Select(h => h.Name).FirstOrDefaultAsync(cancellationToken) : null;
        var subheadingName = entity.BudgetSubheadingId != null ? await db.BudgetSubheadings.Where(s => s.Id == entity.BudgetSubheadingId).Select(s => s.Name).FirstOrDefaultAsync(cancellationToken) : null;
        return Result<BudgetResponseDto>.Success(new BudgetResponseDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId ?? string.Empty,
            DepartmentId = entity.DepartmentId,
            DepartmentName = deptName,
            BudgetHeadingId = entity.BudgetHeadingId,
            BudgetHeadingName = headingName,
            BudgetSubheadingId = entity.BudgetSubheadingId,
            BudgetSubheadingName = subheadingName,
            Year = entity.Year,
            Quarter = entity.Quarter,
            TotalAmount = entity.TotalAmount,
            AllocatedAmount = entity.AllocatedAmount,
            RemainingAmount = entity.RemainingAmount,
            IsLocked = entity.IsLocked,
            Version = entity.Version,
            CreatedBy = entity.CreatedBy,
            CreatedOn = entity.CreatedOn
        });
    }

    public async Task<Result<BudgetResponseDto>> CreateAsync(CreateBudgetDto dto, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var userId = userProfileService.GetUserId();
        var user = await userManager.FindByIdAsync(userId);
        var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;
        var dept = await db.Departments.FirstOrDefaultAsync(d => d.Id == dto.DepartmentId && d.TenantId == tenantId, cancellationToken);
        if (dept == null) return Result<BudgetResponseDto>.Failed("Department not found.");
        var exists = await db.Budgets.AnyAsync(b => b.DepartmentId == dto.DepartmentId && b.Year == dto.Year && b.Quarter == dto.Quarter && b.TenantId == tenantId, cancellationToken);
        if (exists) return Result<BudgetResponseDto>.Failed("Budget for this department, year and quarter already exists.");
        var entity = new Budget
        {
            Id = Guid.NewGuid().ToString(),
            DepartmentId = dto.DepartmentId,
            BudgetHeadingId = dto.BudgetHeadingId,
            BudgetSubheadingId = dto.BudgetSubheadingId,
            Year = dto.Year,
            Quarter = dto.Quarter,
            TotalAmount = dto.TotalAmount,
            AllocatedAmount = 0,
            RemainingAmount = dto.TotalAmount,
            IsLocked = false,
            Version = 1,
            TenantId = tenantId,
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        };
        await db.Budgets.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        await auditService.LogAsync("Budget", entity.Id, "Created", $"Dept {dept.Name}, Year {entity.Year} Q{entity.Quarter}", cancellationToken);
        return Result<BudgetResponseDto>.Success(new BudgetResponseDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId ?? string.Empty,
            DepartmentId = entity.DepartmentId,
            DepartmentName = dept.Name,
            BudgetHeadingId = entity.BudgetHeadingId,
            BudgetSubheadingId = entity.BudgetSubheadingId,
            Year = entity.Year,
            Quarter = entity.Quarter,
            TotalAmount = entity.TotalAmount,
            AllocatedAmount = entity.AllocatedAmount,
            RemainingAmount = entity.RemainingAmount,
            IsLocked = entity.IsLocked,
            Version = entity.Version,
            CreatedBy = entity.CreatedBy,
            CreatedOn = entity.CreatedOn
        });
    }

    public async Task<Result<BudgetResponseDto>> UpdateAsync(string id, UpdateBudgetDto dto, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.Budgets.Where(b => b.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<BudgetResponseDto>.Failed("Budget not found.");
        if (entity.IsLocked) return Result<BudgetResponseDto>.Failed("Budget is locked. Unlock it first to modify.");
        if (dto.TotalAmount.HasValue) entity.TotalAmount = dto.TotalAmount.Value;
        if (dto.DepartmentId != null) entity.DepartmentId = dto.DepartmentId;
        if (dto.BudgetHeadingId != null) entity.BudgetHeadingId = dto.BudgetHeadingId;
        if (dto.BudgetSubheadingId != null) entity.BudgetSubheadingId = dto.BudgetSubheadingId;
        if (dto.Year.HasValue) entity.Year = dto.Year.Value;
        if (dto.Quarter.HasValue) entity.Quarter = dto.Quarter.Value;
        entity.RemainingAmount = entity.TotalAmount - entity.AllocatedAmount;
        entity.Version++;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.Budgets.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        await auditService.LogAsync("Budget", entity.Id, "Updated", $"Version {entity.Version}", cancellationToken);
        var deptName = await db.Departments.Where(d => d.Id == entity.DepartmentId).Select(d => d.Name).FirstOrDefaultAsync(cancellationToken);
        var headingName = entity.BudgetHeadingId != null ? await db.BudgetHeadings.Where(h => h.Id == entity.BudgetHeadingId).Select(h => h.Name).FirstOrDefaultAsync(cancellationToken) : null;
        var subheadingName = entity.BudgetSubheadingId != null ? await db.BudgetSubheadings.Where(s => s.Id == entity.BudgetSubheadingId).Select(s => s.Name).FirstOrDefaultAsync(cancellationToken) : null;
        return Result<BudgetResponseDto>.Success(new BudgetResponseDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId ?? string.Empty,
            DepartmentId = entity.DepartmentId,
            DepartmentName = deptName,
            BudgetHeadingId = entity.BudgetHeadingId,
            BudgetHeadingName = headingName,
            BudgetSubheadingId = entity.BudgetSubheadingId,
            BudgetSubheadingName = subheadingName,
            Year = entity.Year,
            Quarter = entity.Quarter,
            TotalAmount = entity.TotalAmount,
            AllocatedAmount = entity.AllocatedAmount,
            RemainingAmount = entity.RemainingAmount,
            IsLocked = entity.IsLocked,
            Version = entity.Version,
            CreatedBy = entity.CreatedBy,
            CreatedOn = entity.CreatedOn
        });
    }

    public async Task<Result<BudgetResponseDto>> SetLockAsync(string id, bool isLocked, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.Budgets.Where(b => b.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<BudgetResponseDto>.Failed("Budget not found.");
        entity.IsLocked = isLocked;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.Budgets.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        await auditService.LogAsync("Budget", entity.Id, isLocked ? "Locked" : "Unlocked", null, cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.Budgets.Where(b => b.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<bool>.Failed("Budget not found.");
        entity.IsDeleted = true;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.Budgets.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        await auditService.LogAsync("Budget", entity.Id, "Deleted", null, cancellationToken);
        return Result<bool>.Success(true);
    }
}