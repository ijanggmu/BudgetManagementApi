using System.Threading.Tasks;
using Business.Common.Country;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace BeemaEdgeApi.Controllers.V1.Common.OTP;

public class CountryController(ICountryService countryService) : BaseCommonApiController
{
    [AllowAnonymous]
    [HttpPost("GetAllCountryList")]

    public async Task<IActionResult> GetAllCountryListPagination(CommonPaginationRequestModel requestModel)
    {
        var result = await countryService.GetCountryListAsync(requestModel);

        return HandleResult(result);
    }

}
