using Models.Common.Policy.Policy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Draft
{
    public class DraftViewModel: CreatePolicyViewModel
    {
        public string PolicyNumber { get; set; }
        public int DraftNo { get; set; }
        public bool IsClassDetailJSONEmpty { get; set; }
        public bool IsManualEndorsement { get; set; }
    }
}
