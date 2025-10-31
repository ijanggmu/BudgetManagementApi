using Microsoft.AspNetCore.Mvc;
using SharedKernel.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Models.BeemaEdgeApi.Bank;
public class BankRequestViewModel
{
    public int Id { get; set; }
    public string BankId { get; set; }
    public string Name { get; set; }
}
public class BankResponseViewModel
{
    public string Id { get; set; }
    public string BankName { get; set; }
    public string BankType { get; set; }
}
public class GetAgentByCodeRequestModel
{
    public int Code { get; set; }
}
public class AgentResponseModel
{
    public string Id { get; set; }
    public int Sn { get; set; }
    public string CourtesyTitle { get; set; }
    [StringLength(20, ErrorMessage = "Must be at most 20 characters long")]
    [RequiredIf("Type", "Individual", ErrorMessage = "First Name is required.")]
    [DisplayName("First Name")]
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    [StringLength(20, ErrorMessage = "Must be at most 20 characters long")]
    [RequiredIf("Type", "Individual", ErrorMessage = "Last Name is required.")]
    [DisplayName("Last Name")]
    public string LastName { get; set; }
    [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = "Phone number should be a valid number of 7 to 15 characters.")]
    [Required(ErrorMessage = "Phone Number is required")]
    public string PhoneNumber { get; set; }
    [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = "Phone number should be a valid number of 7 to 15 characters.")]
    public string PhoneNumber2 { get; set; }
    [RegularExpression("^[0-9+-]{7,15}$", ErrorMessage = "Phone number should be a valid number of 7 to 15 characters.")]
    public string Telephone { get; set; }
    [Required(ErrorMessage = "Province is required")]
    public string Province { get; set; }
    [Required(ErrorMessage = "District is required")]
    public string District { get; set; }
    [Required(ErrorMessage = "Municipality is required")]
    public string Municipality { get; set; }
    public int Ward { get; set; }
    public string StreetAddress { get; set; }
    //[Required(ErrorMessage = "Citizenship Number is required")]
    public string CitizenshipNumber { get; set; }
    //[Required(ErrorMessage = "Issue Date is required")]
    public string DateOfIssue { get; set; }
    //[Required(ErrorMessage = "Issue District is required")]
    public string IssueDistrict { get; set; }
    //[Required(ErrorMessage = "Pan Number is required")]
    public string PAN { get; set; }
    public string Bank { get; set; }
    public string Branch { get; set; }
    public string AccountNumber { get; set; }
    //[Required(ErrorMessage = "DO/FO  is required")]
    public string DOFO { get; set; }
    public bool Status { get; set; } = true;
    public string EnrollmentDate { get; set; }
    public string ExpiryDate { get; set; }
    public int AgentCode { get; set; }
    [Required(ErrorMessage = "Agent Type  is required")]
    public string Type { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public string CreatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public string AddressInfo { get; set; }
    public string FullName { get; set; }
    public string FullDetail { get; set; }
    public string CitizenshipDetails { get; set; }
    public string PhoneInfo { get; set; }
    public string BankInfo { get; set; }
    [Required(ErrorMessage = "Start Date is required")]
    public DateTime StartDate { get; set; }
    [RequiredIf("Type", "Corporate", ErrorMessage = "Agency Name is required.")]
    public string AgencyName { get; set; }

    public string DisplayName { get; set; }
    public string FullDetailAgency { get; set; }
    [StringLength(100)]
    public string EnrollmentHistory { get; set; }
    public string License { get; set; }
    public string Email { get; set; }
    public string Alias { get; set; }
    public string UserType { get; set; }
    public bool IsAgentDigitalVendor { get; set; }
}
