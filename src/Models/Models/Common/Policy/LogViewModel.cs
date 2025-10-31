using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class LogViewModel
    {
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string ForeignId { get; set; }
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string EditTable { get; set; }
        public string EditType { get; set; }
        public string ChangeJSON { get; set; }
        public string UpdatedBy { get; set; }
    }
}
