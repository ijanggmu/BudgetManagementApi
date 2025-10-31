using System.Threading.Tasks;
using Business.BeemaEdgeApi.Province;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Mvc;

namespace BeemaEdgeApi.Controllers.V1.Common.Location;

public class LocationController : BaseCommonApiController
{
    private readonly ILocationService _locationService;
    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("Provinces")]
    public async Task<IActionResult> GetProvinces()
    {
        var result = await _locationService.GetProvincesAsync();
        return HandleResult(result);
    }

    [HttpPost("Districts")]
    public async Task<IActionResult> GetDistricts(string provinceName)
    {
        var result = await _locationService.GetDistrictsAsync(provinceName);
        return HandleResult(result);
    }

    [HttpPost("Municipalities")]
    public async Task<IActionResult> GetMunicipalities(string districtName)
    {
        var result = await _locationService.GetMunicipalitiesAsync(districtName);
        return HandleResult(result);
    }

    [HttpPost("Wards")]
    public async Task<IActionResult> GetWards(string municipalityName)
    {
        var result = await _locationService.GetWardsAsync(municipalityName);
        return HandleResult(result);
    }
}
