using Microsoft.AspNetCore.Http;

namespace Models.Common.Policy.Policy.GPA
{
    public class InsuredPersonsFileModel
    {
        public IFormFile InsuredPersonsFile { get; set; }
        public string PolicyNumber { get; set; }
        public bool IsOldEndorsement { get; set; }
        public bool  IsNewPolicyFormat { get; set; }
    }
}
