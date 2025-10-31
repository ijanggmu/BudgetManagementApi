using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Medical
{
    public class HIPInsuredPersonFileModel
    {
        public IFormFile InsuredPersonsFile { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyId { get; set; }
    }
}
