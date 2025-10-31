using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
  public class AgricultureFileUploadViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string IdentificationCode { get; set; }
        public string ClassId { get; set; }
        public string Label { get; set; }
        public string Remarks { get; set; }
        public string Url { get; set; }
        public bool IsPhoto { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public IFormFile UrlFile { get; set; }
    }
}
