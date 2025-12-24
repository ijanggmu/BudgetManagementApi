using Models.BeemaEdgeApi.Designation;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IDesignationService
{
    Task<Result<List<DesignationResponseDto>>> GetAllAsync(string? tenantId = null, CancellationToken cancellationToken = default);
    Task<Result<DesignationResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<DesignationResponseDto>> CreateAsync(CreateDesignationDto dto, CancellationToken cancellationToken = default);
    Task<Result<DesignationResponseDto>> UpdateAsync(string id, UpdateDesignationDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

