using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class AccountingPeriodViewModel
    {
        public string FiscalYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Month { get; set; }
        public string MonthInNepali { get; set; }
        public string CreatedBy { get; set; }
        public string FiscalYearNepali { get; set; }
    }
}
