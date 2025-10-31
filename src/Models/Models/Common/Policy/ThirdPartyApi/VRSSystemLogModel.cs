using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.ThirdPartyApi
{
    public class VRSSystemLogModel
    {
        public int SN { get; set; }
        public string Id { get; set; }
        public string PolicyNumber { get; set; }
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }

    }
}
