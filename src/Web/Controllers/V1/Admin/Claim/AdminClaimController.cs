//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.Claim;
//using Microsoft.AspNetCore.Mvc;
//using Models.Common;

//namespace BeemaEdgeApi.Controllers.V1.Admin.Claim;

//public class AdminClaimController(ICustomerClaimService customerClaimService) : BaseApiController
//{

//    [HttpGet("Claims")]
//    public async Task<IActionResult> GetAllCustomerClaim(CommonPaginationRequestModel requestModel)
//    {
//        var result = await customerClaimService.GetCustomerClaimAsync(requestModel);
//        return HandleResult(result);
//    }

//    [HttpGet("Claim/{id}")]
//    public async Task<IActionResult> GetClaimDetails(string id)
//    {
//        var result = await customerClaimService.GetCustomerClaimByIdAsync(id);
//        return HandleResult(result);
//    }

//    [HttpPost("ApplyClaim")]
//    public async Task<IActionResult> ApplyClaim(string id)
//    {
//        var result = await customerClaimService.ApplyClaimAsync(id);
//        return HandleResult(result);
//    }

//    [HttpPut("UpdateClaim")]
//    public async Task<IActionResult> UpdateClaim(string id)
//    {
//        var result = await customerClaimService.UpdateClaimAsync(id);
//        return HandleResult(result);
//    }

//    [HttpPost("GetHospitalNames")]
//    public async Task<IActionResult> GetHospitalNames(CommonPaginationRequestModel requestModel)
//    {
//        var result = await customerClaimService.GetHospitalNamesAsync(requestModel);
//        return HandleResult(result);
//    }

//}


