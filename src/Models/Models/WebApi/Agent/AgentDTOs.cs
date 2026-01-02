namespace Models.BeemaEdgeApi.Fodo;

public class RegisterFodoRequestModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class VerifyFodoOtpRequestModel
{
    public string Username { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}

public class FodoResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? DesignationId { get; set; }
    public string? DesignationTitle { get; set; }
    public string? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? PermanentProvince { get; set; }
    public string? PermanentDistrict { get; set; }
    public string? PermanentMunicipality { get; set; }
    public int? PermanentWard { get; set; }
    public string? TemporaryProvince { get; set; }
    public string? TemporaryDistrict { get; set; }
    public string? TemporaryMunicipality { get; set; }
    public int? TemporaryWard { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDisabled { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}

public class CreateFodoDto
{
    public string FullName { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string? DesignationId { get; set; }
    public string? BranchId { get; set; }
    public string? PermanentProvince { get; set; }
    public string? PermanentDistrict { get; set; }
    public string? PermanentMunicipality { get; set; }
    public int? PermanentWard { get; set; }
    public string? TemporaryProvince { get; set; }
    public string? TemporaryDistrict { get; set; }
    public string? TemporaryMunicipality { get; set; }
    public int? TemporaryWard { get; set; }
    public bool IsActive { get; set; } = true;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public List<string> Roles { get; set; }
}

public class UpdateFodoDto
{
    public string FullName { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string? DesignationId { get; set; }
    public string? BranchId { get; set; }
    public string? PermanentProvince { get; set; }
    public string? PermanentDistrict { get; set; }
    public string? PermanentMunicipality { get; set; }
    public int? PermanentWard { get; set; }
    public string? TemporaryProvince { get; set; }
    public string? TemporaryDistrict { get; set; }
    public string? TemporaryMunicipality { get; set; }
    public int? TemporaryWard { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ImportFodoDto
{
    public string FullName { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public int CountryId { get; set; } = 1; // Default country ID
    public string? DesignationTitle { get; set; } // Lookup by title instead of ID
    public string? BranchCode { get; set; } // Lookup by code instead of ID
    public string? PermanentProvince { get; set; }
    public string? PermanentDistrict { get; set; }
    public string? PermanentMunicipality { get; set; }
    public int? PermanentWard { get; set; }
    public string? TemporaryProvince { get; set; }
    public string? TemporaryDistrict { get; set; }
    public string? TemporaryMunicipality { get; set; }
    public int? TemporaryWard { get; set; }
    public bool IsActive { get; set; } = true;
    public string Password { get; set; } = string.Empty; // Default password or generate
}

