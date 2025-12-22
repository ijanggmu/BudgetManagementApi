using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.AdminPortalApi.ExcelExport;
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
    private readonly IExcelExportService _excelExportService;

    public LeadService(
        ApplicationDataContext db,
        ISieveExtension sieveExtension,
        ILogger<LeadService> logger,
        IUserProfileService userProfileService,
        UserManager<ApplicationUser> userManager,
        IExcelExportService excelExportService)
    {
        _db = db;
        _sieveExtension = sieveExtension;
        _logger = logger;
        _userProfileService = userProfileService;
        _userManager = userManager;
        _excelExportService = excelExportService;
    }

    private static LeadResponseDto MapToDto(Lead lead)
    {
        return new LeadResponseDto(
            lead.Id,
            lead.ProspectId,
            lead.Status.ToString(),
            lead.Source,
            lead.OwnerUserId,
            lead.CreatedOn,
            lead.Prospect != null ? new ProspectResponseDto(
                lead.Prospect.Id,
                lead.Prospect.PrimaryContactId,
                lead.Prospect.PrimaryContact != null ? new ContactResponseDto(
                    lead.Prospect.PrimaryContact.Id,
                    lead.Prospect.PrimaryContact.FullName,
                    lead.Prospect.PrimaryContact.Email,
                    lead.Prospect.PrimaryContact.Phone
                ) : null
            ) : null
        );
    }

    private static LeadActivityResponseDto MapActivityToDto(LeadActivity activity)
    {
        return new LeadActivityResponseDto(
            activity.Id,
            activity.LeadId,
            activity.Kind,
            activity.Notes,
            activity.When
        );
    }

    public async Task<Result<LeadResponseDto>> CreateLeadAsync(CreateLeadPublicDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var contact = new Contact
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone ?? string.Empty
            };

            await _db.Contacts.AddAsync(contact, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken); // Save to get contact ID

            var prospect = new Prospect
            {
                PrimaryContactId = contact.Id
            };

            await _db.Prospects.AddAsync(prospect, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken); // Save to get prospect ID

            var lead = new Lead
            {
                ProspectId = prospect.Id,
                Status = LeadStatus.New,
                Source = "Web"
            };

            await _db.Leads.AddAsync(lead, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Reload lead with related entities for response
            await _db.Entry(lead).Reference(l => l.Prospect).LoadAsync();
            await _db.Entry(lead.Prospect).Reference(p => p.PrimaryContact).LoadAsync();

            return Result<LeadResponseDto>.Success(MapToDto(lead));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error creating lead for contact {Email}: {Message}", dto.Email, ex.Message);
            throw;
        }
    }

    public async Task<Result<LeadActivityResponseDto>> AddActivityAsync(string leadId, LeadActivityDto dto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var lead = await _db.Set<Lead>().FirstOrDefaultAsync(l => l.Id == leadId, cancellationToken);
            if (lead == null)
                return Result<LeadActivityResponseDto>.Failed("Lead not found.");

            var act = new LeadActivity
            {
                LeadId = leadId,
                Kind = dto.Kind,
                Notes = dto.Notes,
                When = DateTimeOffset.UtcNow
            };

            await _db.LeadActivities.AddAsync(act, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<LeadActivityResponseDto>.Success(MapActivityToDto(act));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error adding activity to lead {LeadId}: {Message}", leadId, ex.Message);
            throw;
        }
    }

    public async Task<Result<List<LeadResponseDto>>> ListAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Lead>()
            .Include(l => l.Prospect)
            .ThenInclude(p => p.PrimaryContact)
            .AsNoTracking()
            .AsQueryable();

        var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
        var leads = await result.ToListAsync(cancellationToken);

        var leadDtos = leads.Select(MapToDto).ToList();

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };

        return Result<List<LeadResponseDto>>.Success(leadDtos, pagination);
    }

    public async Task<Result<LeadResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var lead = await _db.Set<Lead>()
            .Include(l => l.Prospect)
            .ThenInclude(p => p.PrimaryContact)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lead == null)
            return Result<LeadResponseDto>.Failed("Lead not found.");

        return Result<LeadResponseDto>.Success(MapToDto(lead));
    }

    public async Task<Result<LeadResponseDto>> UpdateStatusAsync(string id, string newStatus, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var lead = await _db.Set<Lead>().FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
            if (lead == null)
                return Result<LeadResponseDto>.Failed("Lead not found.");

            if (!Enum.TryParse<LeadStatus>(newStatus, true, out var statusEnum))
            {
                return Result<LeadResponseDto>.Failed($"Invalid status: {newStatus}");
            }

            // Simple state machine validation
            if (lead.Status == LeadStatus.Lost && statusEnum == LeadStatus.Contacted)
            {
                return Result<LeadResponseDto>.Failed("Cannot move from Lost to Contacted directly.");
            }

            lead.Status = statusEnum;
            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync();

            // Reload with related entities
            await _db.Entry(lead).Reference(l => l.Prospect).LoadAsync();
            await _db.Entry(lead.Prospect).Reference(p => p.PrimaryContact).LoadAsync();

            return Result<LeadResponseDto>.Success(MapToDto(lead));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating status for lead {LeadId}: {Message}", id, ex.Message);
            throw;
        }
    }

    public async Task<Result<List<LeadActivityResponseDto>>> GetActivitiesAsync(string leadId, CancellationToken cancellationToken = default)
    {
        var lead = await _db.Set<Lead>().AsNoTracking().FirstOrDefaultAsync(l => l.Id == leadId, cancellationToken);
        if (lead == null)
            return Result<List<LeadActivityResponseDto>>.Failed("Lead not found.");

        var activities = await _db.Set<LeadActivity>()
            .Where(a => a.LeadId == leadId)
            .OrderByDescending(a => a.When)
            .ToListAsync();

        var activityDtos = activities.Select(MapActivityToDto).ToList();

        return Result<List<LeadActivityResponseDto>>.Success(activityDtos);
    }

    public async Task<Result<List<LeadResponseDto>>> GetLeadsForAdminAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<List<LeadResponseDto>>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<List<LeadResponseDto>>.Failed("User not found or inactive.");

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
                return Result<List<LeadResponseDto>>.Failed("Access denied. Admin or SuperAdmin role required.");

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

            // Apply Sieve filtering and pagination
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var leads = await result.ToListAsync(cancellationToken);

            var leadDtos = leads.Select(MapToDto).ToList();

            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };

            return Result<List<LeadResponseDto>>.Success(leadDtos, pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads for admin: {Message}", ex.Message);
            return Result<List<LeadResponseDto>>.Failed($"An error occurred while retrieving leads: {ex.Message}");
        }
    }

    public async Task<Result<LeadResponseDto>> GetLeadDetailsForAdminAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<LeadResponseDto>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<LeadResponseDto>.Failed("User not found or inactive.");

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
                return Result<LeadResponseDto>.Failed("Access denied. Admin or SuperAdmin role required.");

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

            var lead = await query.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            if (lead == null)
                return Result<LeadResponseDto>.Failed("Lead not found.");

            // For Tenant Admin, verify the lead belongs to their tenant
            if (!isSuperAdmin && !string.IsNullOrEmpty(user.TenantId) && lead.TenantId != user.TenantId)
            {
                return Result<LeadResponseDto>.Failed("Access denied. Lead does not belong to your tenant.");
            }

            return Result<LeadResponseDto>.Success(MapToDto(lead));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lead details for admin: {Message}", ex.Message);
            return Result<LeadResponseDto>.Failed($"An error occurred while retrieving lead details: {ex.Message}");
        }
    }

    public async Task<Result<List<LeadResponseDto>>> GetLeadsByTenantIdAsync(string tenantId, CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<List<LeadResponseDto>>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<List<LeadResponseDto>>.Failed("User not found or inactive.");

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
                return Result<List<LeadResponseDto>>.Failed("Access denied. SuperAdmin role required to filter by tenantId.");

            // Verify tenant exists
            var tenant = await _db.Set<Data.Entities.Tenant.Tenant>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive);

            if (tenant == null)
                return Result<List<LeadResponseDto>>.Failed("Tenant not found or inactive.");

            // Build query with includes
            IQueryable<Lead> query = _db.Set<Lead>()
                .Include(l => l.Prospect)
                .ThenInclude(p => p.PrimaryContact)
                .AsNoTracking()
                .IgnoreQueryFilters() // Ignore automatic tenant filter for SuperAdmin
                .Where(l => l.TenantId == tenantId);

            // Apply Sieve filtering and pagination
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var leads = await result.ToListAsync(cancellationToken);

            var leadDtos = leads.Select(MapToDto).ToList();

            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };

            return Result<List<LeadResponseDto>>.Success(leadDtos, pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads by tenantId: {Message}", ex.Message);
            return Result<List<LeadResponseDto>>.Failed($"An error occurred while retrieving leads: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> ExportToExcelAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            requestModel.PageSize = -1;
            var result = await GetLeadsForAdminAsync(requestModel, cancellationToken);

            if (!result.IsSuccess || result.Data == null)
                return Result<byte[]>.Failed(result.Error ?? "Failed to retrieve lead data.");

            var columnMappings = new Dictionary<string, string>
            {
                { "Id", "ID" },
                { "ProspectId", "Prospect ID" },
                { "Status", "Status" },
                { "Source", "Source" },
                { "OwnerUserId", "Owner User ID" },
                { "CreatedOn", "Created On" }
            };

            // Flatten the data for export (include prospect and contact info)
            var exportData = result.Data.Select(lead => new
            {
                lead.Id,
                lead.ProspectId,
                lead.Status,
                lead.Source,
                OwnerUserId = lead.OwnerUserId?.ToString() ?? "",
                lead.CreatedOn,
                ProspectName = lead.Prospect?.PrimaryContact?.FullName ?? "",
                ProspectEmail = lead.Prospect?.PrimaryContact?.Email ?? "",
                ProspectPhone = lead.Prospect?.PrimaryContact?.Phone ?? ""
            }).ToList();

            var excelData = await _excelExportService.ExportToExcelAsync(exportData, "Leads", columnMappings);
            return Result<byte[]>.Success(excelData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting leads to Excel: {Message}", ex.Message);
            return Result<byte[]>.Failed($"An error occurred while exporting: {ex.Message}");
        }
    }
}
