namespace Models.BeemaEdgeApi.Customer.CustomerIdentity;

public class UserTotpBackUpCodeResponseModel
{
    public string CodeHash { get; set; }
    public bool IsUsed { get; set; }
    public string CreatedOn { get; set; }
    public string UsedOn { get; set; }
}
public class AvailabilityResponseModel
{
    public bool IsTaken { get; set; }
}
