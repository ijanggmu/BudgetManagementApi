using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class ElectricCommercialVehiclePartialViewModel
    {
        public CommonMotorModel CommonProperties { get; set; }
        public CommonRSMDTModel CommonRSMDTModel { get; set; }
        public string MasterPolicyNumber { get; set; }
        public bool Transportation { get; set; }
        public bool Replacement { get; set; }
        public bool Depreciation { get; set; }
        public decimal TransportationRate { get; set; }
        public decimal TransportationAmount { get; set; }
        public decimal ReplacementRate { get; set; }
        public decimal ReplacementAmount { get; set; }
        public decimal DepreciationRate { get; set; }
        public decimal DepreciationAmount { get; set; }
        public bool HasSmartPolicy { get; set; }
    }
}
