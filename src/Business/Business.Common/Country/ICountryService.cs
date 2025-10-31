using Models.Common;
using Models.Common.Country;
using SharedKernel.Operation;

namespace Business.Common.Country;
public interface ICountryService
{
    Task<Result<List<CountryResponseModel>>> GetCountryListAsync(CommonPaginationRequestModel requestModel);
}

