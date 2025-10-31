using Microsoft.AspNetCore.Http;

namespace Models.Common.Policy.Policy.Fire
{
    public class HouseHoldContentModel
    {
        public IFormFile HouseholdContentData { get; set; }
        public int FileIndex { get; set; }
    }
}
