using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.ResponseParameter
{
    public class UpdatedEmployeesSearchResponseModel
    {
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string FiscalYear { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public AddedEmployees Added { get; set; }
        public UpdatedEmployees Updated { get; set; }
        public DeletedEmployees Deleted { get; set; }
        public bool PolicySyncStatus { get; set; }

        public bool IsRenewal { get; set; }
    }
    public class AddedEmployees
    {
        public List<UpdatedEmployeeDetails> Details { get; set; }
    }
    public class UpdatedEmployees
    {
        public List<UpdatedEmployeeDetails> Details { get; set; }
    }
    public class DeletedEmployees
    {
        public List<UpdatedEmployeeDetails> Details { get; set; }
    }
    public class UpdatedEmployeeDetails
    {
        
        public string EmployeeName { get; set; }
        public string EmployeeID { get; set; }
        public string MasterEmpName { get; set; }
        public string MasterEmpId { get; set; }
        public string Age { get; set; }
        public string RiskSelected { get; set; }
        public string Plan { get; set; }
        
        public decimal SumInsured { get; set; }
        public string Profession { get; set; }
        public string ProfessionRelatedRisk { get; set; }
        public bool AdditionalRisk { get; set; }
        public string AdditionalRiskDetail { get; set; }
        public decimal? AdditionalMedicalSumInsured { get; set; }
        public string Designation { get; set; }

        public FamilyDetail Family { get; set; }
    }

}
