namespace Models.BeemaEdgeApi.Designation;

public class DesignationResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}

public class CreateDesignationDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateDesignationDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ImportDesignationDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

