using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy
{
    public class TagDetailToViewVm
    {
        public string Id { get; set; }
        public int? DraftNumber { get; set; }
        public string PolicyNumber { get; set; }
        public List<string> TagNameList { get; set; }
        public List<string> TagIdList { get; set; }
    }
}
