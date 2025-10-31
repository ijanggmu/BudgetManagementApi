using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class CerealJsonMap
    {
        public List<CerealTypes> CerealTypes { get; set; }
    }
    public class CerealTypes
    {
        public string ClassName { get; set; }
        public string ClassAlias { get; set; }
        public string Group { get; set; }
        public string NepaliClassName { get; set; }
    }
}