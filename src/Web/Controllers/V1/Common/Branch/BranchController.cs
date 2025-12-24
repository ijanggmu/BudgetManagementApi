using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.BeemaEdgeApi.Branch;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Common.Branch;

public class BranchController : BaseCommonApiController
{
    private readonly IBranchService _branchService;
    public BranchController(IBranchService branchService)
    {
        _branchService = branchService;
    }

    [HttpGet("Branches")]
    [Permission(MenuPermissionConstant.CommonUtilitiesView)]
    public async Task<IActionResult> GetAllBranch()
    {
        var result = await _branchService.GetAllBranchAsync();
        return HandleResult(result);
    }

}
