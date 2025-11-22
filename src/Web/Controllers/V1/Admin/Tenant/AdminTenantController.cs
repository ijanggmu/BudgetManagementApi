using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Controllers.V1.Admin.Tenants;

public class AdminTenantController(ITenantAdminService service) : BaseAdminApiController
{
    [HttpGet]
    public async Task<IActionResult> ListAsync([FromQuery] CommonPaginationRequestModel? requestModel)
    {
        return HandleResult(await service.ListAsync(requestModel));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id)
    {
        return HandleResult(await service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTenantDto dto)
    {
        return HandleResult(await service.CreateAsync(dto));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateTenantDto dto)
    {
        return HandleResult(await service.UpdateAsync(id, dto));
    }
}
