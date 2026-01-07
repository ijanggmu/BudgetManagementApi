using System.Threading.Tasks;
using Business.Common.PolicyCalculator;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Microsoft.AspNetCore.Mvc;
using Models.Common.Policy.Policy;
using SharedKernel.Constant.Permission;
using Business.Common.PremiumCalculation.Service;
using Data.Entities.Calculation;
using Models.Common.Policy;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BeemaEdgeApi.Controllers.V1.Common.Premium;

public class PropertySubsidySILimitController(IPropertySubsidySILimitService propertySubsidySILimitService) : BaseCommonApiController
{
    //[HttpPost("GetDTResult")]
    //public IActionResult GetDatatableResult([FromBody] PPTYSubsidyDataParameters parameters)
    //{
    //    Tuple<int, IQueryable<PropertySubsidySILimit>> SurveyorResult = null;
    //    SurveyorResult = _propertySubsidySILimitService.AllIncluding(parameters.Start,
    //                                            parameters.Length,
    //                                            parameters.SortOrder,
    //                                            parameters.Order[0].Dir.ToString(),
    //                                                    x => !x.IsDeleted && (x.SILabel ?? "").ToLower().Contains(parameters.Search.Value.ToLower()));




    //    // (x.EndorsementType ?? "").ToLower().Contains(parameters.Search.Value.ToLower()))  )); || (string.IsNullOrEmpty(parameters.ClassCode) || x.Id.Equals(parameters.ClassCode)

    //    var surveyor = SurveyorResult.Item2.ToList().OrderBy(x => x.CreatedDate);
    //    var SurveyorModel = surveyor.Select(x => new PropertySubsidySILimitViewModel
    //    {
    //        Id = x.Id.ToString(),
    //        SILimit = x.SILimit,
    //        SILabel = x.SILabel,
    //        CreatedDate = x.CreatedDate.ToString(),

    //    }); ;



    //    //SurveyorModel = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), SurveyorModel.AsQueryable());

    //    List<PropertySubsidySILimitViewModel> filteredAgent;
    //    int totalAgent = SurveyorResult.Item1;
    //    var counter = parameters.Start + 1;
    //    filteredAgent = SurveyorModel.Select(x =>
    //    {
    //        x.Sn = counter++;

    //        return x;
    //    }).ToList();
    //    var result = new DTResult<PropertySubsidySILimitViewModel>
    //    {
    //        Draw = parameters.Draw,
    //        Data = filteredAgent,
    //        RecordsFiltered = totalAgent,
    //        RecordsTotal = totalAgent
    //    };
    //    return new OkObjectResult(result);
    //}

    [HttpGet("GetAllSubsidySI")]
    public IActionResult GetAllSubsidySI()
    {
        return Ok(propertySubsidySILimitService.GetAllSubsidySI());
    }

    [HttpPut("UpdateSubsidySILimit/{id}")]
    public async Task<IActionResult> UpdateSubsidySILimit(PropertySubsidySILimitViewModel model, string id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var subsidyLimit = await propertySubsidySILimitService.GetSingleSubsidySILimit(id);
        if (subsidyLimit == null) return NotFound();

        await propertySubsidySILimitService.UpdateSubsidySILimitAsync(model);
        return NoContent();
    }

    [HttpGet("GetSingleSubsidySILimit/{id}")]
    public async Task<IActionResult> GetSingleSubsidySILimit(string id)
    {
        var surveyorList = await propertySubsidySILimitService.GetSingleSubsidySILimit(id);
        if (surveyorList == null) return NotFound();
        return Ok(surveyorList);
    }
}

