using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.Agriculture
{
    public class CropJsonMap
    {
        public List<CropTypes> CropTypes { get; set; }
    }
    public class CropTypes
    {
        public string ClassName { get; set; }
        public string ClassAlias { get; set; }
        public string Group { get; set; }
        public string NepaliClassName { get; set; }
    }
}
