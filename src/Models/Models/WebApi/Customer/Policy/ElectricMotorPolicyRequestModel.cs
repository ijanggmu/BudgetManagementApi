namespace Models.BeemaEdgeApi.Customer.Policy;
internal class ElectricMotorPolicyRequestModel
{
    public ElectricMotorRequestModel ElectricMotorDetails { get; set; }
    public string PortfolioAlias { get; set; }
    public string BranchCode { get; set; }
    public string PortfolioId { get; set; }
    public Guid PartyId { get; set; }
    public string TypeOfParty { get; set; }
    public string PortfolioParent { get; set; }
    public string Class { get; set; }
    public Guid User { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime ProposedDate { get; set; }
    public List<string> ShareCompanyLists { get; set; }
    public bool IsPolicyFromThirdParty { get; set; }
    public string BancassuranceBankName { get; set; }
    public string BancassuranceBankBranch { get; set; }
}
