using System.Text.Json;
using Business.AdminPortalApi.ExcelExport;
using Business.AdminPortalApi.PdfGeneration;
using Business.Common.PolicyCalculator;
using Data.Context;
using Data.Entities.Identity;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.Common.Policy.ThirdPartyApi.e2e;
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
    private readonly IPolicyPremiumCalculatorService _calculatePremium;

    public QuotationService(
        ApplicationDataContext db,
        IQuotationNumberGenerator numbers,
        ISieveExtension sieveExtension,
        IUserProfileService userProfileService,
        UserManager<ApplicationUser> userManager,
        ILogger<QuotationService> logger,
        IExcelExportService excelExportService,
        IQuotationPdfService pdfService,
        ITenantContext tenantContext,
        IPolicyPremiumCalculatorService calculatePremium)
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
        _calculatePremium = calculatePremium;
    }

    private static QuotationResponseDto MapToDto(Quotation quotation)
    {
        var premiumCalculation = new CalculationPremium();
        if (!string.IsNullOrEmpty(quotation.PremiumCalculationJson))
        {
            try
            {
                premiumCalculation = JsonSerializer.Deserialize<CalculationPremium>(quotation.PremiumCalculationJson);
            }
            catch
            {
                premiumCalculation = null;
            }
        }
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
            )).ToList() ?? new List<QuotationItemResponseDto>(),
            premiumCalculation
        );
    }

    public async Task<Result<QuotationResponseDto>> CreateAsync(CreateQuotationDto dto, CancellationToken cancellationToken = default)
    {
        var premiumCalculation = await _calculatePremium.CalculatePolicyPremiumAsync(dto.PremiumRequestModel);
        if(!premiumCalculation.IsSuccess){
            return Result<QuotationResponseDto>.Failed($"An error occurred while calculating Premium");
        }
        var premiumCalculationJson = JsonSerializer.Serialize(premiumCalculation.Data);
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var quote = new Quotation
            {
                Number = await _numbers.NextAsync(),
                ProductId = dto.ProductId.ToString(),
                ProspectId = dto.ProspectId.ToString(),
                InsuranceType = dto.PremiumRequestModel.InsuranceType,
                PortfolioAlias = premiumCalculation.Data.PortfolioAlias,
                TotalPremium = premiumCalculation.Data.NetPremium,
                ValidUntil = dto.ValidUntil,
                PremiumCalculationJson = premiumCalculationJson,
                IsDirectBusiness = dto.IsDirectBusiness?? false,
            };
            if (dto.Items != null && dto.Items.Any())
            {
                foreach (var item in dto.Items)
                {
                    quote.Items.Add(new QuotationItem
                    {
                        CoverageId = item.CoverageId.ToString(),
                        SumInsured = item.SumInsured,
                        Premium = 0m
                    });
                }
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

    public async Task<Result<List<QuotationResponseDto>>> ListAsync(CommonPaginationRequestModel requestModel = null, CancellationToken cancellationToken = default)
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

    public async Task<Result<QuotationResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var quotation = await _db.Set<Quotation>()
            .Include(q => q.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quotation == null)
            return Result<QuotationResponseDto>.Failed("Quotation not found.");

        return Result<QuotationResponseDto>.Success(MapToDto(quotation));
    }

    public async Task<Result<byte[]>> GeneratePdfAsync(string id, CancellationToken cancellationToken = default)
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

    public async Task<Result<List<QuotationResponseDto>>> GetByLeadIdAsync(string leadId, CancellationToken cancellationToken = default)
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

    public async Task<Result<List<QuotationResponseDto>>> GetQuotationsForAdminAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
            // Only check role for query filtering logic
            var roleId = _userProfileService.GetRoleId();
            var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

            // Build query with includes
            IQueryable<Quotation> query = _db.Set<Quotation>()
                .Include(q => q.Items)
                .AsNoTracking();
            // Note: IsDeleted and TenantId filters are now applied globally via query filters

            // For SuperAdmin, ignore the automatic tenant query filter to see all quotations across all tenants
            if (isSuperAdmin)
            {
                query = query.IgnoreQueryFilters();
            }
            // For Tenant Admin, the automatic tenant query filter will already filter by their tenant


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

    public async Task<Result<QuotationResponseDto>> GetQuotationDetailsForAdminAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            // Role authorization is handled by [AdminOrSuperAdmin] filter attribute on controller
            var roleId = _userProfileService.GetRoleId();
            var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

            // Build query with includes
            IQueryable<Quotation> query = _db.Set<Quotation>()
                .Include(q => q.Items)
                .AsNoTracking();
            // Note: IsDeleted and TenantId filters are now applied globally via query filters

            // For SuperAdmin, ignore the automatic tenant query filter
            if (isSuperAdmin)
            {
                query = query.IgnoreQueryFilters();
            }

            var quotation = await query.FirstOrDefaultAsync(q => q.Id == id);

            if (quotation == null)
                return Result<QuotationResponseDto>.Failed("Quotation not found.");

            // For Tenant Admin, global query filter ensures tenant isolation

            return Result<QuotationResponseDto>.Success(MapToDto(quotation));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quotation details for admin: {Message}", ex.Message);
            return Result<QuotationResponseDto>.Failed($"An error occurred while retrieving quotation details: {ex.Message}");
        }
    }

    public async Task<Result<List<QuotationResponseDto>>> GetQuotationsByTenantIdAsync(string tenantId, CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
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

            // Role authorization is handled by [SuperAdminOnly] filter attribute on controller
            // This endpoint is SuperAdmin only, so we can always use IgnoreQueryFilters

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

    public async Task<Result<QuotationResponseDto>> UpdateAsync(string id, UpdateQuotationDto dto, CancellationToken cancellationToken = default)
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

    public async Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
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

    public async Task<Result<byte[]>> ExportToExcelAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            requestModel.PageSize = -1;
            var result = await GetQuotationsForAdminAsync(requestModel, cancellationToken);

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
