using Models.WebApi.Address;
using Models.WebApi.Customer.Policy;

namespace Models.BeemaEdgeApi.Customer.Policy;

public class ContactViewModel
{
    public string Individual_Type { get; set; }
    public string CourtesyTitle { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string FirstName_Np { get; set; }
    public string MiddleName_Np { get; set; }
    public string LastName_Np { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Mobile { get; set; }
    public string Gender { get; set; }
    public string PanNo { get; set; }
    public string TmStreetAddress { get; set; }
    public int? TmWard { get; set; }
    public string TmMunicipality { get; set; }
    public string TmDistrict { get; set; }
    public string TmProvince { get; set; }
    public string PaStreetAddress { get; set; }
    public int? PaWard { get; set; }
    public string PaMunicipality { get; set; }
    public string PaDistrict { get; set; }
    public string PaProvince { get; set; }
    public AddressResponseModel Address { get; set; }
    public string MaritialStatus { get; set; }
    public ContactKYCViewModel ContactKYCViewModel { get; set; }
    public string DIANumber { get; set; }
    public string Id { get; set; }
    public string PartyCode { get; set; }
    public string IdentificationType { get; set; }
    public string IdentificationNo { get; set; }
    public bool IsFromCustomerPortal { get; set; }


}

