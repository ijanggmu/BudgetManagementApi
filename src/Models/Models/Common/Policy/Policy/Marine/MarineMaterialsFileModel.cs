using Microsoft.AspNetCore.Http;

namespace Models.Common.Policy.Policy.Marine
{
    public class MarineMaterialsFileModel
    {
        public IFormFile MarineMaterialsFile { get; set; }
        public string PolicyNumber { get; set; }
        public string ImageUrl { get; set; }
        public bool IsOldEndorsement { get; set; }
        public bool IsInternal { get; set; }
        public string PartyName { get; set; }
        public string PartyId { get; set; }
    }
}
