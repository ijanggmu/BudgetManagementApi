using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class TravelUSDRateRequestModel
    {
        public string Issuer { get; set; }
        public string Group { get; set; }
        public string PlanType { get; set; }
        public int Period { get; set; }
        public bool IsIndividual { get; set; }
        public string DestinationIncludes { get; set; }
        public string MultipleEntries { get; set; }
        public int Age { get; set; }
    }

    public class HEOMIRateRequestModel
    {
        public string PlanType { get; set; }
        public int Period { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsAnnualTrip { get; set; }
        
    }
}
