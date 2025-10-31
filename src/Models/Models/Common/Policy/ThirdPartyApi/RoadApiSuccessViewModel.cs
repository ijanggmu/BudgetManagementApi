using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.ThirdPartyApi
{
    public class RoadApiSuccessViewModel
    {
        public string insurance_company_name { get; set; }
        public string engine_number { get; set; }
        public string chassis_number { get; set; }
        public string vehicle_number { get; set; }
        public string vehicle_number_np { get; set; }
        public string receipt_no { get; set; }
        public string invoice_no { get; set; }
        public string policy_no { get; set; }
        public string effective_date { get; set; }
        public string expiry_date { get; set; }


    }
}
