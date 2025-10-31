using Models.Common.Policy.Endorsement;
using Models.Common.Policy.Policy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Renewal
{
    public class RenewalViewModel: CreatePolicyViewModel
    {
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public int DraftNo { get; set; }
        public bool IsClassDetailJSONEmpty { get; set; }
        public bool IsManualEndorsement { get; set; }
        public string GateWay { get; set; }
        public string TransactionId { get; set; }
        public string NetPremium { get; set; }
    }
}
