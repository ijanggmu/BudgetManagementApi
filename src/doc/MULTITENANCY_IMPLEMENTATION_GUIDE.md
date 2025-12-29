# BeemaEdge Multitenancy Implementation Guide

## Overview

BeemaEdge implements a comprehensive multi-tenant architecture with automatic tenant isolation, role-based access control, and global query filters. This document explains how multitenancy is implemented and how to work with it.

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Entity Types and Global Filters](#entity-types-and-global-filters)
3. [Role-Based Access Control](#role-based-access-control)
4. [Tenant Resolution](#tenant-resolution)
5. [Query Filtering](#query-filtering)
6. [SuperAdmin Access](#superadmin-access)
7. [Best Practices](#best-practices)
8. [Refactoring Summary](#refactoring-summary)

---

## Architecture Overview

### Key Components

1. **TenantContext**: Provides current tenant information from middleware
2. **ApplicationDataContext**: Applies global query filters automatically
3. **UserRoleHelperService**: Centralized role checking service
4. **RoleAuthorizationFilter**: Controller-level role authorization
5. **Global Query Filters**: Automatic filtering by TenantId and IsDeleted

### Tenant Isolation Strategy

The system uses **shared database with tenant isolation** approach:
- All tenants share the same database
- Data is isolated using `TenantId` column
- Global query filters automatically filter by `TenantId`
- SuperAdmin users can access all tenants

---

## Entity Types and Global Filters

### Entity Hierarchy

```
IBaseEntity (IsDeleted property)
  └─ ApplicationBaseEntity
      ├─ TenantEntity (TenantId + global filters)
      └─ Other entities implementing ITenantEntity
```

### 1. TenantEntity (Full Global Filtering)

Entities that **inherit from `TenantEntity`** automatically get:
- ✅ Global query filter for `TenantId`
- ✅ Global query filter for `IsDeleted`
- ✅ Row versioning for concurrency

**Examples:**
- `Lead`
- `Quotation`
- `QuotationItem`
- `LeadActivity`
- `Notification`
- `AttendanceEntry`
- `Branch`
- `Designation`
- `Contact`
- `Prospect`

**Usage:**
```csharp
// No manual filtering needed - global filters apply automatically
var leads = await _db.Set<Lead>()
    .Where(l => l.Status == LeadStatus.New)
    .ToListAsync();
```

### 2. ITenantEntity (Tenant Filtering Only)

Entities that **implement `ITenantEntity` but don't inherit from `TenantEntity`** get:
- ✅ Global query filter for `TenantId`
- ✅ Global query filter for `IsDeleted`
- ❌ No row versioning

**Examples:**
- `Admin`
- `Fodo` (Marketing Executive)
- `ApplicationUser`
- `ApplicationRole`
- `Customer`
- `Corporate`

**Usage:**
```csharp
// No manual TenantId or IsDeleted filtering needed
var admins = await _db.Admins
    .Include(a => a.User)
    .ToListAsync();
```

### 3. ApplicationBaseEntity (IsDeleted Filtering Only)

Entities that **only inherit from `ApplicationBaseEntity`** get:
- ✅ Global query filter for `IsDeleted`
- ❌ No tenant filtering (not tenant-specific)

**Examples:**
- `CompanyBranding`
- `Tenant` (the tenant entity itself)
- `Country`
- `EmailLog`
- `SmsLog`

**Usage:**
```csharp
// Only IsDeleted is filtered automatically
// Manual TenantId filtering may be required for tenant-specific entities
var branding = await _db.Set<CompanyBranding>()
    .Where(b => b.TenantId == tenantId) // Manual tenant filter
    .FirstOrDefaultAsync();
```

---

## Role-Based Access Control

### System Roles

1. **SuperAdmin** (Level 999)
   - Can access all tenants
   - `TenantId = null` in user record
   - Can bypass tenant filters using `IgnoreQueryFilters()`

2. **Admin** (Level 500)
   - Tenant-specific access only
   - `TenantId` set to their tenant
   - Automatically filtered by tenant

3. **Marketing Executive / FoDo** (Level 300)
   - Own data only
   - Tenant-scoped
   - Limited to assigned leads/quotations

### Role Authorization

Role authorization is handled at the **controller level** using filter attributes. Services should **not** perform role authorization checks.

**Controller-Level Authorization:**
```csharp
[ApiController]
[Route("api/v1/Admin")]
[AdminOrSuperAdmin] // All endpoints require Admin or SuperAdmin role
public class AdminController : BaseAdminApiController
{
    [HttpGet]
    [Permission(MenuPermissionConstant.AdminManagementView)]
    public async Task<IActionResult> ListAsync()
    {
        // Role already verified by [AdminOrSuperAdmin] filter
        // No need to check roles in service
    }
}
```

**Service-Level Role Checking (for query logic only):**
Services only check roles when needed for query filtering logic (e.g., to determine if `IgnoreQueryFilters()` should be used):

```csharp
public class AdminService
{
    public async Task<Result<List<AdminDto>>> GetAdminsAsync()
    {
        // Get roleId from JWT token (no database query needed)
        var roleId = _userProfileService.GetRoleId();
        var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

        var query = _db.Admins;
        
        // Only check role for query filtering logic, not authorization
        if (isSuperAdmin)
            query = query.IgnoreQueryFilters();

        // Query logic...
    }
}
```

### RoleAuthorizationFilter

Controller-level role authorization:

```csharp
[ApiController]
[Route("api/v1/Admin")]
[AdminOrSuperAdmin] // Requires Admin or SuperAdmin role
public class AdminController : BaseAdminApiController
{
    [HttpGet]
    [Permission(MenuPermissionConstant.AdminManagementView)]
    public async Task<IActionResult> ListAsync()
    {
        // Only Admin or SuperAdmin can access
    }
}
```

**Available Attributes:**
- `[SuperAdminOnly]` - Only SuperAdmin
- `[AdminOrSuperAdmin]` - Admin or SuperAdmin
- `[RoleAuthorizationFilter("Role1", "Role2")]` - Custom roles

---

## Tenant Resolution

### TenantResolutionMiddleware

The middleware extracts tenant information from:
1. **Subdomain**: `tenant1.beemaedge.com`
2. **Header**: `X-Tenant-Id` or `X-Tenant-Slug`
3. **User's TenantId**: From authenticated user record

**Flow:**
```
Request → TenantResolutionMiddleware → ITenantContext → ApplicationDataContext.CurrentTenantId
```

### Setting TenantId on New Entities

When creating new entities, `TenantId` is automatically set in `ApplicationDataContext.SaveChanges()`:

```csharp
// In ApplicationDataContext.UpdateShadowFields()
if (_tenantContext?.TenantId is string tid)
{
    var addedTenantEntities = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Added && e.Entity is ITenantEntity);
    
    foreach (var item in addedTenantEntities)
    {
        if (string.IsNullOrEmpty(item.Entity.TenantId))
        {
            item.Entity.TenantId = tid;
        }
    }
}
```

**Exception:** SuperAdmin users have `TenantId = null` to allow cross-tenant access.

---

## Query Filtering

### Automatic Filtering

Global query filters are applied automatically to all queries:

```csharp
// This query automatically includes:
// WHERE TenantId = @CurrentTenantId AND IsDeleted = false
var leads = await _db.Set<Lead>()
    .Where(l => l.Status == LeadStatus.New)
    .ToListAsync();
```

### Bypassing Filters (SuperAdmin Only)

SuperAdmin users can bypass filters to see all tenants:

```csharp
IQueryable<Lead> query = _db.Set<Lead>();

if (roleInfo.IsSuperAdmin)
{
    query = query.IgnoreQueryFilters(); // Bypass both TenantId and IsDeleted filters
}

var allLeads = await query.ToListAsync();
```

### Manual Filtering (When Needed)

For entities without global tenant filters, manual filtering is required:

```csharp
// CompanyBranding doesn't have global tenant filter
var branding = await _db.Set<CompanyBranding>()
    .Where(b => b.TenantId == tenantId) // Manual filter required
    .FirstOrDefaultAsync();
```

---

## SuperAdmin Access

### Characteristics

1. **TenantId = null**: SuperAdmin users have null TenantId
2. **Bypass Filters**: Can use `IgnoreQueryFilters()` to see all data
3. **Cross-Tenant Operations**: Can create/manage entities for any tenant

### Implementation Pattern

```csharp
public async Task<Result<List<AdminDto>>> GetAdminsAsync(string? tenantId = null)
{
    var userId = _userProfileService.GetUserId();
    var roleInfo = await _roleHelperService.GetUserRoleInfoAsync(userId);
    
    if (!roleInfo.IsSuperAdmin && !roleInfo.IsAdmin)
        return Result<List<AdminDto>>.Failed("Unauthorized");

    IQueryable<Admin> query = _db.Admins.Include(a => a.User);

    if (roleInfo.IsSuperAdmin)
    {
        // SuperAdmin: filter by tenantId if provided, otherwise show all
        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(a => a.TenantId == tenantId);
        }
        query = query.IgnoreQueryFilters(); // Bypass global filters
    }
    // For Admin: global query filter automatically applies

    var admins = await query.ToListAsync();
    return Result<List<AdminDto>>.Success(admins);
}
```

---

## Best Practices

### 1. Use Filter Attributes for Authorization

❌ **Don't:**
```csharp
// In service
if (!roleInfo.IsSuperAdmin && !roleInfo.IsAdmin)
    return Result.Failed("Unauthorized");
```

✅ **Do:**
```csharp
// In controller
[AdminOrSuperAdmin] // Handles authorization
public class MyController : BaseAdminApiController
{
    // Service doesn't need to check authorization
}

// In service (only for query logic)
var roleId = _userProfileService.GetRoleId();
var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
```

### 2. Don't Manually Filter TenantId/IsDeleted for TenantEntity

❌ **Don't:**
```csharp
var leads = await _db.Set<Lead>()
    .Where(l => l.TenantId == tenantId && !l.IsDeleted) // Redundant!
    .ToListAsync();
```

✅ **Do:**
```csharp
var leads = await _db.Set<Lead>()
    .Where(l => l.Status == LeadStatus.New) // Global filters apply automatically
    .ToListAsync();
```

### 3. Use IgnoreQueryFilters() Only for SuperAdmin

❌ **Don't:**
```csharp
// Always bypassing filters
var query = _db.Set<Lead>().IgnoreQueryFilters();
```

✅ **Do:**
```csharp
var query = _db.Set<Lead>();
if (roleInfo.IsSuperAdmin)
{
    query = query.IgnoreQueryFilters();
}
```

### 4. Check Entity Type Before Querying

**For TenantEntity:**
- No manual TenantId/IsDeleted filtering needed
- Use `IgnoreQueryFilters()` for SuperAdmin

**For ITenantEntity (non-TenantEntity):**
- No manual TenantId/IsDeleted filtering needed
- Use `IgnoreQueryFilters()` for SuperAdmin

**For ApplicationBaseEntity only:**
- Manual TenantId filtering required if tenant-specific
- IsDeleted is filtered automatically

### 5. Use RoleAuthorizationFilter on Controllers

❌ **Don't:**
```csharp
[HttpGet]
public async Task<IActionResult> GetAdmins()
{
    // Manual role check in controller
    var roles = await GetUserRoles();
    if (!roles.Contains("Admin")) return Forbid();
    // ...
}
```

✅ **Do:**
```csharp
[HttpGet]
[AdminOrSuperAdmin] // Automatic role check
[Permission(MenuPermissionConstant.AdminManagementView)]
public async Task<IActionResult> GetAdmins()
{
    // Role already verified by filter
    // ...
}
```

---

## Refactoring Summary

### What Was Refactored

1. **Created UserRoleHelperService**
   - Centralized role checking logic
   - Eliminates duplicate role queries
   - Provides `UserRoleInfo` with all role information

2. **Created RoleAuthorizationFilter**
   - Controller-level role authorization
   - `[SuperAdminOnly]`, `[AdminOrSuperAdmin]` attributes
   - Reduces boilerplate in controllers

3. **Added Global Query Filters**
   - **IsDeleted filter**: Applied to all `IBaseEntity` entities
   - **TenantId filter for ITenantEntity**: Applied to entities implementing `ITenantEntity` but not inheriting `TenantEntity`
   - **TenantId filter for TenantEntity**: Already existed, now documented

4. **Refactored Services**
   - Removed manual role checking code
   - Removed manual `IsDeleted` filtering
   - Removed manual `TenantId` filtering where global filters apply
   - Used `UserRoleHelperService` for role checks

### Benefits

1. **Consistency**: All services use the same role checking pattern
2. **Maintainability**: Role logic centralized in one place
3. **Performance**: Reduced duplicate role queries
4. **Safety**: Global filters prevent accidental data leaks
5. **Clarity**: Code is more readable and self-documenting

### Migration Notes

**Before:**
```csharp
var user = await userManager.FindByIdAsync(userId);
var roles = await userManager.GetRolesAsync(user);
var isSuperAdmin = roles.Contains(SystemRoles.SuperAdmin);
var isAdmin = roles.Contains(SystemRoles.Admin);

if (!isSuperAdmin && !isAdmin)
    return Result.Failed("Unauthorized");

var query = db.Admins
    .Where(a => !a.IsDeleted && !a.User.IsDeleted);
    
if (isSuperAdmin)
    query = query.IgnoreQueryFilters();
```

**After:**
```csharp
var roleInfo = await _roleHelperService.GetUserRoleInfoAsync(userId);
if (!roleInfo.IsSuperAdmin && !roleInfo.IsAdmin)
    return Result.Failed("Unauthorized");

var query = db.Admins.Include(a => a.User);
// IsDeleted and TenantId filters applied automatically

if (roleInfo.IsSuperAdmin)
    query = query.IgnoreQueryFilters();
```

---

## Common Patterns

### Pattern 1: List with Tenant Filtering

**Controller:**
```csharp
[AdminOrSuperAdmin] // Role authorization handled here
[HttpGet]
public async Task<IActionResult> GetEntitiesAsync()
    => HandleResult(await _service.GetEntitiesAsync());
```

**Service:**
```csharp
public async Task<Result<List<EntityDto>>> GetEntitiesAsync()
{
    // Role authorization is handled by filter attribute on controller
    // Only check role for query filtering logic
    var roleId = _userProfileService.GetRoleId();
    var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

    IQueryable<Entity> query = _db.Set<Entity>();
    
    if (isSuperAdmin)
        query = query.IgnoreQueryFilters();
    // For Admin: global filter applies automatically

    var entities = await query.ToListAsync();
    return Result<List<EntityDto>>.Success(entities);
}
```

### Pattern 2: Get by ID with Tenant Verification

**Controller:**
```csharp
[AdminOrSuperAdmin] // Role authorization handled here
[HttpGet("{id}")]
public async Task<IActionResult> GetEntityByIdAsync(string id)
    => HandleResult(await _service.GetEntityByIdAsync(id));
```

**Service:**
```csharp
public async Task<Result<EntityDto>> GetEntityByIdAsync(string id)
{
    // Role authorization is handled by filter attribute on controller
    var roleId = _userProfileService.GetRoleId();
    var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);
    
    IQueryable<Entity> query = _db.Set<Entity>().Where(e => e.Id == id);
    
    if (isSuperAdmin)
        query = query.IgnoreQueryFilters();
    // For Admin: global filter ensures tenant isolation

    var entity = await query.FirstOrDefaultAsync();
    if (entity == null)
        return Result<EntityDto>.Failed("Entity not found");

    return Result<EntityDto>.Success(MapToDto(entity));
}
```

### Pattern 3: Create with Tenant Assignment

**Controller:**
```csharp
[AdminOrSuperAdmin] // Role authorization handled here
[HttpPost]
public async Task<IActionResult> CreateAsync([FromBody] CreateEntityDto dto)
    => HandleResult(await _service.CreateAsync(dto));
```

**Service:**
```csharp
public async Task<Result<EntityDto>> CreateAsync(CreateEntityDto dto)
{
    // Role authorization is handled by filter attribute on controller
    var roleId = _userProfileService.GetRoleId();
    var isSuperAdmin = !string.IsNullOrEmpty(roleId) && roleId.Contains(SystemRoles.SuperAdmin);

    // TenantId will be set automatically in SaveChanges()
    // For SuperAdmin: can specify tenantId in dto
    // For Admin: uses CurrentTenantId from context
    
    var entity = new Entity
    {
        // Properties from dto
        TenantId = isSuperAdmin ? dto.TenantId : _db.CurrentTenantId
    };

    await _db.Set<Entity>().AddAsync(entity);
    await _db.SaveChangesAsync();

    return Result<EntityDto>.Success(MapToDto(entity));
}
```

---

## Troubleshooting

### Issue: Query returns no results

**Possible Causes:**
1. Global tenant filter is excluding data
2. Global IsDeleted filter is excluding soft-deleted records
3. User doesn't have access to the tenant

**Solution:**
- Check if user is SuperAdmin and use `IgnoreQueryFilters()` if needed
- Verify `CurrentTenantId` is set correctly
- Check if records are soft-deleted

### Issue: Cannot access cross-tenant data as SuperAdmin

**Solution:**
- Ensure `IgnoreQueryFilters()` is called on the query
- Verify user has SuperAdmin role
- Check that user's `TenantId` is null

### Issue: TenantId not being set on new entities

**Solution:**
- Ensure entity implements `ITenantEntity`
- Check that `TenantContext.TenantId` is set
- Verify `SaveChanges()` is called (not `SaveChangesAsync()` without await)

---

## Summary

The BeemaEdge multitenancy implementation provides:

1. **Automatic Tenant Isolation**: Global query filters ensure data isolation
2. **Role-Based Access**: SuperAdmin, Admin, and Marketing Executive roles
3. **Centralized Role Checking**: `UserRoleHelperService` eliminates duplication
4. **Controller-Level Authorization**: `RoleAuthorizationFilter` attributes
5. **Type-Safe Entity Filtering**: Different filter strategies based on entity type

By following these patterns and best practices, you can ensure secure, maintainable, and efficient multitenant operations.

---

**Last Updated:** 2024  
**Version:** 2.0

