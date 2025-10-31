using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.ResponseParameter
{
    public class EmployeeSearchOldVersionresponseModel
    {
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string EmployeeName { get; set; }
        public string Age { get; set; }
        public string Plan { get; set; }
        public string RiskSelected { get; set; }
        public string EmployeeID { get; set; }
        public decimal SumInsured { get; set; }
        public FamilyDetail Family { get; set; }
    }
}
