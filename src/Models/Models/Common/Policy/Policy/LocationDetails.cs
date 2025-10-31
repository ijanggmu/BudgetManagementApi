using System.Collections.Generic;

namespace Models.Common.Policy.Policy
{
    public class LocationDetails
    {
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string Portfolio { get; set; }
        public string InsuredPerson { get; set; }
        public IEnumerable<Location> Location { get; set; }
    }
}