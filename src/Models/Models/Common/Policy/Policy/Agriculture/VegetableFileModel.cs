using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class VegetableFileModel
    {

        public IFormFile VegetableMaterialsFile { get; set; }
        public string PolicyNumber { get; set; }
        public bool IsOldEndorsement { get; set; }
    }
}
