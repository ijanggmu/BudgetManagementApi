using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IQuotationService
{
    Task<Result<QuotationResponseDto>> CreateAsync(CreateQuotationDto dto, CancellationToken cancellationToken = default);
    Task<Result<List<QuotationResponseDto>>> ListAsync(CommonPaginationRequestModel? requestModel = null, CancellationToken cancellationToken = default);
    Task<Result<QuotationResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<byte[]>> GeneratePdfAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<List<QuotationResponseDto>>> GetByLeadIdAsync(string leadId, CancellationToken cancellationToken = default);
    Task<Result<List<QuotationResponseDto>>> GetQuotationsForAdminAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<QuotationResponseDto>> GetQuotationDetailsForAdminAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<List<QuotationResponseDto>>> GetQuotationsByTenantIdAsync(string tenantId, CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<QuotationResponseDto>> UpdateAsync(string id, UpdateQuotationDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<byte[]>> ExportToExcelAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
}
