using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IQuotationService
{
    Task<Result<QuotationResponseDto>> CreateAsync(CreateQuotationDto dto);
    Task<Result<List<QuotationResponseDto>>> ListAsync(CommonPaginationRequestModel? requestModel = null);
    Task<Result<QuotationResponseDto>> GetByIdAsync(string id);
    Task<Result<byte[]>> GeneratePdfAsync(string id);
    Task<Result<List<QuotationResponseDto>>> GetByLeadIdAsync(string leadId);
    Task<Result<List<QuotationResponseDto>>> GetQuotationsForAdminAsync(CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null);
    Task<Result<QuotationResponseDto>> GetQuotationDetailsForAdminAsync(string id);
    Task<Result<List<QuotationResponseDto>>> GetQuotationsByTenantIdAsync(string tenantId, CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null);
    Task<Result<QuotationResponseDto>> UpdateAsync(string id, UpdateQuotationDto dto);
    Task<Result<bool>> DeleteAsync(string id);
    Task<Result<byte[]>> ExportToExcelAsync(string? status = null, DateTime? from = null, DateTime? to = null, string? tenantId = null);
}
