using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class AgricultureClassTagViewModel
    {
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public int NumberOfTags { get; set; }
        public int NumberOfRemovedTags { get; set; }
    }
}
