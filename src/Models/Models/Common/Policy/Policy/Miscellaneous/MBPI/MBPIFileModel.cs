using Microsoft.AspNetCore.Http;

namespace Models.Common.Policy.Policy.Miscellaneous.MBPI
{
    public class MBPIFileModel
    {
        public IFormFile MBPIFile { get; set; }
        public string PolicyNumber { get; set; }
        public string ImageUrl { get; set; }
        public bool IsOldEndorsement { get; set; }
        public bool IsInternal { get; set; }
        public string PartyName { get; set; }
        public string PartyId { get; set; }
    }
}