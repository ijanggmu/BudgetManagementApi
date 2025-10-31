using Data.Context;
using Infrastructure.Common.UserProfile;
using Microsoft.EntityFrameworkCore;
using Models.BeemaEdgeApi.Customer.Dasboard;
using SharedKernel.Operation;
using SharedKernel.SystemEnum.Payment;

namespace Business.BeemaEdgeApi.Dashboard;

public class CustomerDashboardService(ApplicationDataContext dataContext, IUserProfileService userProfileService) : ICustomerDashboardService
{
    public async Task<Result<InsuranceOverviewResponseModel>> GetInsuranceOverviewAsync()
    {
        var userId = userProfileService.GetUserId();

        var today = DateTime.UtcNow.Date.AddDays(-30);

        var overview = await dataContext.Customers
            .Where(c => c.UserId == userId && !c.User.IsDeleted)
            .Select(c => new InsuranceOverviewResponseModel
            {
                UserDetails = new UserDetailResponseModel
                {
                    FullName = c.FullName,
                    KycStatus = c.KycStatus.ToString(),
                    KycRejectReason = c.KycRejectedReason

                },
                TotalPolicies = dataContext.PolicyDrafts
                    .Count(p => p.Status == PurchaseStatus.Acknowledged && p.Customer.UserId == userId && !p.Customer.User.IsDeleted),
                TotalClaimed = 0,              // TODO: Replace if needed
                PendingClaims = 0,            // TODO: Replace if needed
                TotalExpiringPolicies = dataContext.PolicyDrafts
                                .Where(x => x.Customer.UserId == userId && (x.Status == PurchaseStatus.Paid || x.Status == PurchaseStatus.Acknowledged) && x.ExpiryDate <= today)
                                .Count(),    // TODO: Replace if needed
                TotalClaimPaid = 0            // TODO: Replace if needed
            })
            .FirstOrDefaultAsync();

        return Result<InsuranceOverviewResponseModel>.Success(overview ?? new InsuranceOverviewResponseModel());
    }

}
