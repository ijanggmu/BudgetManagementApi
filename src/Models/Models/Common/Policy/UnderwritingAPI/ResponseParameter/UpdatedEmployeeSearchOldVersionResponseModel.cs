using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.UnderwritingAPI.ResponseParameter
{
    public class UpdatedEmployeeSearchOldVersionResponseModel
    {
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public AddedEmployees Added { get; set; }
        public UpdatedEmployees Updated { get; set; }
        public DeletedEmployees Deleted { get; set; }
        public bool IsRenewal { get; set; }
    }


}
