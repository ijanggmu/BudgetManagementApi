using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Tenant;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface ILeadService
{
    Task<Result<Lead>> CreateLeadAsync(CreateLeadPublicDto dto);
    Task<Result<LeadActivity>> AddActivityAsync(string leadId, LeadActivityDto dto);
    Task<Result<List<Lead>>> ListAsync(CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null);
    Task<Result<Lead>> GetByIdAsync(string id);
    Task<Result<Lead>> UpdateStatusAsync(string id, string newStatus);
    Task<Result<List<LeadActivity>>> GetActivitiesAsync(string leadId);
    Task<Result<List<Lead>>> GetLeadsForAdminAsync(CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null);
    Task<Result<Lead>> GetLeadDetailsForAdminAsync(string id);
    Task<Result<List<Lead>>> GetLeadsByTenantIdAsync(string tenantId, CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null);
}
