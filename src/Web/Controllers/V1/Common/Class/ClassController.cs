using System.Collections.Generic;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Infrastructure.CoreApi.CoreApi;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Operation;
using static BeemaEdgeApi.Controllers.V1.Common.Class.ClassController;

namespace BeemaEdgeApi.Controllers.V1.Common.Class;

public class ClassController(IClassService classService) : BaseCommonApiController
{
    [HttpGet("List")]
    public async Task<IActionResult> GetClassList()
    {
        var result = await classService.GetClassListAsync();
        return HandleResult(result);
    }

    [HttpGet("ByPortfolio/{portfolioId}")]
    public async Task<IActionResult> GetClassesByPortfolio(string portfolioId)
    {
        var result = await classService.GetClassesByPortfolioAsync(portfolioId);
        return HandleResult(result);
    }


    public interface IClassService
    {
        Task<Result<List<string>>> GetClassListAsync();
        Task<Result<List<string>>> GetClassesByPortfolioAsync(string portfolioId);
    }

    public class ClassService : IClassService
    {
        private readonly ICoreApiService _coreApiService;
        public ClassService(ICoreApiService coreApiService)
        {
            _coreApiService = coreApiService;
        }

        public async Task<Result<List<string>>> GetClassListAsync()
        {
            var result = await _coreApiService.GetClassListAsync();
            return Result<List<string>>.Success(result.Data);
        }

        public async Task<Result<List<string>>> GetClassesByPortfolioAsync(string portfolioId)
        {
            var result = await _coreApiService.GetClassesByPortfolioAsync();
            return Result<List<string>>.Success(result.Data);
        }
    }
}
