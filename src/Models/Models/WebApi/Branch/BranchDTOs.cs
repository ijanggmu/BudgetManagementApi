namespace Models.BeemaEdgeApi.Branch;

public class BranchResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public int Ward { get; set; }
    public bool IsActive { get; set; } = true;
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}

public class CreateBranchDto
{
    public string BranchName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public int Ward { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateBranchDto
{
    public string BranchName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public int Ward { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ImportBranchDto
{
    public string BranchName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public int Ward { get; set; }
    public bool IsActive { get; set; } = true;
}

