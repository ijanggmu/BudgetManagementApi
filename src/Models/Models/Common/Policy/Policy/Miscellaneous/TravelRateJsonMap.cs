using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{

    public class TravelRateRootobject
    {
        public TravelRateModel[] TravelRateList { get; set; }
    }

    public class TravelRateModel
    {
        //public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Issuer { get; set; }
        public string Group { get; set; }
        public string PlanType { get; set; }
        public string PeriodFrom { get; set; }
        public string PeriodTo { get; set; }
        public string IndividaulRate { get; set; }
        public string FamilyRate { get; set; }
        public string DestintionIncludes { get; set; }
        public string MultipleEntries { get; set; }
        public string AgeFrom { get; set; }
        public string AgeTo { get; set; }
    }

}
