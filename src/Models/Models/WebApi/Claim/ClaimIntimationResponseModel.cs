using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.WebApi.Claim;
public class ClaimIntimationResponseModel
{
    public MetadataResponse Metadata { get; set; }
    public ClaimIntimationDataResponse Data { get; set; }
    public string Error { get; set; }
}
public class MetadataResponse
{
    public string Copyright { get; set; }
    public string Email { get; set; }
    public ApiResponse Api { get; set; }
    public object Pagination { get; set; }
}

public class ApiResponse
{
    public string Version { get; set; }
}

public class ClaimIntimationDataResponse
{
    public string IntimationNumber { get; set; }
    public string Status { get; set; }
    public string ClaimYear { get; set; }
    public string UwFiscalYear { get; set; }
    public DateTime IntimationReceivedDate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
}
