using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.WebApi.Address;

namespace Models.WebApi.Customer;
public class CustomerResponseModel
{
    public string Individual_Type { get; set; }
    public string CourtesyTitle { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }

    public string Email { get; set; }
    public string Phone { get; set; }
    public string Gender { get; set; }
    public string PanNo { get; set; }
    public string MaritialStatus { get; set; }

    public string CitizenshipNo { get; set; }
    public string CitizenshipIssueDate { get; set; }
    public string VoterIdNumber { get; set; }
    public string LicenseNumber { get; set; }
    public string CitizenshipIssueDistrict { get; set; }

    public string PassportNumber { get; set; }
    public string PassportIssueDate { get; set; }
    public string PassportIssuePlace { get; set; }
    public string PassportExpiryDate { get; set; }

    public string DobAD { get; set; }
    public string DobBS { get; set; }
    public string Occupation { get; set; }

    public List<AddressResponseModel> Address { get; set; }
}
