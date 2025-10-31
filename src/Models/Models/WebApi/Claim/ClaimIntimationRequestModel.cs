using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.WebApi.Claim;
public class ClaimIntimationRequestModel
{
    public string Id { get; set; }
    public string PolicyNumber { get; set; }
    public string DocumentNumber { get; set; }
    public string insuredName { get; set; }
    public string insuredPhoneNumber { get; set; }
    public decimal ClaimAmount { get; set; }
    public string Remarks { get; set; }
    public string CancelledRemarks { get; set; }
    public DateTime OccuranceDateTime { get; set; }
    public string OccuranceDateTimeNepali { get; set; }
    public string Class { get; set; }
    public ClaimIntimationTravelModel ClaimIntimationE2ETravelModel { get; set; }
}

public class DamageLocationPayload
{
    public string Province { get; set; }
    public string District { get; set; }
    public string Municipality { get; set; }
    public int Ward { get; set; }
    public string StreetAddress { get; set; }
}
public class ClaimIntimationTravelModel
{
    public decimal ClaimAmountUSD { get; set; }
    public string OccuranceCountry { get; set; }
}


