using System;

namespace Models.Common.Policy
{
    public class FiscalYearDetailViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string PolicyNumber { get; set; }
    }
}