using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class BirdJsonMap
    {
        public List<BirdTypes> BirdTypes { get; set; }
    }
    public class BirdTypes
    {
        public string ClassName { get; set; }
        public string ClassAlias { get; set; }
        public string Group { get; set; }
        public string NepaliClassName { get; set; }
    }
}
