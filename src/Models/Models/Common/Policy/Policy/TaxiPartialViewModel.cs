using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class TaxiPartialViewModel
    {
        public CommonMotorModel CommonProperties { get; set; }
        public CommonRSMDTModel CommonRSMDTModel { get; set; }
    }
}
