using System;

namespace Models.WebApi.Noticeboard;

/// <summary>
/// DTO for creating a new noticeboard entry
/// </summary>
public class CreateNoticeboardDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPinned { get; set; } = false;
    public int Priority { get; set; } = 0;
}

/// <summary>
/// DTO for updating an existing noticeboard entry
/// </summary>
public class UpdateNoticeboardDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPinned { get; set; } = false;
    public int Priority { get; set; } = 0;
}

/// <summary>
/// DTO for noticeboard response
/// </summary>
public class NoticeboardResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsPinned { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedBy { get; set; }
}
