using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Identity;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Constant;
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class LeadService : ILeadService
{
    private readonly ApplicationDataContext _db;
    private readonly ISieveExtension _sieveExtension;
    private readonly ILogger<LeadService> _logger;
    private readonly IUserProfileService _userProfileService;
    private readonly UserManager<ApplicationUser> _userManager;

    public LeadService(
        ApplicationDataContext db,
        ISieveExtension sieveExtension,
        ILogger<LeadService> logger,
        IUserProfileService userProfileService,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _sieveExtension = sieveExtension;
        _logger = logger;
        _userProfileService = userProfileService;
        _userManager = userManager;
    }

    public async Task<Result<Lead>> CreateLeadAsync(CreateLeadPublicDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var contact = new Contact
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone ?? string.Empty
            };

            await _db.Contacts.AddAsync(contact);
            await _db.SaveChangesAsync(); // Save to get contact ID

            var prospect = new Prospect
            {
                PrimaryContactId = contact.Id
            };

            await _db.Prospects.AddAsync(prospect);
            await _db.SaveChangesAsync(); // Save to get prospect ID

            var lead = new Lead
            {
                ProspectId = prospect.Id,
                Status = LeadStatus.New,
                Source = "Web"
            };

            await _db.Leads.AddAsync(lead);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            // Reload lead with related entities for response
            await _db.Entry(lead).Reference(l => l.Prospect).LoadAsync();
            await _db.Entry(lead.Prospect).Reference(p => p.PrimaryContact).LoadAsync();

            return Result<Lead>.Success(lead);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating lead for contact {Email}: {Message}", dto.Email, ex.Message);
            throw;
        }
    }

    public async Task<Result<LeadActivity>> AddActivityAsync(string leadId, LeadActivityDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var lead = await _db.Set<Lead>().FirstOrDefaultAsync(l => l.Id == leadId);
            if (lead == null)
                return Result<LeadActivity>.Failed("Lead not found.");

            var act = new LeadActivity
            {
                LeadId = leadId,
                Kind = dto.Kind,
                Notes = dto.Notes,
                When = DateTimeOffset.UtcNow
            };

            await _db.LeadActivities.AddAsync(act);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<LeadActivity>.Success(act);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error adding activity to lead {LeadId}: {Message}", leadId, ex.Message);
            throw;
        }
    }

    public async Task<Result<List<Lead>>> ListAsync(CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null)
    {
        var query = _db.Set<Lead>()
            .Include(l => l.Prospect)
            .ThenInclude(p => p.PrimaryContact)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeadStatus>(status, true, out var statusEnum))
        {
            query = query.Where(l => l.Status == statusEnum);
        }

        if (from.HasValue)
        {
            query = query.Where(l => l.CreatedOn >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(l => l.CreatedOn <= to.Value);
        }

        var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
        var leads = await result.ToListAsync();

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };

        return Result<List<Lead>>.Success(leads, pagination);
    }

    public async Task<Result<Lead>> GetByIdAsync(string id)
    {
        var lead = await _db.Set<Lead>()
            .Include(l => l.Prospect)
            .ThenInclude(p => p.PrimaryContact)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lead == null)
            return Result<Lead>.Failed("Lead not found.");

        return Result<Lead>.Success(lead);
    }

    public async Task<Result<Lead>> UpdateStatusAsync(string id, string newStatus)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var lead = await _db.Set<Lead>().FirstOrDefaultAsync(l => l.Id == id);
            if (lead == null)
                return Result<Lead>.Failed("Lead not found.");

            if (!Enum.TryParse<LeadStatus>(newStatus, true, out var statusEnum))
            {
                return Result<Lead>.Failed($"Invalid status: {newStatus}");
            }

            // Simple state machine validation
            if (lead.Status == LeadStatus.Lost && statusEnum == LeadStatus.Contacted)
            {
                return Result<Lead>.Failed("Cannot move from Lost to Contacted directly.");
            }

            lead.Status = statusEnum;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            // Reload with related entities
            await _db.Entry(lead).Reference(l => l.Prospect).LoadAsync();
            await _db.Entry(lead.Prospect).Reference(p => p.PrimaryContact).LoadAsync();

            return Result<Lead>.Success(lead);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating status for lead {LeadId}: {Message}", id, ex.Message);
            throw;
        }
    }

    public async Task<Result<List<LeadActivity>>> GetActivitiesAsync(string leadId)
    {
        var lead = await _db.Set<Lead>().AsNoTracking().FirstOrDefaultAsync(l => l.Id == leadId);
        if (lead == null)
            return Result<List<LeadActivity>>.Failed("Lead not found.");

        var activities = await _db.Set<LeadActivity>()
            .Where(a => a.LeadId == leadId)
            .OrderByDescending(a => a.When)
            .ToListAsync();

        return Result<List<LeadActivity>>.Success(activities);
    }

    public async Task<Result<List<Lead>>> GetLeadsForAdminAsync(CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<List<Lead>>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<List<Lead>>.Failed("User not found or inactive.");

            var userRoles = await _db.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Join(_db.Roles.Where(r => !r.IsDeleted),
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .ToListAsync();

            var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);
            var isAdmin = userRoles.Contains(SystemRoles.Admin);

            if (!isSuperAdmin && !isAdmin)
                return Result<List<Lead>>.Failed("Access denied. Admin or SuperAdmin role required.");

            // Build query with includes
            IQueryable<Lead> query = _db.Set<Lead>()
                .Include(l => l.Prospect)
                .ThenInclude(p => p.PrimaryContact)
                .AsNoTracking();

            // For SuperAdmin, ignore the automatic tenant query filter to see all leads across all tenants
            if (isSuperAdmin)
            {
                query = query.IgnoreQueryFilters();
            }
            // For Tenant Admin, the automatic tenant query filter will already filter by their tenant
            // But we can add an explicit filter for clarity and to ensure it works correctly
            else if (!string.IsNullOrEmpty(user.TenantId))
            {
                // Tenant Admin: only show leads from their tenant
                // The query filter should handle this, but we add explicit filter for safety
                query = query.Where(l => l.TenantId == user.TenantId);
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeadStatus>(status, true, out var statusEnum))
            {
                query = query.Where(l => l.Status == statusEnum);
            }

            // Apply date range filters
            if (from.HasValue)
            {
                query = query.Where(l => l.CreatedOn >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(l => l.CreatedOn <= to.Value);
            }

            // Apply Sieve filtering and pagination
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var leads = await result.ToListAsync();

            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };

            return Result<List<Lead>>.Success(leads, pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads for admin: {Message}", ex.Message);
            return Result<List<Lead>>.Failed($"An error occurred while retrieving leads: {ex.Message}");
        }
    }

    public async Task<Result<Lead>> GetLeadDetailsForAdminAsync(string id)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<Lead>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<Lead>.Failed("User not found or inactive.");

            var userRoles = await _db.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Join(_db.Roles.Where(r => !r.IsDeleted),
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .ToListAsync();

            var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);
            var isAdmin = userRoles.Contains(SystemRoles.Admin);

            if (!isSuperAdmin && !isAdmin)
                return Result<Lead>.Failed("Access denied. Admin or SuperAdmin role required.");

            // Build query with includes
            IQueryable<Lead> query = _db.Set<Lead>()
                .Include(l => l.Prospect)
                .ThenInclude(p => p.PrimaryContact)
                .AsNoTracking();

            // For SuperAdmin, ignore the automatic tenant query filter
            if (isSuperAdmin)
            {
                query = query.IgnoreQueryFilters();
            }

            var lead = await query.FirstOrDefaultAsync(l => l.Id == id);

            if (lead == null)
                return Result<Lead>.Failed("Lead not found.");

            // For Tenant Admin, verify the lead belongs to their tenant
            if (!isSuperAdmin && !string.IsNullOrEmpty(user.TenantId) && lead.TenantId != user.TenantId)
            {
                return Result<Lead>.Failed("Access denied. Lead does not belong to your tenant.");
            }

            return Result<Lead>.Success(lead);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lead details for admin: {Message}", ex.Message);
            return Result<Lead>.Failed($"An error occurred while retrieving lead details: {ex.Message}");
        }
    }

    public async Task<Result<List<Lead>>> GetLeadsByTenantIdAsync(string tenantId, CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<List<Lead>>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<List<Lead>>.Failed("User not found or inactive.");

            var userRoles = await _db.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Join(_db.Roles.Where(r => !r.IsDeleted),
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .ToListAsync();

            var isSuperAdmin = userRoles.Contains(SystemRoles.SuperAdmin);

            // Only SuperAdmin can filter by tenantId
            if (!isSuperAdmin)
                return Result<List<Lead>>.Failed("Access denied. SuperAdmin role required to filter by tenantId.");

            // Verify tenant exists
            var tenant = await _db.Set<Data.Entities.Tenant.Tenant>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive);

            if (tenant == null)
                return Result<List<Lead>>.Failed("Tenant not found or inactive.");

            // Build query with includes
            IQueryable<Lead> query = _db.Set<Lead>()
                .Include(l => l.Prospect)
                .ThenInclude(p => p.PrimaryContact)
                .AsNoTracking()
                .IgnoreQueryFilters() // Ignore automatic tenant filter for SuperAdmin
                .Where(l => l.TenantId == tenantId);

            // Apply status filter
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeadStatus>(status, true, out var statusEnum))
            {
                query = query.Where(l => l.Status == statusEnum);
            }

            // Apply date range filters
            if (from.HasValue)
            {
                query = query.Where(l => l.CreatedOn >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(l => l.CreatedOn <= to.Value);
            }

            // Apply Sieve filtering and pagination
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var leads = await result.ToListAsync();

            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };

            return Result<List<Lead>>.Success(leads, pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads by tenantId: {Message}", ex.Message);
            return Result<List<Lead>>.Failed($"An error occurred while retrieving leads: {ex.Message}");
        }
    }
}
