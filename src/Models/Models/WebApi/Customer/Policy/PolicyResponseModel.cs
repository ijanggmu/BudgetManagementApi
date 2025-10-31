namespace Models.BeemaEdgeApi.Customer.Policy;
public class PolicyResponseModel
{
    public string PolicyId { get; set; }
    public string PolicyName { get; set; }
    public string PolicyHolder { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
