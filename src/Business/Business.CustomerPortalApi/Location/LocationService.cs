using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.CoreApi.CoreApi;
using Models.Common.Location;
using Models.Common.Policy.Manufacturer;
using Models.Common.Province;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Province;
public class LocationService : ILocationService
{
    private readonly ICoreApiService _coreApiService;

    public LocationService(ICoreApiService coreApiService)
    {
        _coreApiService = coreApiService;
    }


    public async Task<Result<List<ProvinceViewModel>>> GetProvincesAsync()
    {
        var result =  await _coreApiService.GetProvincesAsync();
        return Result<List<ProvinceViewModel>>.Success(result);
    }

    public async Task<Result<List<DistrictViewModel>>> GetDistrictsAsync(string provinceName)
    {
        var result = await _coreApiService.GetDistrictsAsync(provinceName);
        return Result<List<DistrictViewModel>>.Success(result);
    }

    public async Task<Result<List<MunicipalityViewModel>>> GetMunicipalitiesAsync(string provinceName)
    {
        var result = await _coreApiService.GetMunicipalitiesAsync(provinceName);
        return Result<List<MunicipalityViewModel>>.Success(result);
    }

    public async Task<Result<List<WardViewModel>>> GetWardsAsync(string municipalityName)
    {
        var result = await _coreApiService.GetWardsAsync(municipalityName);
        return Result<List<WardViewModel>>.Success(result);
    }

}
