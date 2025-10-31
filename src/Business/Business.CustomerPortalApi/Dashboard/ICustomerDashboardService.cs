using Models.BeemaEdgeApi.Customer.Dasboard;
using SharedKernel.Operation;

namespace Business.BeemaEdgeApi.Dashboard;
public interface ICustomerDashboardService
{
    Task<Result<InsuranceOverviewResponseModel>> GetInsuranceOverviewAsync();
}
