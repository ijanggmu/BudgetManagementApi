using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Common.Location;
using Models.Common.Province;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Province;
public class LocationService : ILocationService
{
    public LocationService()
    {
    }

    public async Task<Result<List<ProvinceViewModel>>> GetProvincesAsync()
    {
        var result = NepalLocationData.GetProvinces();
        return await Task.FromResult(Result<List<ProvinceViewModel>>.Success(result));
    }

    public async Task<Result<List<DistrictViewModel>>> GetDistrictsAsync(string provinceName)
    {
        var result = NepalLocationData.GetDistricts(provinceName);
        return await Task.FromResult(Result<List<DistrictViewModel>>.Success(result));
    }

    public async Task<Result<List<MunicipalityViewModel>>> GetMunicipalitiesAsync(string districtName)
    {
        var result = NepalLocationData.GetMunicipalities(districtName);
        return await Task.FromResult(Result<List<MunicipalityViewModel>>.Success(result));
    }

    public async Task<Result<List<WardViewModel>>> GetWardsAsync(string municipalityName)
    {
        var result = NepalLocationData.GetWards(municipalityName);
        return await Task.FromResult(Result<List<WardViewModel>>.Success(result));
    }

}
