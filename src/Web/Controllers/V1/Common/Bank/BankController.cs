using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.BeemaEdgeApi.Bank;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Common.Bank;
[Route("api/[controller]")]
[ApiController]
public class BankController(IBankService bankService) : BaseCommonApiController
{
    [HttpGet]
    [Permission(MenuPermissionConstant.CommonUtilitiesView)]
    public async Task<IActionResult> GetAllBank()
    {
        var result = await bankService.GetAllBankAsync();
        return HandleResult(result);
    }
}
