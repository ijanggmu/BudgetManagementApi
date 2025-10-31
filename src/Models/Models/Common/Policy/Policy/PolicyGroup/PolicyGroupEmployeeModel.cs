using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.PolicyGroup
{
    public class PolicyGroupEmployeeModel
    {
        public string MasterEmployeeId { get; set; }
        public string MasterEmployeeName { get; set; }
        public string MasterPortfolio { get; set; }
        public string EmployeeNameGPA { get; set; }
        public string EmployeeIdGPA { get; set; }
        public string GPAPolicyNumber { get; set; }
        public string EmployeeNameHIP { get; set; }
        public string EmployeeIdHIP { get; set; }
        public string HIPPolicyNumber { get; set; }
        public string EmployeeNameMED { get; set; }
        public string EmployeeIdMED { get; set; }
        public string MEDPolicyNumber { get; set; }
        public string FiscalYear { get; set; }
    }
}
