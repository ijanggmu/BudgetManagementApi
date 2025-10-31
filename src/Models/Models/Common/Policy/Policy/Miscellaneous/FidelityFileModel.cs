using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Miscellaneous
{
    public class FidelityFileModel
    {
        public IFormFile FidelityMaterialsFile { get; set; }
        public string PolicyNumber { get; set; }
        public bool IsOldEndorsement { get; set; }
    }
}
