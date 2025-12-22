using System;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Common.TenantDomain;
using Models.Common;
using Models.WebApi.TenantDTOs;

namespace BeemaEdgeApi.Controllers.V1.FoDo.Lead;

public class LeadController(ILeadService leads) : BaseMarketingExecutiveApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateLeadPublicDto dto)
    {
        return HandleResult(await leads.CreateLeadAsync(dto));
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync([FromQuery] CommonPaginationRequestModel requestModel, [FromQuery] string? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        return HandleResult(await leads.ListAsync(requestModel));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(string id)
    {
        return HandleResult(await leads.GetByIdAsync(id));
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatusAsync(string id, [FromBody] UpdateLeadStatusDto dto)
    {
        return HandleResult(await leads.UpdateStatusAsync(id, dto.Status));
    }

    [HttpPost("{id}/activities")]
    public async Task<IActionResult> AddActivityAsync(string id, [FromBody] LeadActivityDto dto)
    {
        return HandleResult(await leads.AddActivityAsync(id, dto));
    }

    [HttpGet("{id}/activities")]
    public async Task<IActionResult> GetActivitiesAsync(string id)
    {
        return HandleResult(await leads.GetActivitiesAsync(id));
    }
}



