namespace Models.BeemaEdgeApi.Customer.Dasboard;

public class InsuranceOverviewResponseModel
{
    public int TotalPolicies { get; set; }
    public int TotalClaimed { get; set; }
    public int PendingClaims { get; set; }
    public int TotalExpiringPolicies{ get; set; }
    public int TotalClaimPaid { get; set; }
    public UserDetailResponseModel UserDetails { get; set; }
}
public class UserDetailResponseModel
{
    public string FullName { get; set; }
    public string KycStatus { get; set; }
    public string KycRejectReason { get; set; }
}

