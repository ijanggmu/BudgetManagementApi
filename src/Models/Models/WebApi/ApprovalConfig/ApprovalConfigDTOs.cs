using Models.Common;

namespace Models.BeemaEdgeApi.ApprovalConfig;

public class ApprovalConfigStepDto
{
    public string? Id { get; set; }
    public int StepOrder { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public string ApproverRoleId { get; set; } = string.Empty;
    public string? ApproverRoleName { get; set; }
    public bool IsMandatory { get; set; }
}

public class ApprovalConfigResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public List<ApprovalConfigStepDto> Steps { get; set; } = new();
    public DateTime CreatedOn { get; set; }
}

public class ApprovalConfigListRequestModel : CommonPaginationRequestModel
{
    public string? TenantId { get; set; }
    public string? DepartmentId { get; set; }
}

public class CreateApprovalConfigDto
{
    /// <summary>
    /// Optional. When null or empty, the config is treated as a default
    /// tenant-wide approval configuration (applies to all departments
    /// that don't have an explicit override).
    /// </summary>
    public string? DepartmentId { get; set; }
    public List<ApprovalConfigStepInputDto> Steps { get; set; } = new();
}

public class ApprovalConfigStepInputDto
{
    public int StepOrder { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public string ApproverRoleId { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
}

public class UpdateApprovalConfigDto
{
    public string? DepartmentId { get; set; }
    public List<ApprovalConfigStepInputDto>? Steps { get; set; }
}

public class ApprovalConfigImportResultDto
{
    public int CreatedCount { get; set; }
    public int UpdatedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>Minimal role info for approval config approver dropdown.</summary>
public class ApproverRoleItemDto
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string RoleDisplayName { get; set; } = string.Empty;
}
