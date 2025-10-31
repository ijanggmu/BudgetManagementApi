using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.ResponseParameter
{
    public class EmployeeSearchResponseModel
    {
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string FiscalYear { get; set; }
        public bool PolicySyncStatus { get; set; }

        public EmployeeDetailsFresh Employee { get; set; }
    }

    public class EmployeeDetailsFresh
    {
        public List<EmployeeInfo> Details { get; set; }
        
    }
    public class EmployeeInfo
    {
        public string EmployeeName { get; set; }
        public string MasterEmpName { get; set; }
        public string MasterEmpId { get; set; }
        public string EmployeeID { get; set; }
        public string Age { get; set; }
        public string Plan { get; set; }
        public string RiskSelected { get; set; }
       
        public decimal SumInsured { get; set; }
        public string Profession { get; set; }
        public string ProfessionRelatedRisk { get; set; }
        public bool AdditionalRisk { get; set; }
        public string AdditionalRiskDetail { get; set; }
        public decimal? AdditionalMedicalSumInsured { get; set; }
        public string Designation { get; set; }
        public FamilyDetail Family { get; set; }
    }
    public class FamilyDetail
    {
        public string Parent1Name { get; set; }
        public string Parent1Age { get; set; }
        public string Parent2Name { get; set; }
        public string Parent2Age { get; set; }
        public string SpouseName { get; set; }
        public string SpouseAge { get; set; }
        public string Child1Name { get; set; }
        public string Child1Age { get; set; }
        public string Child2Name { get; set; }
        public string Child2Age { get; set; }
        public string Child3Name { get; set; }
        public string Child3Age { get; set; }
        public string Child4Name { get; set; }
        public string Child4Age { get; set; }
        public string Child5Name { get; set; }
        public string Child5Age { get; set; }
        public string Child6Name { get; set; }
        public string Child6Age { get; set; }
        public string Child7Name { get; set; }
        public string Child7Age { get; set; }
        public string Child8Name { get; set; }
        public string Child8Age { get; set; }
        public string Child9Name { get; set; }
        public string Child9Age { get; set; }
        public string Child10Name { get; set; }
        public string Child10Age { get; set; }

    }
}
