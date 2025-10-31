using System.Threading.Tasks;
using Business.BeemaEdgeApi.Bank;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BeemaEdgeApi.Controllers.V1.Common.Bank;
[Route("api/[controller]")]
[ApiController]
public class BankController(IBankService bankService) : BaseCommonApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllBank()
    {
        var result = await bankService.GetAllBankAsync();
        return HandleResult(result);
    }
}
