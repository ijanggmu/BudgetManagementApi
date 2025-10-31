//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.Dashboard;
//using Microsoft.AspNetCore.Mvc;

//namespace BeemaEdgeApi.Controllers.V1.Admin.Dashboard;

//public class AdminDashboardController(ICustomerDashboardService customerDashboardService) : BaseApiController
//{
//    [HttpGet("GetCustomerDashboard")]
//    public async Task<IActionResult> GetCustomerDashboard()
//    {

//        var result = await customerDashboardService.GetInsuranceOverviewAsync();
//        return HandleResult(result);
//    }


//}


