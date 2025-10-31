using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Common.Location;
using Models.Common.Province;
using Models.BeemaEdgeApi.Customer.CustomerIdentity;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Province;
public interface ILocationService
{
    Task<Result<List<ProvinceViewModel>>> GetProvincesAsync();
    Task<Result<List<DistrictViewModel>>> GetDistrictsAsync(string provinceName);
    Task<Result<List<MunicipalityViewModel>>> GetMunicipalitiesAsync(string districtName);
    Task<Result<List<WardViewModel>>> GetWardsAsync(string municipilatyName);

}
