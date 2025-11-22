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
using SharedKernel.Constant.Roles;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class QuotationService : IQuotationService
{
    private readonly ApplicationDataContext _db;
    private readonly IQuotationNumberGenerator _numbers;
    private readonly ISieveExtension _sieveExtension;
    private readonly IUserProfileService _userProfileService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<QuotationService> _logger;

    public QuotationService(
        ApplicationDataContext db, 
        IQuotationNumberGenerator numbers, 
        ISieveExtension sieveExtension,
        IUserProfileService userProfileService,
        UserManager<ApplicationUser> userManager,
        ILogger<QuotationService> logger)
    {
        _db = db;
        _numbers = numbers;
        _sieveExtension = sieveExtension;
        _userProfileService = userProfileService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<Quotation>> CreateAsync(CreateQuotationDto dto)
    {
        var quote = new Quotation
        {
            Number = await _numbers.NextAsync(),
            ProductId = dto.ProductId.ToString(),
            ProspectId = dto.ProspectId.ToString(),
        };

        foreach (var item in dto.Items)
        {
            quote.Items.Add(new QuotationItem
            {
                CoverageId = item.CoverageId.ToString(),
                SumInsured = item.SumInsured,
                Premium = 0m
            });
        }

        _db.Add(quote);
        await _db.SaveChangesAsync();
        return Result<Quotation>.Success(quote);
    }

    public async Task<Result<List<Quotation>>> ListAsync(CommonPaginationRequestModel? requestModel = null)
    {
        var query = _db.Set<Quotation>()
            .Include(q => q.Items)
            .AsNoTracking()
            .OrderByDescending(q => q.CreatedOn);

        if (requestModel != null)
        {
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var quotations = await result.ToListAsync();
            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };
            return Result<List<Quotation>>.Success(quotations, pagination);
        }

        var allQuotations = await query.ToListAsync();
        return Result<List<Quotation>>.Success(allQuotations);
    }

    public async Task<Result<Quotation>> GetByIdAsync(string id)
    {
        var quotation = await _db.Set<Quotation>()
            .Include(q => q.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quotation == null)
            return Result<Quotation>.Failed("Quotation not found.");

        return Result<Quotation>.Success(quotation);
    }

    public async Task<Result<string>> GeneratePdfAsync(string id)
    {
        var quote = await _db.Set<Quotation>().FirstOrDefaultAsync(q => q.Id == id);
        if (quote == null)
            return Result<string>.Failed("Quotation not found.");

        // Placeholder for PDF generation logic
        // In real app, use DinkToPdf or similar, upload to S3, return URL
        var pdfUrl = $"https://storage.example.com/quotes/{id}.pdf";
        
        quote.PdfUrl = pdfUrl;
        await _db.SaveChangesAsync();
        
        return Result<string>.Success(pdfUrl);
    }

    public async Task<Result<List<Quotation>>> GetByLeadIdAsync(string leadId)
    {
        var lead = await _db.Set<Lead>().AsNoTracking().FirstOrDefaultAsync(l => l.Id == leadId);
        if (lead == null)
            return Result<List<Quotation>>.Failed("Lead not found.");

        // Assuming ProspectId links to quotations
        var quotations = await _db.Set<Quotation>()
            .Include(q => q.Items)
            .Where(q => q.ProspectId == lead.ProspectId)
            .OrderByDescending(q => q.CreatedOn)
            .ToListAsync();

        return Result<List<Quotation>>.Success(quotations);
    }

    public async Task<Result<List<Quotation>>> GetQuotationsForAdminAsync(CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<List<Quotation>>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<List<Quotation>>.Failed("User not found or inactive.");

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
                return Result<List<Quotation>>.Failed("Access denied. Admin or SuperAdmin role required.");

            // Build query with includes
            IQueryable<Quotation> query = _db.Set<Quotation>()
                .Include(q => q.Items)
                .AsNoTracking();

            // For SuperAdmin, ignore the automatic tenant query filter to see all quotations across all tenants
            if (isSuperAdmin)
            {
                query = query.IgnoreQueryFilters();
            }
            // For Tenant Admin, the automatic tenant query filter will already filter by their tenant
            // But we can add an explicit filter for clarity and to ensure it works correctly
            else if (!string.IsNullOrEmpty(user.TenantId))
            {
                // Tenant Admin: only show quotations from their tenant
                query = query.Where(q => q.TenantId == user.TenantId);
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(q => q.Status == status);
            }

            // Apply date range filters
            if (from.HasValue)
            {
                query = query.Where(q => q.CreatedOn >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(q => q.CreatedOn <= to.Value);
            }

            // Apply Sieve filtering and pagination
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var quotations = await result.ToListAsync();

            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };

            return Result<List<Quotation>>.Success(quotations, pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quotations for admin: {Message}", ex.Message);
            return Result<List<Quotation>>.Failed($"An error occurred while retrieving quotations: {ex.Message}");
        }
    }

    public async Task<Result<Quotation>> GetQuotationDetailsForAdminAsync(string id)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<Quotation>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<Quotation>.Failed("User not found or inactive.");

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
                return Result<Quotation>.Failed("Access denied. Admin or SuperAdmin role required.");

            // Build query with includes
            IQueryable<Quotation> query = _db.Set<Quotation>()
                .Include(q => q.Items)
                .AsNoTracking();

            // For SuperAdmin, ignore the automatic tenant query filter
            if (isSuperAdmin)
            {
                query = query.IgnoreQueryFilters();
            }

            var quotation = await query.FirstOrDefaultAsync(q => q.Id == id);

            if (quotation == null)
                return Result<Quotation>.Failed("Quotation not found.");

            // For Tenant Admin, verify the quotation belongs to their tenant
            if (!isSuperAdmin && !string.IsNullOrEmpty(user.TenantId) && quotation.TenantId != user.TenantId)
            {
                return Result<Quotation>.Failed("Access denied. Quotation does not belong to your tenant.");
            }

            return Result<Quotation>.Success(quotation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quotation details for admin: {Message}", ex.Message);
            return Result<Quotation>.Failed($"An error occurred while retrieving quotation details: {ex.Message}");
        }
    }

    public async Task<Result<List<Quotation>>> GetQuotationsByTenantIdAsync(string tenantId, CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<List<Quotation>>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<List<Quotation>>.Failed("User not found or inactive.");

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
                return Result<List<Quotation>>.Failed("Access denied. SuperAdmin role required to filter by tenantId.");

            // Verify tenant exists
            var tenant = await _db.Set<Data.Entities.Tenant.Tenant>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive);

            if (tenant == null)
                return Result<List<Quotation>>.Failed("Tenant not found or inactive.");

            // Build query with includes
            IQueryable<Quotation> query = _db.Set<Quotation>()
                .Include(q => q.Items)
                .AsNoTracking()
                .IgnoreQueryFilters() // Ignore automatic tenant filter for SuperAdmin
                .Where(q => q.TenantId == tenantId);

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(q => q.Status == status);
            }

            // Apply date range filters
            if (from.HasValue)
            {
                query = query.Where(q => q.CreatedOn >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(q => q.CreatedOn <= to.Value);
            }

            // Apply Sieve filtering and pagination
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var quotations = await result.ToListAsync();

            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };

            return Result<List<Quotation>>.Success(quotations, pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quotations by tenantId: {Message}", ex.Message);
            return Result<List<Quotation>>.Failed($"An error occurred while retrieving quotations: {ex.Message}");
        }
    }
}
