namespace Models.Common.Policy.Policy
{
    public class Location:ILocation
    {
        public string Name { get; set; }
        public string[] Locations { get; set; }
    }
}