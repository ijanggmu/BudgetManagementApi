using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class PolicyTimeSpendViewModel
    {
        public int Sn { get; set; }
        public int DraftId { get; set; }
        public string PolicyAlias { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public string StaffBranch { get; set; }
        public DateTime PolicyCreatedDate { get; set; }
        public string TimeSpend { get; set; }
        public string CreatedBy { get; set; }
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string Branch { get; set; }
        public bool IsDeleted { get; set; }
        public string Portfolio { get; set; }
    }

    public class TimeLogStaffList
    {
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
    }
}
