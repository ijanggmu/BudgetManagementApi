# Authorization Refactoring - Unified PermissionAttribute

## Overview

Authorization checks have been moved from service layer to controller layer using filter attributes. **All authorization is now handled through a unified `PermissionAttribute`** that supports both permission-based and role-based authorization.

## Key Changes

### 1. Enhanced PermissionAttribute

**Before:** Separate `RoleAuthorizationFilterAttribute` for role checking, `PermissionAttribute` for permission checking.

**After:** Unified `PermissionAttribute` that supports:
- Permission-based authorization (existing)
- Role-based authorization (new)
- Combined authorization (role AND/OR permission)

```csharp
// Role-based
[RequireRoles("SuperAdmin", "Admin")]
// OR
[AdminOrSuperAdmin]  // Convenience attribute

// Permission-based (existing)
[Permission(MenuPermissionConstant.AdminManagementView)]

// Combined
[Permission(
    new[] { MenuPermissionConstant.AdminManagementCreate },
    new[] { "Admin" },
    requireBoth: true  // Must have both
)]
```

### 2. Authorization Moved to Controller Level

**Before:**
```csharp
// In service
public async Task<Result<List<AdminDto>>> GetAdminsAsync()
{
    var roleInfo = await _roleHelperService.GetUserRoleInfoAsync(userId);
    if (!roleInfo.IsSuperAdmin && !roleInfo.IsAdmin)
        return Result<List<AdminDto>>.Failed("Unauthorized");
    // ...
}
```

**After:**
```csharp
// In controller
[AdminOrSuperAdmin] // Authorization handled here
public class AdminController : BaseAdminApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAdminsAsync()
        => HandleResult(await _service.GetAdminsAsync());
}

// In service - no authorization check needed
public async Task<Result<List<AdminDto>>> GetAdminsAsync()
{
    // Only check role for query filtering logic
    var roleId = _userProfileService.GetRoleId();
    var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
    // ...
}
```

### 3. Services Only Check Roles for Query Logic

Services should **not** perform authorization checks. They only check roles when needed for query filtering logic (e.g., to determine if `IgnoreQueryFilters()` should be used).

**Pattern:**
```csharp
// In service
public async Task<Result<List<EntityDto>>> GetEntitiesAsync()
{
    // Authorization is handled by [AdminOrSuperAdmin] filter on controller
    // Only check role for query filtering logic
    var roleId = _userProfileService.GetRoleId();
    var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

    IQueryable<Entity> query = _db.Set<Entity>();
    
    if (isSuperAdmin)
        query = query.IgnoreQueryFilters();
    
    // ...
}
```

## Available Filter Attributes

All attributes are now part of `PermissionAttribute`:

### 1. AdminOrSuperAdmin
Requires Admin or SuperAdmin role:
```csharp
[AdminOrSuperAdmin]
public class AdminController : BaseAdminApiController
{
    // All endpoints require Admin or SuperAdmin
}
```

### 2. SuperAdminOnly
Requires SuperAdmin role only:
```csharp
[SuperAdminOnly]
[HttpGet("tenants")]
public async Task<IActionResult> GetAllTenantsAsync()
{
    // Only SuperAdmin can access
}
```

### 3. RequireRoles
Custom role combinations:
```csharp
[RequireRoles("Admin", "FoDo")]
[HttpGet("custom")]
public async Task<IActionResult> CustomEndpointAsync()
{
    // Requires Admin OR FoDo role
}
```

### 4. Permission (Existing)
Permission-based authorization:
```csharp
[Permission(MenuPermissionConstant.AdminManagementView)]
[HttpGet]
public async Task<IActionResult> GetAdminsAsync()
{
    // Requires AdminManagementView permission
}
```

## Benefits

1. **Separation of Concerns**: Authorization is handled at controller level, business logic in services
2. **Performance**: No database queries for role checking (uses JWT token claims)
3. **Consistency**: All authorization handled in one place (filter attributes)
4. **Maintainability**: Easier to see authorization requirements (attributes on controllers)
5. **Testability**: Services don't need to mock role checking for authorization

## Migration Guide

### Step 1: Add Filter Attribute to Controller

```csharp
// Before
public class AdminController : BaseAdminApiController
{
    // ...
}

// After - Use unified PermissionAttribute system
[AdminOrSuperAdmin] // Add this (convenience attribute)
// OR
[RequireRoles("Admin", "SuperAdmin")] // Custom roles
public class AdminController : BaseAdminApiController
{
    // ...
}
```

### Step 2: Remove Authorization Checks from Service

```csharp
// Before
public async Task<Result<List<AdminDto>>> GetAdminsAsync()
{
    var roleInfo = await _roleHelperService.GetUserRoleInfoAsync(userId);
    if (!roleInfo.IsSuperAdmin && !roleInfo.IsAdmin)
        return Result<List<AdminDto>>.Failed("Unauthorized");
    // ...
}

// After
public async Task<Result<List<AdminDto>>> GetAdminsAsync()
{
    // Authorization handled by filter - remove this check
    // Only check role for query logic if needed
    var roleId = _userProfileService.GetRoleId();
    var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
    // ...
}
```

### Step 3: Remove IUserRoleHelperService Dependency (if only used for authorization)

```csharp
// Before
public class AdminService(
    // ...
    IUserRoleHelperService roleHelperService)
{
    // ...
}

// After
public class AdminService(
    // ...
    // Remove roleHelperService if only used for authorization
)
{
    // ...
}
```

## Important Notes

1. **Role Checking in Services**: Only check roles when needed for query filtering logic (e.g., `IgnoreQueryFilters()`), not for authorization.

2. **GetRoleId()**: Uses JWT token claims, no database query needed. Returns comma-separated role names (e.g., "SuperAdmin,Admin").

3. **Filter Execution Order**: Filters execute before the action method, so authorization is checked before service logic runs.

4. **Error Responses**: Filter returns 403 Forbidden with proper error format if authorization fails.

## Example: Complete Refactoring

### Controller
```csharp
[ApiController]
[Route("api/v1/Admin")]
[AdminOrSuperAdmin] // Authorization handled here
public class AdminController : BaseAdminApiController
{
    private readonly IAdminService _adminService;

    [HttpGet]
    [Permission(MenuPermissionConstant.AdminManagementView)]
    public async Task<IActionResult> ListAsync([FromQuery] string? tenantId = null)
        => HandleResult(await _adminService.GetAdminsForAdminAsync(tenantId));

    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.AdminManagementView)]
    public async Task<IActionResult> GetAsync(string id)
        => HandleResult(await _adminService.GetAdminByIdAsync(id));
}
```

### Service
```csharp
public class AdminService : IAdminService
{
    private readonly ApplicationDataContext _db;
    private readonly IUserProfileService _userProfileService;

    public async Task<Result<List<AdminResponseDto>>> GetAdminsForAdminAsync(string? tenantId = null)
    {
        // Authorization handled by [AdminOrSuperAdmin] filter
        // Only check role for query filtering logic
        var roleId = _userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        IQueryable<Admin> query = _db.Admins.Include(a => a.User);

        if (isSuperAdmin)
        {
            if (!string.IsNullOrEmpty(tenantId))
                query = query.Where(a => a.TenantId == tenantId);
            query = query.IgnoreQueryFilters();
        }

        var admins = await query.ToListAsync();
        // ... mapping logic
    }
}
```

---

**Last Updated:** 2024  
**Status:** Implemented

