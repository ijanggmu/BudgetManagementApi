using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy.PolicyGroup
{
    public class PolicyGroupEmployeeExportModel
    {
        public List<UnMatchedData> GpaList { get; set; }
        public List<UnMatchedData> HipList { get; set; }
        public List<UnMatchedData> MedList { get; set; }
    }
}
