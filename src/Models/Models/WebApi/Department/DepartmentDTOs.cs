using Models.Common;

namespace Models.BeemaEdgeApi.Department;

public class DepartmentResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}

public class CreateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class DepartmentListRequestModel : CommonPaginationRequestModel
{
    public string? TenantId { get; set; }
}
