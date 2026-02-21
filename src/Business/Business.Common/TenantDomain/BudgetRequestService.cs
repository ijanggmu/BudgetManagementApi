using System.Text.Json;
using Data.Context;
using Data.Entities.Identity;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.BudgetRequest;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class BudgetRequestService(
    ApplicationDataContext db,
    UserManager<ApplicationUser> userManager,
    IUserProfileService userProfileService,
    ISieveExtension sieveExtension,
    IMemoService memoService)
    : IBudgetRequestService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<Result<List<BudgetRequestResponseDto>>> GetAllAsync(BudgetRequestListRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        IQueryable<BudgetRequest> query = db.BudgetRequests.AsQueryable();
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            query = query.IgnoreQueryFilters().Where(b => b.TenantId == requestModel.TenantId);
        else if (isSuperAdmin)
            query = query.IgnoreQueryFilters();
        if (!string.IsNullOrEmpty(requestModel.DepartmentId))
            query = query.Where(b => b.DepartmentId == requestModel.DepartmentId);
        if (!string.IsNullOrEmpty(requestModel.Status))
            query = query.Where(b => b.Status.ToString() == requestModel.Status);
        if (!string.IsNullOrEmpty(requestModel.UserId))
            query = query.Where(b => b.UserId == requestModel.UserId);

        var (result, totalCount, totalPage) = await sieveExtension.ApplySieve(query, requestModel);
        var list = await result.ToListAsync(cancellationToken);
        var deptIds = list.Select(b => b.DepartmentId).Distinct().ToList();
        var userIds = list.Select(b => b.UserId).Distinct().ToList();
        var departments = await db.Departments.Where(d => deptIds.Contains(d.Id)).ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);
        var roles = await db.Roles.ToDictionaryAsync(r => r.Id, r => r.Name ?? r.NormalizedName ?? "", cancellationToken);
        var users = await db.Users.Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Email ?? "", cancellationToken);

        var dtos = list.Select(b => MapToDto(b, departments.GetValueOrDefault(b.DepartmentId), users.GetValueOrDefault(b.UserId), roles.GetValueOrDefault(b.NextApproverRoleId ?? ""))).ToList();
        return Result<List<BudgetRequestResponseDto>>.Success(dtos, new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        });
    }

    public async Task<Result<BudgetRequestResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.BudgetRequests.Where(b => b.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<BudgetRequestResponseDto>.Failed("Budget request not found.");
        var deptName = await db.Departments.Where(d => d.Id == entity.DepartmentId).Select(d => d.Name).FirstOrDefaultAsync(cancellationToken);
        var userName = await db.Users.Where(u => u.Id == entity.UserId).Select(u => u.UserName ?? u.Email).FirstOrDefaultAsync(cancellationToken);
        var nextRoleName = entity.NextApproverRoleId == null ? null : await db.Roles.Where(r => r.Id == entity.NextApproverRoleId).Select(r => r.Name ?? r.NormalizedName).FirstOrDefaultAsync(cancellationToken);
        return Result<BudgetRequestResponseDto>.Success(MapToDto(entity, deptName, userName ?? "", nextRoleName ?? ""));
    }

    public async Task<Result<BudgetRequestResponseDto>> CreateAsync(CreateBudgetRequestDto dto, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var userId = userProfileService.GetUserId();
        var user = await userManager.FindByIdAsync(userId);
        var tenantId = isSuperAdmin ? user?.TenantId : db.CurrentTenantId;
        var dept = await db.Departments.FirstOrDefaultAsync(d => d.Id == dto.DepartmentId && d.TenantId == tenantId, cancellationToken);
        if (dept == null) return Result<BudgetRequestResponseDto>.Failed("Department not found.");
        var config = await db.ApprovalConfigs.FirstOrDefaultAsync(c => c.DepartmentId == dto.DepartmentId && c.TenantId == tenantId, cancellationToken);
        if (config == null) return Result<BudgetRequestResponseDto>.Failed("No approval config for this department.");
        List<(int Order, string RoleId, string RoleName)> steps = ParseApprovalSteps(config.StepsJson);
        if (steps.Count == 0) return Result<BudgetRequestResponseDto>.Failed("Approval config has no steps.");
        var first = steps.OrderBy(s => s.Order).First();
        var entity = new BudgetRequest
        {
            Id = Guid.NewGuid().ToString(),
            DepartmentId = dto.DepartmentId,
            UserId = userId,
            Amount = dto.Amount,
            Purpose = dto.Purpose?.Trim() ?? "",
            Status = BudgetRequestStatus.PendingApproval,
            NextApproverRoleId = first.RoleId,
            CurrentApprovalStep = 1,
            RequestedDate = DateTime.UtcNow,
            TenantId = tenantId ?? "",
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        };
        await db.BudgetRequests.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        var userName = user?.UserName ?? user?.Email ?? "";
        var nextRoleName = await db.Roles.Where(r => r.Id == first.RoleId).Select(r => r.Name ?? r.NormalizedName).FirstOrDefaultAsync(cancellationToken);
        return Result<BudgetRequestResponseDto>.Success(MapToDto(entity, dept.Name, userName ?? "", nextRoleName ?? ""));
    }

    public async Task<Result<BudgetRequestResponseDto>> ApproveAsync(string id, ApproveBudgetRequestDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var request = await db.BudgetRequests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (request == null) return Result<BudgetRequestResponseDto>.Failed("Budget request not found.");
        if (request.Status != BudgetRequestStatus.PendingApproval)
            return Result<BudgetRequestResponseDto>.Failed("Request is not pending approval.");
        var userRoles = await db.Set<ApplicationUserRoles>().Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToListAsync(cancellationToken);
        if (!userRoles.Contains(request.NextApproverRoleId))
            return Result<BudgetRequestResponseDto>.Failed("You are not authorized to approve this request.");
        var approver = await userManager.FindByIdAsync(userId);
        var role = await db.Roles.FindAsync(request.NextApproverRoleId);
        var history = AppendApprovalHistory(request.ApprovalHistoryJson, request.NextApproverRoleId, role?.Name ?? role?.NormalizedName ?? "", userId, approver?.UserName ?? approver?.Email ?? "", dto.SignatureUrl);
        request.ApprovalHistoryJson = history;
        var config = await db.ApprovalConfigs.FirstOrDefaultAsync(c => c.DepartmentId == request.DepartmentId && c.TenantId == request.TenantId, cancellationToken);
        var steps = config != null ? ParseApprovalSteps(config.StepsJson).OrderBy(s => s.Order).ToList() : new List<(int Order, string RoleId, string RoleName)>();
        var currentIndex = steps.FindIndex(s => s.RoleId == request.NextApproverRoleId);
        if (currentIndex >= 0 && currentIndex < steps.Count - 1)
        {
            var next = steps[currentIndex + 1];
            request.NextApproverRoleId = next.RoleId;
            request.CurrentApprovalStep++;
        }
        else
        {
            request.NextApproverRoleId = null;
            request.Status = BudgetRequestStatus.Approved;
            request.ApprovedDate = DateTime.UtcNow;
        }
        request.LastModifiedBy = userId;
        request.LastModifiedOn = DateTime.UtcNow;
        db.BudgetRequests.Update(request);
        await db.SaveChangesAsync(cancellationToken);
        if (request.Status == BudgetRequestStatus.Approved)
        {
            var year = request.RequestedDate.Year;
            var quarter = (request.RequestedDate.Month - 1) / 3 + 1;
            var budget = await db.Budgets.FirstOrDefaultAsync(b => b.DepartmentId == request.DepartmentId && b.TenantId == request.TenantId && b.Year == year && b.Quarter == quarter, cancellationToken);
            if (budget != null)
            {
                budget.AllocatedAmount += request.Amount;
                budget.RemainingAmount = budget.TotalAmount - budget.AllocatedAmount;
                db.Budgets.Update(budget);
                await db.SaveChangesAsync(cancellationToken);
            }
            var memoResult = await memoService.CreateAsync(new Models.BeemaEdgeApi.Memo.CreateMemoDto { BudgetRequestId = request.Id }, cancellationToken);
            if (memoResult.IsSuccess && memoResult.Data != null)
            {
                request.MemoFileUrl = memoResult.Data.FileUrl;
                db.BudgetRequests.Update(request);
                await db.SaveChangesAsync(cancellationToken);
            }
        }
        var deptName = await db.Departments.Where(d => d.Id == request.DepartmentId).Select(d => d.Name).FirstOrDefaultAsync(cancellationToken);
        var userName = await db.Users.Where(u => u.Id == request.UserId).Select(u => u.UserName ?? u.Email).FirstOrDefaultAsync(cancellationToken);
        var nextRoleName = request.NextApproverRoleId == null ? null : await db.Roles.Where(r => r.Id == request.NextApproverRoleId).Select(r => r.Name ?? r.NormalizedName).FirstOrDefaultAsync(cancellationToken);
        return Result<BudgetRequestResponseDto>.Success(MapToDto(request, deptName ?? "", userName ?? "", nextRoleName ?? ""));
    }

    public async Task<Result<BudgetRequestResponseDto>> RejectAsync(string id, RejectBudgetRequestDto dto, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        var roleId = userProfileService.GetRoleId();
        var request = await db.BudgetRequests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (request == null) return Result<BudgetRequestResponseDto>.Failed("Budget request not found.");
        if (request.Status != BudgetRequestStatus.PendingApproval)
            return Result<BudgetRequestResponseDto>.Failed("Request is not pending approval.");
        var userRoles = await db.Set<ApplicationUserRoles>().Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToListAsync(cancellationToken);
        if (!userRoles.Contains(request.NextApproverRoleId))
            return Result<BudgetRequestResponseDto>.Failed("You are not authorized to reject this request.");
        request.Status = BudgetRequestStatus.Rejected;
        request.RejectedDate = DateTime.UtcNow;
        request.NextApproverRoleId = null;
        request.LastModifiedBy = userId;
        request.LastModifiedOn = DateTime.UtcNow;
        db.BudgetRequests.Update(request);
        await db.SaveChangesAsync(cancellationToken);
        var deptName = await db.Departments.Where(d => d.Id == request.DepartmentId).Select(d => d.Name).FirstOrDefaultAsync(cancellationToken);
        var userName = await db.Users.Where(u => u.Id == request.UserId).Select(u => u.UserName ?? u.Email).FirstOrDefaultAsync(cancellationToken);
        return Result<BudgetRequestResponseDto>.Success(MapToDto(request, deptName ?? "", userName ?? "", null));
    }

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.BudgetRequests.Where(b => b.Id == id);
        if (isSuperAdmin) query = query.IgnoreQueryFilters();
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        if (entity == null) return Result<bool>.Failed("Budget request not found.");
        if (entity.Status == BudgetRequestStatus.Approved)
            return Result<bool>.Failed("Cannot delete an approved request.");
        entity.IsDeleted = true;
        entity.LastModifiedBy = userProfileService.GetUserId();
        entity.LastModifiedOn = DateTime.UtcNow;
        db.BudgetRequests.Update(entity);
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private static List<(int Order, string RoleId, string RoleName)> ParseApprovalSteps(string stepsJson)
    {
        var list = new List<(int, string, string)>();
        if (string.IsNullOrEmpty(stepsJson)) return list;
        try
        {
            var raw = JsonSerializer.Deserialize<List<JsonElement>>(stepsJson);
            if (raw == null) return list;
            foreach (var el in raw)
            {
                var order = el.TryGetProperty("stepOrder", out var so) ? so.GetInt32() : 0;
                var roleId = el.TryGetProperty("approverRoleId", out var r) ? r.GetString() ?? "" : "";
                var roleName = el.TryGetProperty("approverRoleName", out var rn) ? rn.GetString() ?? "" : "";
                list.Add((order, roleId, roleName));
            }
        }
        catch { }
        return list;
    }

    private static string AppendApprovalHistory(string currentJson, string roleId, string roleName, string userId, string userName, string signatureUrl)
    {
        var list = new List<object>();
        if (!string.IsNullOrEmpty(currentJson))
        {
            try
            {
                var raw = JsonSerializer.Deserialize<List<JsonElement>>(currentJson);
                if (raw != null)
                    foreach (var el in raw)
                        list.Add(el);
            }
            catch { }
        }
        list.Add(new { roleId, roleName, userId, userName, approvedAt = DateTime.UtcNow, signatureUrl });
        return JsonSerializer.Serialize(list, JsonOptions);
    }

    private static BudgetRequestResponseDto MapToDto(BudgetRequest b, string departmentName, string userName, string? nextApproverRoleName)
    {
        return new BudgetRequestResponseDto
        {
            Id = b.Id,
            TenantId = b.TenantId ?? "",
            DepartmentId = b.DepartmentId,
            DepartmentName = departmentName,
            UserId = b.UserId,
            UserName = userName,
            Amount = b.Amount,
            Purpose = b.Purpose,
            Status = b.Status.ToString(),
            NextApproverRoleId = b.NextApproverRoleId,
            NextApproverRoleName = nextApproverRoleName ?? "",
            RequestedDate = b.RequestedDate,
            ApprovedDate = b.ApprovedDate,
            RejectedDate = b.RejectedDate,
            MemoFileUrl = b.MemoFileUrl,
            CreatedOn = b.CreatedOn
        };
    }

    public async Task<Result<byte[]>> ExportAsync(BudgetRequestListRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        IQueryable<BudgetRequest> query = db.BudgetRequests.AsQueryable();
        if (isSuperAdmin && !string.IsNullOrEmpty(requestModel.TenantId))
            query = query.IgnoreQueryFilters().Where(b => b.TenantId == requestModel.TenantId);
        else if (isSuperAdmin)
            query = query.IgnoreQueryFilters();
        if (!string.IsNullOrEmpty(requestModel.DepartmentId))
            query = query.Where(b => b.DepartmentId == requestModel.DepartmentId);
        if (!string.IsNullOrEmpty(requestModel.Status))
            query = query.Where(b => b.Status.ToString() == requestModel.Status);
        if (!string.IsNullOrEmpty(requestModel.UserId))
            query = query.Where(b => b.UserId == requestModel.UserId);
        var list = await query.Take(10000).ToListAsync(cancellationToken);
        var deptIds = list.Select(b => b.DepartmentId).Distinct().ToList();
        var userIds = list.Select(b => b.UserId).Distinct().ToList();
        var departments = await db.Departments.Where(d => deptIds.Contains(d.Id)).ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);
        var users = await db.Users.Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.UserName ?? u.Email ?? "", cancellationToken);
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        writer.WriteLine("Id,DepartmentId,DepartmentName,UserId,UserName,Amount,Purpose,Status,RequestedDate,ApprovedDate,RejectedDate,CreatedOn");
        foreach (var b in list)
        {
            var deptName = departments.GetValueOrDefault(b.DepartmentId) ?? "";
            var userName = users.GetValueOrDefault(b.UserId) ?? "";
            writer.WriteLine($"{b.Id},{b.DepartmentId},{Escape(deptName)},{b.UserId},{Escape(userName)},{b.Amount},{Escape(b.Purpose)},{b.Status},{b.RequestedDate:O},{b.ApprovedDate:O},{b.RejectedDate:O},{b.CreatedOn:O}");
        }
        writer.Flush();
        stream.Position = 0;
        return Result<byte[]>.Success(stream.ToArray());
    }

    private static string Escape(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        if (s.Contains(',') || s.Contains('"') || s.Contains('\n')) return "\"" + s.Replace("\"", "\"\"") + "\"";
        return s;
    }
}
