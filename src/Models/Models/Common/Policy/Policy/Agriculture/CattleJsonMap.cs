using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class CattleJsonMap
    {
        public List<CattleTypes> CattleTypes { get; set; }
    }
    public class CattleTypes
    {
        public string ClassName { get; set; }
        public string ClassAlias { get; set; }
        public string Group { get; set; }
        public string NepaliClassName { get; set; }
    }
}
