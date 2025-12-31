using Models.BeemaEdgeApi.Entity;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public interface IEntitySettingsService
{
    Task<Result<EntitySettingsResponseDto>> GetAsync(CancellationToken cancellationToken = default);
    Task<Result<EntitySettingsResponseDto>> UpdateAsync(UpdateEntitySettingsDto dto, CancellationToken cancellationToken = default);
}




