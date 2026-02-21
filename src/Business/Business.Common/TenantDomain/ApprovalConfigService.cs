using System.Text.Json;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.ApprovalConfig;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class ApprovalConfigService(
    ApplicationDataContext db,
    UserManager<Data.Entities.Identity.ApplicationUser> userManager,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension)
    : IApprovalConfigService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<Result<List<ApprovalConfigResponseDto>>> GetAllAsync(ApprovalConfigListRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        IQueryable<ApprovalConfig> query = db.ApprovalConfigs.AsQueryable();
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            query = query.IgnoreQueryFilters().Where(a => a.TenantId == requestModel.TenantId);
        else if (isSuperAdmin)
            query = query.IgnoreQueryFilters();
        if (!string.IsNullOrEmpty(requestModel.DepartmentId))
            query = query.Where(a => a.DepartmentId == requestModel.DepartmentId);

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);
        var list = await result.ToListAsync(cancellationToken);
        var deptIds = list.Select(a => a.DepartmentId).Distinct().ToList();
        var departments = await db.Departments.Where(d => deptIds.Contains(d.Id)).ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        var dtos = list.Select(a => MapToDto(a, departments.GetValueOrDefault(a.DepartmentId))).ToList();
        return Result<List<ApprovalConfigResponseDto>>.Success(dtos, new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        });
    }

    public async Task<Result<ApprovalConfigResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.ApprovalConfigs.Where(a => a.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<ApprovalConfigResponseDto>.Failed("Approval config not found.");
        var deptName = await db.Departments.Where(d => d.Id == entity.DepartmentId).Select(d => d.Name).FirstOrDefaultAsync(cancellationToken);
        return Result<ApprovalConfigResponseDto>.Success(MapToDto(entity, deptName));
    }

    public async Task<Result<ApprovalConfigResponseDto>> GetByDepartmentIdAsync(string departmentId, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.ApprovalConfigs.Where(a => a.DepartmentId == departmentId);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<ApprovalConfigResponseDto>.Failed("Approval config not found for this department.");
        var deptName = await db.Departments.Where(d => d.Id == entity.DepartmentId).Select(d => d.Name).FirstOrDefaultAsync(cancellationToken);
        return Result<ApprovalConfigResponseDto>.Success(MapToDto(entity, deptName));
    }

    public async Task<Result<ApprovalConfigResponseDto>> CreateAsync(CreateApprovalConfigDto dto, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var userId = userProfileService.GetUserId();
        var user = await userManager.FindByIdAsync(userId);
        var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;
        var dept = await db.Departments.FirstOrDefaultAsync(d => d.Id == dto.DepartmentId && d.TenantId == tenantId, cancellationToken);
        if (dept == null) return Result<ApprovalConfigResponseDto>.Failed("Department not found.");
        var exists = await db.ApprovalConfigs.AnyAsync(a => a.DepartmentId == dto.DepartmentId && a.TenantId == tenantId, cancellationToken);
        if (exists) return Result<ApprovalConfigResponseDto>.Failed("Approval config for this department already exists.");

        var stepsJson = JsonSerializer.Serialize(dto.Steps.Select(s => new { s.StepOrder, s.MinAmount, s.MaxAmount, s.ApproverRoleId, s.IsMandatory }).ToList(), JsonOptions);
        var entity = new ApprovalConfig
        {
            Id = Guid.NewGuid().ToString(),
            DepartmentId = dto.DepartmentId,
            StepsJson = stepsJson,
            TenantId = tenantId,
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        };
        await db.ApprovalConfigs.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return Result<ApprovalConfigResponseDto>.Success(MapToDto(entity, dept.Name));
    }

    public async Task<Result<ApprovalConfigResponseDto>> UpdateAsync(string id, UpdateApprovalConfigDto dto, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.ApprovalConfigs.Where(a => a.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<ApprovalConfigResponseDto>.Failed("Approval config not found.");
        if (dto.DepartmentId != null) entity.DepartmentId = dto.DepartmentId;
        if (dto.Steps != null)
            entity.StepsJson = JsonSerializer.Serialize(dto.Steps.Select(s => new { s.StepOrder, s.MinAmount, s.MaxAmount, s.ApproverRoleId, s.IsMandatory }).ToList(), JsonOptions);
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.ApprovalConfigs.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        var deptName = await db.Departments.Where(d => d.Id == entity.DepartmentId).Select(d => d.Name).FirstOrDefaultAsync(cancellationToken);
        return Result<ApprovalConfigResponseDto>.Success(MapToDto(entity, deptName));
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.ApprovalConfigs.Where(a => a.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<bool>.Failed("Approval config not found.");
        entity.IsDeleted = true;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.ApprovalConfigs.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private static ApprovalConfigResponseDto MapToDto(ApprovalConfig a, string departmentName)
    {
        var steps = new List<ApprovalConfigStepDto>();
        if (!string.IsNullOrEmpty(a.StepsJson))
        {
            try
            {
                var raw = JsonSerializer.Deserialize<List<JsonElement>>(a.StepsJson);
                if (raw != null)
                    foreach (var el in raw)
                    {
                        steps.Add(new ApprovalConfigStepDto
                        {
                            StepOrder = el.TryGetProperty("stepOrder", out var so) ? so.GetInt32() : 0,
                            MinAmount = el.TryGetProperty("minAmount", out var min) ? min.GetDecimal() : 0,
                            MaxAmount = el.TryGetProperty("maxAmount", out var max) ? max.GetDecimal() : 0,
                            ApproverRoleId = el.TryGetProperty("approverRoleId", out var rid) ? rid.GetString() ?? "" : "",
                            ApproverRoleName = el.TryGetProperty("approverRoleName", out var rn) ? rn.GetString() : null,
                            IsMandatory = el.TryGetProperty("isMandatory", out var im) && im.GetBoolean()
                        });
                    }
            }
            catch { /* ignore */ }
        }
        return new ApprovalConfigResponseDto
        {
            Id = a.Id,
            TenantId = a.TenantId ?? string.Empty,
            DepartmentId = a.DepartmentId,
            DepartmentName = departmentName,
            Steps = steps.OrderBy(s => s.StepOrder).ToList(),
            CreatedOn = a.CreatedOn
        };
    }
}
