using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface ILeadService
{
    Task<Result<LeadResponseDto>> CreateLeadAsync(CreateLeadPublicDto dto);
    Task<Result<LeadActivityResponseDto>> AddActivityAsync(string leadId, LeadActivityDto dto);
    Task<Result<List<LeadResponseDto>>> ListAsync(CommonPaginationRequestModel requestModel);
    Task<Result<LeadResponseDto>> GetByIdAsync(string id);
    Task<Result<LeadResponseDto>> UpdateStatusAsync(string id, string newStatus);
    Task<Result<List<LeadActivityResponseDto>>> GetActivitiesAsync(string leadId);
    Task<Result<List<LeadResponseDto>>> GetLeadsForAdminAsync(CommonPaginationRequestModel requestModel);
    Task<Result<LeadResponseDto>> GetLeadDetailsForAdminAsync(string id);
    Task<Result<List<LeadResponseDto>>> GetLeadsByTenantIdAsync(string tenantId, CommonPaginationRequestModel requestModel);
    Task<Result<byte[]>> ExportToExcelAsync(string? status = null, DateTime? from = null, DateTime? to = null, string tenantId = null);
}
