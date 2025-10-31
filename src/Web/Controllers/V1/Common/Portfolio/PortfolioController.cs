using System.Collections.Generic;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Infrastructure.CoreApi.CoreApi;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Operation;

namespace BeemaEdgeApi.Controllers.V1.Common.Portfolio;

public class PortfolioController(IPortfolioService portfolioService) : BaseCommonApiController
{

    [HttpGet("List")]
    public async Task<IActionResult> GetPortfolioList()
    {
        var result = await portfolioService.GetPortfolioListAsync();
        return HandleResult(result);
    }


    //[HttpGet("Grouped")]
    //public async Task<IActionResult> GetGroupedByPortfolio()
    //{
    //    var result = await portfolioService.GetGroupedByPortfolioAsync();

    //    return HandleResult(result);
    //}
}
public interface IPortfolioService
{
    Task<Result<List<string>>> GetPortfolioListAsync();
    Task<Result<Dictionary<string, List<string>>>> GetGroupedByPortfolioAsync();
}
public class PortfolioService(ICoreApiService coreApiService) : IPortfolioService
{
    public async Task<Result<List<string>>> GetPortfolioListAsync()
    {
        var result = await coreApiService.GetPortfolioListAsync();
        return Result<List<string>>.Success(result);
    }

    public async Task<Result<Dictionary<string, List<string>>>> GetGroupedByPortfolioAsync()
    {
        var result = await coreApiService.GetGroupedByPortfolioAsync();
        return Result<Dictionary<string, List<string>>>.Success(result.Data);
    }
}
