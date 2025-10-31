using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
   public class AgricultureTagListViewModel
    {
        public string Id { get; set; }
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string IdentificationCode { get; set; }
        public string ClassId { get; set; }
        public string Remarks { get; set; }
        public string PhotoIncludingTagNoUrl { get; set; }
        public string FrontViewUrl { get; set; }
        public string RightSideViewUrl { get; set; }
        public string LeftSideViewUrl { get; set; }
        public string BackViewUrl { get; set; }
        public string OverallViewUrl { get; set; }
        public string TopViewUrl { get; set; }

        public string VideoUrl { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public IFormFile PhotoIncludingTagNoFile { get; set; }
        public IFormFile FrontViewFile { get; set; }
        public IFormFile RightSideViewFile { get; set; }
        public IFormFile LeftSideViewFile { get; set; }
        public IFormFile BackViewFile { get; set; }
        public IFormFile OverallViewFile { get; set; }
        public IFormFile TopViewFile { get; set; }
        public IFormFile VideoFile { get; set; }
        public bool ValidateUpload { get; set; }
    }
}
