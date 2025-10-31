namespace Models.Common.Policy.Policy
{
    public interface ILocation
    {
        string Name { get; set; }
        string[] Locations { get; set; }
    }
}