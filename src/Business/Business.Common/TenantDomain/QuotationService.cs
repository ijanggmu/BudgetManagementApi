using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class QuotationService : IQuotationService
{
    private readonly ApplicationDataContext _db;
    private readonly IQuotationNumberGenerator _numbers;
    private readonly ISieveExtension _sieveExtension;

    public QuotationService(ApplicationDataContext db, IQuotationNumberGenerator numbers, ISieveExtension sieveExtension)
    {
        _db = db;
        _numbers = numbers;
        _sieveExtension = sieveExtension;
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
}
