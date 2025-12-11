using Business.AdminPortalApi.ExcelExport;
using Business.AdminPortalApi.PdfGeneration;
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
using SharedKernel.Models.Tenancy;
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
    private readonly IExcelExportService _excelExportService;
    private readonly IQuotationPdfService _pdfService;
    private readonly ITenantContext _tenantContext;

    public QuotationService(
        ApplicationDataContext db,
        IQuotationNumberGenerator numbers,
        ISieveExtension sieveExtension,
        IUserProfileService userProfileService,
        UserManager<ApplicationUser> userManager,
        ILogger<QuotationService> logger,
        IExcelExportService excelExportService,
        IQuotationPdfService pdfService,
        ITenantContext tenantContext)
    {
        _db = db;
        _numbers = numbers;
        _sieveExtension = sieveExtension;
        _userProfileService = userProfileService;
        _userManager = userManager;
        _logger = logger;
        _excelExportService = excelExportService;
        _pdfService = pdfService;
        _tenantContext = tenantContext;
    }

    private static QuotationResponseDto MapToDto(Quotation quotation)
    {
        return new QuotationResponseDto(
            quotation.Id,
            quotation.Number,
            quotation.Status,
            quotation.ProductId,
            quotation.ProspectId,
            quotation.TotalPremium,
            quotation.DiscountPercent,
            quotation.ValidUntil,
            quotation.PdfUrl,
            quotation.CreatedOn,
            quotation.Items?.Select(item => new QuotationItemResponseDto(
                item.Id,
                item.QuotationId,
                item.CoverageId,
                item.SumInsured,
                item.Premium
            )).ToList() ?? new List<QuotationItemResponseDto>()
        );
    }

    public async Task<Result<QuotationResponseDto>> CreateAsync(CreateQuotationDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
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
            await transaction.CommitAsync();

            // Reload with items
            await _db.Entry(quote).Collection(q => q.Items).LoadAsync();

            return Result<QuotationResponseDto>.Success(MapToDto(quote));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating quotation: {Message}", ex.Message);
            return Result<QuotationResponseDto>.Failed($"An error occurred while creating quotation: {ex.Message}");
        }
    }

    public async Task<Result<List<QuotationResponseDto>>> ListAsync(CommonPaginationRequestModel? requestModel = null)
    {
        var query = _db.Set<Quotation>()
            .Include(q => q.Items)
            .AsNoTracking()
            .OrderByDescending(q => q.CreatedOn);

        if (requestModel != null)
        {
            var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
            var quotations = await result.ToListAsync();
            var quotationDtos = quotations.Select(MapToDto).ToList();
            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };
            return Result<List<QuotationResponseDto>>.Success(quotationDtos, pagination);
        }

        var allQuotations = await query.ToListAsync();
        var allQuotationDtos = allQuotations.Select(MapToDto).ToList();
        return Result<List<QuotationResponseDto>>.Success(allQuotationDtos);
    }

    public async Task<Result<QuotationResponseDto>> GetByIdAsync(string id)
    {
        var quotation = await _db.Set<Quotation>()
            .Include(q => q.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quotation == null)
            return Result<QuotationResponseDto>.Failed("Quotation not found.");

        return Result<QuotationResponseDto>.Success(MapToDto(quotation));
    }

    public async Task<Result<byte[]>> GeneratePdfAsync(string id)
    {
        try
        {
            var quote = await _db.Set<Quotation>()
                .Include(q => q.Items)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
                return Result<byte[]>.Failed("Quotation not found.");

            // Get quotation DTO
            var quotationDto = MapToDto(quote);

            // Get prospect and contact information
            var prospect = await _db.Set<Prospect>()
                .Include(p => p.PrimaryContact)
                .FirstOrDefaultAsync(p => p.Id == quote.ProspectId);

            var prospectName = prospect?.PrimaryContact?.FullName ?? "Customer";
            var prospectEmail = prospect?.PrimaryContact?.Email;
            var prospectPhone = prospect?.PrimaryContact?.Phone;

            // Get tenant and branding information
            string? companyName = null;
            string? logoUrl = null;

            if (_tenantContext.TenantId != null)
            {
                var tenant = await _db.Set<Tenant>()
                    .Include(t => t.Branding)
                    .FirstOrDefaultAsync(t => t.Id == _tenantContext.TenantId);

                companyName = tenant?.Name;
                logoUrl = tenant?.Branding?.LogoUrl;
            }

            // Generate PDF
            var pdfBytes = await _pdfService.GenerateQuotationPdfAsync(
                quotationDto,
                prospectName,
                prospectEmail,
                prospectPhone,
                companyName,
                logoUrl);

            return Result<byte[]>.Success(pdfBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF for quotation {QuotationId}: {Message}", id, ex.Message);
            return Result<byte[]>.Failed($"An error occurred while generating PDF: {ex.Message}");
        }
    }

    public async Task<Result<List<QuotationResponseDto>>> GetByLeadIdAsync(string leadId)
    {
        var lead = await _db.Set<Lead>().AsNoTracking().FirstOrDefaultAsync(l => l.Id == leadId);
        if (lead == null)
            return Result<List<QuotationResponseDto>>.Failed("Lead not found.");

        // Assuming ProspectId links to quotations
        var quotations = await _db.Set<Quotation>()
            .Include(q => q.Items)
            .Where(q => q.ProspectId == lead.ProspectId)
            .OrderByDescending(q => q.CreatedOn)
            .ToListAsync();

        var quotationDtos = quotations.Select(MapToDto).ToList();
        return Result<List<QuotationResponseDto>>.Success(quotationDtos);
    }

    public async Task<Result<List<QuotationResponseDto>>> GetQuotationsForAdminAsync(CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<List<QuotationResponseDto>>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<List<QuotationResponseDto>>.Failed("User not found or inactive.");

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
                return Result<List<QuotationResponseDto>>.Failed("Access denied. Admin or SuperAdmin role required.");

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

            var quotationDtos = quotations.Select(MapToDto).ToList();

            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };

            return Result<List<QuotationResponseDto>>.Success(quotationDtos, pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quotations for admin: {Message}", ex.Message);
            return Result<List<QuotationResponseDto>>.Failed($"An error occurred while retrieving quotations: {ex.Message}");
        }
    }

    public async Task<Result<QuotationResponseDto>> GetQuotationDetailsForAdminAsync(string id)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<QuotationResponseDto>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<QuotationResponseDto>.Failed("User not found or inactive.");

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
                return Result<QuotationResponseDto>.Failed("Access denied. Admin or SuperAdmin role required.");

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
                return Result<QuotationResponseDto>.Failed("Quotation not found.");

            // For Tenant Admin, verify the quotation belongs to their tenant
            if (!isSuperAdmin && !string.IsNullOrEmpty(user.TenantId) && quotation.TenantId != user.TenantId)
            {
                return Result<QuotationResponseDto>.Failed("Access denied. Quotation does not belong to your tenant.");
            }

            return Result<QuotationResponseDto>.Success(MapToDto(quotation));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quotation details for admin: {Message}", ex.Message);
            return Result<QuotationResponseDto>.Failed($"An error occurred while retrieving quotation details: {ex.Message}");
        }
    }

    public async Task<Result<List<QuotationResponseDto>>> GetQuotationsByTenantIdAsync(string tenantId, CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var userId = _userProfileService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Result<List<QuotationResponseDto>>.Failed("User not authenticated.");

            // Get current user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted || user.IsDisabled)
                return Result<List<QuotationResponseDto>>.Failed("User not found or inactive.");

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
                return Result<List<QuotationResponseDto>>.Failed("Access denied. SuperAdmin role required to filter by tenantId.");

            // Verify tenant exists
            var tenant = await _db.Set<Data.Entities.Tenant.Tenant>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive);

            if (tenant == null)
                return Result<List<QuotationResponseDto>>.Failed("Tenant not found or inactive.");

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

            var quotationDtos = quotations.Select(MapToDto).ToList();

            var pagination = new Pagination
            {
                TotalItems = totalCount,
                TotalPages = totalPage,
                PageSize = requestModel.PageSize,
                CurrentPage = requestModel.PageNumber
            };

            return Result<List<QuotationResponseDto>>.Success(quotationDtos, pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quotations by tenantId: {Message}", ex.Message);
            return Result<List<QuotationResponseDto>>.Failed($"An error occurred while retrieving quotations: {ex.Message}");
        }
    }

    public async Task<Result<QuotationResponseDto>> UpdateAsync(string id, UpdateQuotationDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var quotation = await _db.Set<Quotation>()
                .Include(q => q.Items)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quotation == null)
                return Result<QuotationResponseDto>.Failed("Quotation not found.");

            // Update properties if provided
            if (!string.IsNullOrEmpty(dto.Status))
                quotation.Status = dto.Status;

            if (dto.TotalPremium.HasValue)
                quotation.TotalPremium = dto.TotalPremium.Value;

            if (dto.DiscountPercent.HasValue)
                quotation.DiscountPercent = dto.DiscountPercent.Value;

            if (dto.ValidUntil.HasValue)
                quotation.ValidUntil = dto.ValidUntil.Value;

            // Update items if provided
            if (dto.Items != null && dto.Items.Any())
            {
                // Remove existing items
                _db.Set<QuotationItem>().RemoveRange(quotation.Items);

                // Add new items
                foreach (var itemDto in dto.Items)
                {
                    quotation.Items.Add(new QuotationItem
                    {
                        CoverageId = itemDto.CoverageId.ToString(),
                        SumInsured = itemDto.SumInsured,
                        Premium = 0m // Can be calculated later
                    });
                }
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            // Reload with items
            await _db.Entry(quotation).Collection(q => q.Items).LoadAsync();

            return Result<QuotationResponseDto>.Success(MapToDto(quotation));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating quotation {QuotationId}: {Message}", id, ex.Message);
            return Result<QuotationResponseDto>.Failed($"An error occurred while updating quotation: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(string id)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var quotation = await _db.Set<Quotation>()
                .Include(q => q.Items)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quotation == null)
                return Result<bool>.Failed("Quotation not found.");

            // Delete items first (if cascade delete is not configured)
            _db.Set<QuotationItem>().RemoveRange(quotation.Items);

            // Delete quotation
            _db.Set<Quotation>().Remove(quotation);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting quotation {QuotationId}: {Message}", id, ex.Message);
            return Result<bool>.Failed($"An error occurred while deleting quotation: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> ExportToExcelAsync(string? status = null, DateTime? from = null, DateTime? to = null, string? tenantId = null)
    {
        try
        {
            var requestModel = new CommonPaginationRequestModel { PageNumber = 1, PageSize = int.MaxValue };
            var result = string.IsNullOrEmpty(tenantId)
                ? await GetQuotationsForAdminAsync(requestModel, status, from, to)
                : await GetQuotationsByTenantIdAsync(tenantId, requestModel, status, from, to);

            if (!result.IsSuccess || result.Data == null)
                return Result<byte[]>.Failed(result.Error ?? "Failed to retrieve quotation data.");

            var columnMappings = new Dictionary<string, string>
            {
                { "Id", "ID" },
                { "Number", "Quotation Number" },
                { "Status", "Status" },
                { "ProductId", "Product ID" },
                { "ProspectId", "Prospect ID" },
                { "TotalPremium", "Total Premium" },
                { "DiscountPercent", "Discount %" },
                { "ValidUntil", "Valid Until" },
                { "PdfUrl", "PDF URL" },
                { "CreatedOn", "Created On" },
                { "Items", "Items Count" }
            };

            // Flatten the data for export
            var exportData = result.Data.Select(quotation => new
            {
                quotation.Id,
                quotation.Number,
                quotation.Status,
                quotation.ProductId,
                quotation.ProspectId,
                TotalPremium = quotation.TotalPremium?.ToString("F2") ?? "",
                DiscountPercent = quotation.DiscountPercent?.ToString("F2") ?? "",
                ValidUntil = quotation.ValidUntil?.ToString("yyyy-MM-dd") ?? "",
                PdfUrl = quotation.PdfUrl ?? "",
                quotation.CreatedOn,
                ItemsCount = quotation.Items?.Count ?? 0
            }).ToList();

            var excelData = await _excelExportService.ExportToExcelAsync(exportData, "Quotations", columnMappings);
            return Result<byte[]>.Success(excelData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting quotations to Excel: {Message}", ex.Message);
            return Result<byte[]>.Failed($"An error occurred while exporting: {ex.Message}");
        }
    }
}
