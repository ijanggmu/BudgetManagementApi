using BeemaEdgeApi.Controllers.V1.BaseController;
using Infrastructure.CoreApi.CoreApi;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Operation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeemaEdgeApi.Controllers.V1.Common.Business;

public class BusinessController : BaseCommonApiController
{

    //[HttpGet("BusinessTypes")]
    //public IActionResult GetBusinessTypes()
    //{
    //    // Dummy implementation
    //    var businessTypes = new List<string> { "BusinessType1", "BusinessType2", "BusinessType3" };
    //    var result = Result<List<string>>.Success(businessTypes);
    //    return HandleResult(result);
    //}

    //public interface IBusinessTypeService
    //{
    //    Task<Result<List<string>>> GetBusinessTypesAsync();
    //}

    //public class BusinessTypeService : IBusinessTypeService
    //{
    //    private readonly ICoreApiService _coreApiService;

    //    public BusinessTypeService(ICoreApiService coreApiService)
    //    {
    //        _coreApiService = coreApiService;
    //    }
    //    public async Task<Result<List<string>>> GetBusinessTypesAsync()
    //    {
    //        var result = await _coreApiService.GetBusinessTypesAsync();
    //        return await Task.FromResult(Result<List<string>>.Success(result.Data));
    //    }
    //}
}
