using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class PrintTrackViewModel
    {
        public string Id { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime PrintDateime { get; set; }
        public string PrintedBy { get; set; }
        public string Remarks { get; set; }
        public string PolicyIssuanseId { get; set; }
    }
}
