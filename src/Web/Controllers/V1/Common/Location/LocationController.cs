using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.BeemaEdgeApi.Province;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Common.Location;

public class LocationController(ILocationService locationService) : BaseCommonApiController
{
    [HttpGet("Provinces")]
    [Permission(MenuPermissionConstant.CommonUtilitiesView)]
    public async Task<IActionResult> GetProvinces()
    {
        var result = await locationService.GetProvincesAsync();
        return HandleResult(result);
    }

    [HttpPost("Districts")]
    [Permission(MenuPermissionConstant.CommonUtilitiesView)]
    public async Task<IActionResult> GetDistricts(string provinceName)
    {
        var result = await locationService.GetDistrictsAsync(provinceName);
        return HandleResult(result);
    }

    [HttpPost("Municipalities")]
    [Permission(MenuPermissionConstant.CommonUtilitiesView)]
    public async Task<IActionResult> GetMunicipalities(string districtName)
    {
        var result = await locationService.GetMunicipalitiesAsync(districtName);
        return HandleResult(result);
    }

    [HttpPost("Wards")]
    [Permission(MenuPermissionConstant.CommonUtilitiesView)]
    public async Task<IActionResult> GetWards(string municipalityName)
    {
        var result = await locationService.GetWardsAsync(municipalityName);
        return HandleResult(result);
    }
}
