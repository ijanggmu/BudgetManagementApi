using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface ILeadService
{
    Task<Result<LeadResponseDto>> CreateLeadAsync(CreateLeadPublicDto dto, CancellationToken cancellationToken = default);
    Task<Result<LeadActivityResponseDto>> AddActivityAsync(string leadId, LeadActivityDto dto, CancellationToken cancellationToken = default);
    Task<Result<List<LeadResponseDto>>> ListAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<LeadResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<LeadResponseDto>> UpdateStatusAsync(string id, string newStatus, CancellationToken cancellationToken = default);
    Task<Result<List<LeadActivityResponseDto>>> GetActivitiesAsync(string leadId, CancellationToken cancellationToken = default);
    Task<Result<List<LeadResponseDto>>> GetLeadsForAdminAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<LeadResponseDto>> GetLeadDetailsForAdminAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<List<LeadResponseDto>>> GetLeadsByTenantIdAsync(string tenantId, CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<Result<byte[]>> ExportToExcelAsync(CommonPaginationRequestModel requestModel, CancellationToken cancellationToken = default);
}
