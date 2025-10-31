using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class ClassDetailViewModel
    {
        public string Id { get; set; }
        public string PolicyNo { get; set; }
        public string DocumentNo { get; set; }
        public string PolicyIssuanceId { get; set; }
        public string ClassId { get; set; }
        public string PortfolioId { get; set; }
        public string ClassDetailJSON { get; set; }
        public string CalulationDetailJSON { get;  set; }
        public bool HasMultipleAssets { get; set; }
        public bool IsIssued { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
