using Models.BeemaEdgeApi.Customer.Dasboard;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Dashboard;

public class AdminDashboardService : IAdminDashboardService
{
    public async Task<Result<InsuranceOverviewResponseModel>> GetInsuranceOverviewAsync()
    {
        // Simulate fetching data from a database or external service
        return Result<InsuranceOverviewResponseModel>.Success(new InsuranceOverviewResponseModel
        {
            TotalPolicies = 10,
            TotalClaimed = 5,
            PendingClaims = 2,
            TotalExpiringPolicies = 3,
            TotalClaimPaid = 4
        });
    }
}
