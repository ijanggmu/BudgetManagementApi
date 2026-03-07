using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.BudgetMemoAudit;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class BudgetMemoAuditService(
    ApplicationDataContext db,
    IUserProfileService userProfileService)
    : IBudgetMemoAuditService
{
    public async Task LogAsync(string entityType, string entityId, string action, string? details = null, CancellationToken cancellationToken = default)
    {
        var userId = userProfileService.GetUserId();
        var tenantId = db.CurrentTenantId;
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var userName = userId; // Could resolve from UserManager if needed
        var log = new BudgetMemoAuditLog
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = tenantId ?? string.Empty,
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            Details = details,
            UserName = userName,
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        };
        await db.BudgetMemoAuditLogs.AddAsync(log, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<List<BudgetMemoAuditItemDto>>> GetListAsync(BudgetMemoAuditListRequestModel request, CancellationToken cancellationToken = default)
    {
        var roleId = userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
        var query = db.BudgetMemoAuditLogs.AsNoTracking().Where(x => !x.IsDeleted);
        if (isSuperAdmin && !string.IsNullOrEmpty(request.TenantId))
            query = query.IgnoreQueryFilters().Where(x => x.TenantId == request.TenantId);
        else if (!isSuperAdmin)
            query = query.Where(x => x.TenantId == db.CurrentTenantId);
        else if (isSuperAdmin)
            query = query.IgnoreQueryFilters();
        if (!string.IsNullOrEmpty(request.EntityType))
            query = query.Where(x => x.EntityType == request.EntityType);
        if (!string.IsNullOrEmpty(request.EntityId))
            query = query.Where(x => x.EntityId == request.EntityId);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        var list = await query
            .OrderByDescending(x => x.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new BudgetMemoAuditItemDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                Action = x.Action,
                Details = x.Details,
                UserId = x.CreatedBy,
                UserName = x.UserName,
                CreatedOn = x.CreatedOn
            })
            .ToListAsync(cancellationToken);

        return Result<List<BudgetMemoAuditItemDto>>.Success(list, new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            PageSize = request.PageSize,
            CurrentPage = request.PageNumber
        });
    }
}
