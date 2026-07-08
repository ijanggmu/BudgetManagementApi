namespace Models.WebApi.TenantDTOs;

public record AdminResponseDto(
    string Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    string Username,
    string UserId,
    string? TenantId,
    string TenantName,
    List<string> Roles,
    List<string> RoleDisplayNames,
    bool IsDisabled,
    bool EmailConfirmed,
    DateTime CreatedOn,
    string? DepartmentId,
    string? DepartmentName
);

public record CreateAdminDto(
    string FullName,
    string Email,
    string Username,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    List<string> Roles,
    string? TenantId,
    string? DepartmentId
);

public record UpdateAdminDto(
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsDisabled,
    List<string> Roles,
    string? DepartmentId
);

