using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class UserAccessViewModel
    {
        public string Id { get; set; }
        public int Sn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedDate { get; set; } 
        public string UpdatedDate { get; set; }
        public bool IsConfigured { get; set; }
        public bool IsDeleted { get; set; }
        public string UserId { get; set; }
        public string ClassCode { get; set; }
    }
}
