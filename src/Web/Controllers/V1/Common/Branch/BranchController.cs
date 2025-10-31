using System.Threading.Tasks;
using Business.BeemaEdgeApi.Branch;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Mvc;

namespace BeemaEdgeApi.Controllers.V1.Common.Branch;

public class BranchController : BaseCommonApiController
{
    private readonly IBranchService _branchService;
    public BranchController(IBranchService branchService)
    {
        _branchService = branchService;
    }

    [HttpGet("Branches")]
    public async Task<IActionResult> GetAllBranch()
    {
        var result = await _branchService.GetAllBranchAsync();
        return HandleResult(result);
    }

}
