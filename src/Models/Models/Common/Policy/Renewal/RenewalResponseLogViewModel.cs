using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Renewal
{
    public class RenewalResponseLogViewModel
    {
        public int SN { get; set; }
        public string Id { get; set; }
        public string IssueDateOfCall { get; set; }
        public string StartTime { get; set; }
        public string ResponseTime { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseBody { get; set; }
        public string Status { get; set; }
    }
}
