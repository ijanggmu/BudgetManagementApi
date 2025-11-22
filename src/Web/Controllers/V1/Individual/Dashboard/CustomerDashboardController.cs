//using System.Threading.Tasks;
//using Business.BeemaEdgeApi.Dashboard;
//using BeemaEdgeApi.Controllers.V1.BaseController;
//using Microsoft.AspNetCore.Mvc;

//namespace BeemaEdgeApi.Controllers.V1.Customer.Dashboard;

//public class CustomerDashboardController(ICustomerDashboardService customerDashboardService) : BaseIndividualApiController
//{
//    [HttpGet("GetCustomerDashboard")]
//    public async Task<IActionResult> GetCustomerDashboard()
//    {
//        var result = await customerDashboardService.GetInsuranceOverviewAsync();
//        return HandleResult(result);
//    }
//}
