namespace Models.WebApi.Customer.Policy;
public class ContactKYCViewModel
{
    public string CitizenshipNo { get; set; }
    public string CitizenshipIssueDate { get; set; }
    public string VoterIdNumber { get; set; }
    public string LicenseNumber { get; set; }
    public string CitizenshipIssueDistrict { get; set; }
    public string PassportNumber { get; set; }
    public string PassportIssueDate { get; set; }
    public string PassportIssuePlace { get; set; }
    public string PassportExpiryDate { get; set; }
    public string NidNumber { get; set; }
    public DateTime DobAD { get; set; }
    public DateTime DobBS { get; set; }
    public string ClientClassification { get; set; }
    public List<string> OccupationJson { get; set; }
    public string IdentificationType { get; set; }
    public string IdentificationNo { get; set; }
}
