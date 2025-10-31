using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.WebApi.Claim
{
    public class CoreClaimIntimationRequestModel
    {
        public string Id { get; set; }
        public string PolicyNumber { get; set; }
        public string DocumentNumber { get; set; }
        public decimal ClaimAmount { get; set; }
        public string Remarks { get; set; }
        public string CancelledRemarks { get; set; }
        public DateTime OccuranceDateTime { get; set; }
        public string OccuranceDateTimeNepali { get; set; }
        public string NatureOfLoss { get; set; }
        public DamageLocationPayload DamageLocation { get; set; }
        public ClaimIntimationE2ETravelModel ClaimIntimationE2ETravelModel { get; set; }
        public ClaimIntimationE2EPHIModel ClaimIntimationE2EPHI { get; set; }
        public string InsuredName { get; set; }
        public string InsuredPhoneNumber { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int SumInsured { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ClassId { get; set; }
    }
    public class ClaimIntimationE2ETravelModel
    {
        public decimal ClaimAmountUSD { get; set; }
        public string OccuranceCountry { get; set; }
        public string OccuranceCity { get; set; }
        public string ClaimFor { get; set; }
        public string ClaimantName { get; set; }
    }
    public class ClaimIntimationE2EPHIModel
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Relation { get; set; }
    }
}
