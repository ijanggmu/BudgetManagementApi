using Models.BeemaEdgeApi.Customer.Dasboard;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Dashboard;
public interface IAdminDashboardService
{
    Task<Result<InsuranceOverviewResponseModel>> GetInsuranceOverviewAsync();
}
