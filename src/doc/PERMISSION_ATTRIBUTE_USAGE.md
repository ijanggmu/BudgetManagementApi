# PermissionAttribute Usage Guide

## Overview

The `PermissionAttribute` has been enhanced to support both **permission-based** and **role-based** authorization, consolidating all authorization logic into a single, flexible attribute.

## Usage Patterns

### 1. Permission-Based Authorization (Existing)

Check if user has specific permissions:

```csharp
[HttpGet]
[Permission(MenuPermissionConstant.AdminManagementView)]
public async Task<IActionResult> GetAdminsAsync()
{
    // User must have AdminManagementView permission
}
```

### 2. Role-Based Authorization (New)

Check if user has specific roles:

```csharp
// Option A: Using RequireRolesAttribute (recommended)
[HttpGet]
[RequireRoles("SuperAdmin", "Admin")]
public async Task<IActionResult> GetAdminsAsync()
{
    // User must have SuperAdmin OR Admin role
}

// Option B: Using convenience attributes
[AdminOrSuperAdmin] // Requires Admin or SuperAdmin
public class AdminController : BaseAdminApiController
{
    // All endpoints require Admin or SuperAdmin
}

[SuperAdminOnly] // Requires SuperAdmin only
[HttpGet("tenants")]
public async Task<IActionResult> GetAllTenantsAsync()
{
    // Only SuperAdmin can access
}
```

### 3. Combined Authorization (New)

Require both role AND permission:

```csharp
[HttpPost]
[Permission(
    new[] { MenuPermissionConstant.AdminManagementCreate },  // Permissions
    new[] { "Admin", "SuperAdmin" },                         // Roles
    requireBoth: true                                         // Must have both
)]
public async Task<IActionResult> CreateAdminAsync([FromBody] CreateAdminDto dto)
{
    // User must have AdminManagementCreate permission AND (Admin OR SuperAdmin) role
}
```

### 4. Role OR Permission (New)

Require role OR permission (at least one):

```csharp
[HttpGet]
[Permission(
    new[] { MenuPermissionConstant.AdminManagementView },  // Permissions
    new[] { "SuperAdmin" },                                 // Roles
    requireBoth: false                                      // Must have at least one
)]
public async Task<IActionResult> GetAdminsAsync()
{
    // User must have AdminManagementView permission OR SuperAdmin role
}
```

## Available Convenience Attributes

### AdminOrSuperAdmin
Requires Admin or SuperAdmin role:
```csharp
[AdminOrSuperAdmin]
public class AdminController : BaseAdminApiController
{
    // All endpoints require Admin or SuperAdmin
}
```

### SuperAdminOnly
Requires SuperAdmin role only:
```csharp
[SuperAdminOnly]
[HttpGet("tenants")]
public async Task<IActionResult> GetAllTenantsAsync()
{
    // Only SuperAdmin can access
}
```

### RequireRoles
Custom role combinations:
```csharp
[RequireRoles("Admin", "SuperAdmin", "FoDo")]
[HttpGet("custom")]
public async Task<IActionResult> CustomEndpointAsync()
{
    // Requires Admin OR SuperAdmin OR FoDo
}
```

## Constructor Overloads

### 1. Permission-Only
```csharp
[Permission("18-12-1", "18-12-2")]  // Permission strings
```

### 2. Role-Only (via RequireRolesAttribute)
```csharp
[RequireRoles("SuperAdmin", "Admin")]  // Role names
```

### 3. Combined (Role AND Permission)
```csharp
[Permission(
    permissions: new[] { "18-12-1" },
    requiredRoles: new[] { "Admin" },
    requireBoth: true  // Must have both
)]
```

### 4. Combined (Role OR Permission)
```csharp
[Permission(
    permissions: new[] { "18-12-1" },
    requiredRoles: new[] { "SuperAdmin" },
    requireBoth: false  // Must have at least one
)]
```

## Migration from RoleAuthorizationFilterAttribute

If you were using `RoleAuthorizationFilterAttribute`, replace it with:

**Before:**
```csharp
[RoleAuthorizationFilter("SuperAdmin", "Admin")]
public class AdminController : BaseAdminApiController
{
    // ...
}
```

**After:**
```csharp
[RequireRoles("SuperAdmin", "Admin")]
// OR
[AdminOrSuperAdmin]  // If checking Admin or SuperAdmin
public class AdminController : BaseAdminApiController
{
    // ...
}
```

## Best Practices

1. **Use convenience attributes when possible:**
   ```csharp
   [AdminOrSuperAdmin]  // ✅ Clear and concise
   // Instead of
   [RequireRoles("Admin", "SuperAdmin")]  // More verbose
   ```

2. **Controller-level role checks, method-level permission checks:**
   ```csharp
   [AdminOrSuperAdmin]  // Controller level
   public class AdminController : BaseAdminApiController
   {
       [Permission(MenuPermissionConstant.AdminManagementView)]  // Method level
       [HttpGet]
       public async Task<IActionResult> GetAdminsAsync() { }
   }
   ```

3. **Use `requireBoth: true` sparingly:**
   - Only when you truly need both role AND permission
   - Most cases should use role OR permission

4. **Prefer permissions for fine-grained control:**
   - Roles are coarse-grained (Admin, SuperAdmin, etc.)
   - Permissions are fine-grained (View, Create, Update, Delete, Export)
   - Use permissions when you need specific operation-level control

## Examples

### Example 1: Admin Controller
```csharp
[ApiController]
[Route("api/v1/Admin")]
[AdminOrSuperAdmin]  // All endpoints require Admin or SuperAdmin
public class AdminController : BaseAdminApiController
{
    [HttpGet]
    [Permission(MenuPermissionConstant.AdminManagementView)]  // + View permission
    public async Task<IActionResult> ListAsync()
        => HandleResult(await _service.GetAdminsAsync());

    [HttpPost]
    [Permission(MenuPermissionConstant.AdminManagementCreate)]  // + Create permission
    public async Task<IActionResult> CreateAsync([FromBody] CreateAdminDto dto)
        => HandleResult(await _service.CreateAsync(dto));
}
```

### Example 2: SuperAdmin Only Endpoint
```csharp
[ApiController]
[Route("api/v1/Admin/Tenant")]
[AdminOrSuperAdmin]  // Base requirement
public class AdminTenantController : BaseAdminApiController
{
    [HttpGet]
    [SuperAdminOnly]  // Override: Only SuperAdmin
    [Permission(MenuPermissionConstant.TenantsView)]
    public async Task<IActionResult> GetAllTenantsAsync()
        => HandleResult(await _service.GetAllTenantsAsync());
}
```

### Example 3: Custom Role Combination
```csharp
[ApiController]
[Route("api/v1/Reports")]
public class ReportsController : BaseAdminApiController
{
    [HttpGet("admin-reports")]
    [RequireRoles("Admin", "SuperAdmin")]  // Custom role combination
    [Permission(MenuPermissionConstant.ReportsView)]
    public async Task<IActionResult> GetAdminReportsAsync()
        => HandleResult(await _service.GetAdminReportsAsync());
}
```

## Error Responses

All authorization failures return:
```json
{
  "isSuccess": false,
  "error": "Access denied. Required roles: Admin, SuperAdmin",
  "errorCode": "4032"
}
```

Status Code: `403 Forbidden`

---

**Last Updated:** 2024  
**Version:** 2.0

