using System.ComponentModel.DataAnnotations;

namespace Models.Common;

public abstract class PaginationRequestModel
{
    private int _pageSize = 10;

    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = (value > 0) ? value : 1;
    }
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value == 0 || value > 600) ? _pageSize : value;
    }
}
public class CommonPaginationRequestModel : PaginationRequestModel
{
    public string Query { get; set; }
    public string Filters { get; set; }
    public string Sorts { get; set; }

    /// <summary>Optional tenant scope (e.g. SuperAdmin user-management / roles list). Sent in POST body as tenantId.</summary>
    public string TenantId { get; set; }
}
public class RejectKycRequest
{
    [Required]
    public string Reason { get; set; }
}
