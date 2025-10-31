using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.ThirdPartyApi
{
    public class MotorDetailModel
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string VehicleType { get; set; }
        public string RegistrationType { get; set; }
        public string RegistrationNo { get; set; }
        public string RegistrationNoNp { get; set; }
        public string ChassisNumber { get; set; }
        public string EngineNumber { get; set; }
    }
}
