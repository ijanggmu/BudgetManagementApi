using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Common.Policy.Configuration
{
    public class FiscalYearConfigViewModel
    {
        public string Id { get; set; }
        public int SN { get; set; }
        [Required]
        public string FiscalYear { get; set; }    
        public string FiscalYearNepali { get; set; }
        [Required]
        public DateTime StartDate { get; set; }        
        [Required]
        public DateTime EndDate { get; set; }     
        public string YearOfOperation { get; set; }     
        [Required]
        public string ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public List<AccountingPeriodViewModel> MonthWiseConfigViewModel { get; set; } = new List<AccountingPeriodViewModel>();
    }
}
