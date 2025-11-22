using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Tenant;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IQuotationService
{
    Task<Result<Quotation>> CreateAsync(CreateQuotationDto dto);
    Task<Result<List<Quotation>>> ListAsync(CommonPaginationRequestModel? requestModel = null);
    Task<Result<Quotation>> GetByIdAsync(string id);
    Task<Result<string>> GeneratePdfAsync(string id);
    Task<Result<List<Quotation>>> GetByLeadIdAsync(string leadId);
    Task<Result<List<Quotation>>> GetQuotationsForAdminAsync(CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null);
    Task<Result<Quotation>> GetQuotationDetailsForAdminAsync(string id);
    Task<Result<List<Quotation>>> GetQuotationsByTenantIdAsync(string tenantId, CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null);
}
