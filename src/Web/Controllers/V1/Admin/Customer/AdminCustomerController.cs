//using System.Threading;
//using System.Threading.Tasks;
//using Business.AdminPortalApi.Claim;
//using BeemaEdgeApi.Controllers.V1.BaseController;
//using BeemaEdgeApi.Filters.AuthenticationFilters;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Models.Common;

//namespace BeemaEdgeApi.Controllers.V1.Admin.Customer;
//[AllowAnonymous]
//[HmacAuth]
//public class AdminCustomerController(ICmsCustomerService cmsCustomerService) : BaseAdminApiController
//{
//    [HttpPost]
//    public async Task<IActionResult> GetAll([FromBody] CommonPaginationRequestModel pagination, CancellationToken ct)
//    {
//        var result = await cmsCustomerService.GetAllCustomersAsync(pagination, ct);
//        return HandleResult(result);
//    }
//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(string id, CancellationToken ct)
//    {
//        var result = await cmsCustomerService.GetCustomerByIdAsync(id, ct);
//        return HandleResult(result);
//    }

//    [HttpPost("{id}/approve")]
//    public async Task<IActionResult> ApproveKyc(string id, CancellationToken ct)
//    {
//        var result = await cmsCustomerService.ApproveCustomerKycAsync(id, ct);
//        return HandleResult(result);
//    }

//    [HttpPost("{id}/reject")]
//    public async Task<IActionResult> RejectKyc(string id, [FromBody] RejectKycRequest request, CancellationToken ct)
//    {
//        var result = await cmsCustomerService.RejectCustomerKycAsync(id, request.Reason, ct);
//        return HandleResult(result);
//    }


//}
