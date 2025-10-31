using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class PassengerCarryingVehiclePartialViewModel
    {
        public CommonMotorModel CommonProperties { get; set; }
        public CommonRSMDTModel CommonRSMDTModel { get; set; }
        public bool PersonalAccidentForConductor { get; set; }
        public bool IsPersonalAccidentForHelper { get; set; }
        public bool IsRiotStrikeAndTerrorismForHelper { get; set; }
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

        [RegularExpression("[0-9,]{0,23}[.]?([0-9]{1,2})?", ErrorMessage = ("Must be a number with 2 decimal points and maximum length of 25"))]
        public string SumInsuredAmountForHelper { get; set; }
        public bool OwnUseForPrivateTourismSchool { get; set; }
    }
}
