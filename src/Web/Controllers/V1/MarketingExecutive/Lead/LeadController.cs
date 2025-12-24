using System;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Common.TenantDomain;
using Models.Common;
using Models.WebApi.TenantDTOs;
using System.Threading;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.FoDo.Lead;

public class LeadController(ILeadService leads) : BaseMarketingExecutiveApiController
{
    [HttpPost]
    [Permission(MenuPermissionConstant.MarketingExecutiveLeadsCreate)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateLeadPublicDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await leads.CreateLeadAsync(dto, cancellationToken));
    }

    [HttpGet]
    [Permission(MenuPermissionConstant.MarketingExecutiveLeadsView)]
    public async Task<IActionResult> ListAsync([FromQuery] CommonPaginationRequestModel requestModel, [FromQuery] string? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken = default)
    {
        return HandleResult(await leads.ListAsync(requestModel, cancellationToken));
    }

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.MarketingExecutiveLeadsView)]
    public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await leads.GetByIdAsync(id, cancellationToken));
    }

    [HttpPatch("{id}/status")]
    [Permission(MenuPermissionConstant.MarketingExecutiveLeadsUpdate)]
    public async Task<IActionResult> UpdateStatusAsync(string id, [FromBody] UpdateLeadStatusDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await leads.UpdateStatusAsync(id, dto.Status, cancellationToken));
    }

    [HttpPost("{id}/activities")]
    [Permission(MenuPermissionConstant.MarketingExecutiveLeadsUpdate)]
    public async Task<IActionResult> AddActivityAsync(string id, [FromBody] LeadActivityDto dto, CancellationToken cancellationToken = default)
    {
        return HandleResult(await leads.AddActivityAsync(id, dto, cancellationToken));
    }

    [HttpGet("{id}/activities")]
    [Permission(MenuPermissionConstant.MarketingExecutiveLeadsView)]
    public async Task<IActionResult> GetActivitiesAsync(string id, CancellationToken cancellationToken = default)
    {
        return HandleResult(await leads.GetActivitiesAsync(id, cancellationToken));
    }
}



