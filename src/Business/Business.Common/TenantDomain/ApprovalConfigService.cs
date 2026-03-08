using System.Globalization;
using System.Text;
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

        var roleIds = dto.Steps.Select(s => s.ApproverRoleId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
        var roleNames = roleIds.Count > 0
            ? await db.Roles.Where(r => roleIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id, r => r.Name ?? r.NormalizedName ?? "", cancellationToken)
            : new Dictionary<string, string>();
        var stepsWithNames = dto.Steps.Select(s => new { s.StepOrder, s.MinAmount, s.MaxAmount, s.ApproverRoleId, ApproverRoleName = roleNames.GetValueOrDefault(s.ApproverRoleId, ""), s.IsMandatory }).ToList();
        var stepsJson = JsonSerializer.Serialize(stepsWithNames, JsonOptions);
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
        {
            var roleIds = dto.Steps.Select(s => s.ApproverRoleId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
            var roleNames = roleIds.Count > 0
                ? await db.Roles.Where(r => roleIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id, r => r.Name ?? r.NormalizedName ?? "", cancellationToken)
                : new Dictionary<string, string>();
            var stepsWithNames = dto.Steps.Select(s => new { s.StepOrder, s.MinAmount, s.MaxAmount, s.ApproverRoleId, ApproverRoleName = roleNames.GetValueOrDefault(s.ApproverRoleId, ""), s.IsMandatory }).ToList();
            entity.StepsJson = JsonSerializer.Serialize(stepsWithNames, JsonOptions);
        }
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

    public async Task<Result<ApprovalConfigImportResultDto>> ImportFromCsvAsync(Stream csvStream, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var userId = userProfileService.GetUserId();
        var user = await userManager.FindByIdAsync(userId);
        var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;
        if (string.IsNullOrEmpty(tenantId)) return Result<ApprovalConfigImportResultDto>.Failed("Tenant context required.");

        var result = new ApprovalConfigImportResultDto();
        using var reader = new StreamReader(csvStream, Encoding.UTF8);
        var header = await reader.ReadLineAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(header)) return Result<ApprovalConfigImportResultDto>.Failed("CSV file is empty.");
        var lines = new List<string>();
        while (await reader.ReadLineAsync(cancellationToken) is { } line)
            lines.Add(line);

        var roleIds = await db.Roles.ToDictionaryAsync(r => r.Id, r => r.Name ?? r.NormalizedName ?? "", cancellationToken);
        var departments = await db.Departments.Where(d => d.TenantId == tenantId).ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        var rowsByDept = new Dictionary<string, List<(int StepOrder, decimal MinAmount, decimal MaxAmount, string ApproverRoleId, bool IsMandatory)>>(StringComparer.OrdinalIgnoreCase);
        var rowIndex = 0;
        foreach (var line in lines)
        {
            rowIndex++;
            var parts = ParseCsvLine(line);
            if (parts.Count < 5) { result.Errors.Add($"Row {rowIndex + 1}: expected at least 5 columns (DepartmentId,StepOrder,MinAmount,MaxAmount,ApproverRoleId[,IsMandatory])."); continue; }
            var deptId = parts[0].Trim();
            if (string.IsNullOrEmpty(deptId)) { result.Errors.Add($"Row {rowIndex + 1}: DepartmentId is required."); continue; }
            if (!departments.ContainsKey(deptId)) { result.Errors.Add($"Row {rowIndex + 1}: Department '{deptId}' not found."); continue; }
            if (!int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var stepOrder)) { result.Errors.Add($"Row {rowIndex + 1}: StepOrder must be a number."); continue; }
            if (!decimal.TryParse(parts[2].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var minAmount)) { result.Errors.Add($"Row {rowIndex + 1}: MinAmount must be a number."); continue; }
            if (!decimal.TryParse(parts[3].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var maxAmount)) { result.Errors.Add($"Row {rowIndex + 1}: MaxAmount must be a number."); continue; }
            var approverRoleId = parts[4].Trim();
            if (string.IsNullOrEmpty(approverRoleId)) { result.Errors.Add($"Row {rowIndex + 1}: ApproverRoleId is required."); continue; }
            if (!roleIds.ContainsKey(approverRoleId)) { result.Errors.Add($"Row {rowIndex + 1}: Role '{approverRoleId}' not found."); continue; }
            var isMandatory = parts.Count > 5 && bool.TryParse(parts[5].Trim(), out var im) && im;

            if (!rowsByDept.TryGetValue(deptId, out var list))
            {
                list = new List<(int, decimal, decimal, string, bool)>();
                rowsByDept[deptId] = list;
            }
            list.Add((stepOrder, minAmount, maxAmount, approverRoleId, isMandatory));
        }

        foreach (var (deptId, steps) in rowsByDept.OrderBy(x => x.Key))
        {
            if (steps.Count == 0) continue;
            var stepsJson = JsonSerializer.Serialize(steps.Select(s => new { stepOrder = s.StepOrder, minAmount = s.MinAmount, maxAmount = s.MaxAmount, approverRoleId = s.ApproverRoleId, approverRoleName = roleIds.GetValueOrDefault(s.ApproverRoleId), isMandatory = s.IsMandatory }).ToList(), JsonOptions);
            var existing = await db.ApprovalConfigs.FirstOrDefaultAsync(a => a.DepartmentId == deptId && a.TenantId == tenantId && !a.IsDeleted, cancellationToken);
            if (existing != null)
            {
                existing.StepsJson = stepsJson;
                existing.LastModifiedBy = userId;
                existing.LastModifiedOn = DateTime.UtcNow;
                db.ApprovalConfigs.Update(existing);
                result.UpdatedCount++;
            }
            else
            {
                var entity = new ApprovalConfig
                {
                    Id = Guid.NewGuid().ToString(),
                    DepartmentId = deptId,
                    StepsJson = stepsJson,
                    TenantId = tenantId,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow
                };
                await db.ApprovalConfigs.AddAsync(entity, cancellationToken);
                result.CreatedCount++;
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result<ApprovalConfigImportResultDto>.Success(result);
    }

    private static List<string> ParseCsvLine(string line)
    {
        var list = new List<string>();
        var sb = new StringBuilder();
        var inQuotes = false;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"') { inQuotes = !inQuotes; continue; }
            if (!inQuotes && (c == ',' || c == ';')) { list.Add(sb.ToString()); sb.Clear(); continue; }
            sb.Append(c);
        }
        list.Add(sb.ToString());
        return list;
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
