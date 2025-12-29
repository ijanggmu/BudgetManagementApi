# TenantId Implementation Summary

## Overview
This document summarizes the implementation of `TenantId` across all entities and the special handling for SuperAdmin users.

---

## Entities with TenantId

### ✅ **Identity Entities**
1. **ApplicationUser** - `TenantId` property added, implements `ITenantEntity`
   - **SuperAdmin**: `TenantId = null` (can access all tenants)
   - **Regular Users**: `TenantId` automatically set from tenant context

2. **ApplicationRole** - `TenantId` property added, implements `ITenantEntity`
   - Roles can be tenant-specific or global

### ✅ **User Profile Entities**
3. **Admin** - `TenantId` property added, implements `ITenantEntity`
   - **SuperAdmin**: `TenantId = null`
   - **Tenant Admin**: `TenantId` set to their tenant

4. **Agent** - `TenantId` property added, implements `ITenantEntity`
   - Agents belong to specific tenants

5. **Customer** - `TenantId` property added, implements `ITenantEntity`
   - Customers belong to specific tenants

6. **Corporate** - `TenantId` property added, implements `ITenantEntity`
   - Corporates belong to specific tenants

### ✅ **Tenant-Specific Entities** (Already had TenantId via TenantEntity)
- Lead
- LeadActivity
- Prospect
- Contact
- Quotation
- QuotationItem
- Notification
- NotificationHistory
- AttendanceEntry
- RenewalReminder
- Product
- RatingArtifacts

---

## SuperAdmin Handling

### **SuperAdmin Characteristics:**
- ✅ **TenantId = null** (or empty)
- ✅ Can access **all tenants**
- ✅ Not restricted by tenant context
- ✅ Can manage system-wide operations
- ✅ Can create/manage tenants

### **Implementation Details:**

#### 1. **Automatic TenantId Assignment (ApplicationDataContext)**
```csharp
// SuperAdmin users are skipped when setting TenantId
if (user is ApplicationUser && user has SuperAdmin role)
{
    // Skip TenantId assignment - keep it null
    continue;
}
```

#### 2. **Tenant Resolution (TenantResolutionMiddleware)**
```csharp
// SuperAdmin doesn't require tenant resolution
if (userRoles.Contains(SystemRoles.SuperAdmin))
{
    // SuperAdmin can work across all tenants
    // Tenant resolved from header/subdomain if needed, but not required
}
```

#### 3. **User Seeding (UserSeeder)**
```csharp
// SuperAdmin created with null TenantId
TenantId = userInfo.Role == SystemRoles.SuperAdmin ? null : null
```

---

## Automatic TenantId Assignment

### **How It Works:**
1. All entities implementing `ITenantEntity` automatically get `TenantId` set
2. Assignment happens in `ApplicationDataContext.UpdateShadowFields()`
3. Only sets `TenantId` if it's null or empty
4. **Exception**: SuperAdmin users are skipped

### **When TenantId is Set:**
- ✅ On entity creation (`EntityState.Added`)
- ✅ On entity modification if `TenantId` is empty (backward compatibility)
- ✅ From `ITenantContext.TenantId` (current tenant context)

### **When TenantId is NOT Set:**
- ❌ For SuperAdmin users (always null)
- ❌ If `TenantId` already has a value
- ❌ If no tenant context is available

---

## Tenant Resolution Priority

The system resolves tenant in this order:

1. **X-Tenant Header** or **Subdomain**
   - Explicit tenant specification
   - Highest priority

2. **UserId from JWT Token**
   - Extract userId from claims
   - Query user's `TenantId`
   - Resolve tenant from user's `TenantId`
   - **Exception**: SuperAdmin users don't require tenant resolution

3. **Default/No Tenant**
   - If no tenant found, operations may be restricted
   - SuperAdmin can still operate

---

## Entities That May Need TenantId (Future Consideration)

These entities currently don't have `TenantId` but might need it in the future:

### **Logging/Audit Entities:**
- `UserOtp` - Could be tenant-specific if OTPs are tenant-scoped
- `EmailLog` - Could be tenant-specific for tenant email tracking
- `SmsLog` - Could be tenant-specific for tenant SMS tracking

### **Policy/Transaction Entities:**
- `PaymentTransaction` - Might need tenant context
- `PolicyDraft` - Might need tenant context
- `Motor`, `PrivateVehicle`, `ITI` - Might need tenant context

**Note**: These are currently system-wide entities. Add `TenantId` only if multi-tenant isolation is required.

---

## ITenantEntity Interface

```csharp
public interface ITenantEntity
{
    string TenantId { get; set; }
}
```

### **Purpose:**
- Marks entities that support multi-tenancy
- Enables automatic `TenantId` assignment
- Provides consistent pattern across codebase

### **Entities Implementing ITenantEntity:**
- ✅ `TenantEntity` (base class)
- ✅ `ApplicationUser`
- ✅ `ApplicationRole`
- ✅ `Admin`
- ✅ `Agent`
- ✅ `Customer`
- ✅ `Corporate`
- ✅ All entities inheriting from `TenantEntity`

---

## Best Practices

### ✅ **DO:**
- Always implement `ITenantEntity` for tenant-scoped entities
- Let the system automatically assign `TenantId` (don't set manually unless needed)
- Keep SuperAdmin `TenantId` as null
- Use tenant context for automatic assignment

### ❌ **DON'T:**
- Don't manually set `TenantId` for SuperAdmin
- Don't set `TenantId` if entity should be system-wide
- Don't bypass tenant resolution for regular users
- Don't hardcode `TenantId` values

---

## Migration Notes

### **Database Migration Required:**
When adding `TenantId` to existing entities, you'll need to:

1. **Create Migration:**
   ```bash
   dotnet ef migrations add "AddTenantIdToEntities" -c ApplicationDataContext -o Migrations/Core
   ```

2. **Update Existing Data:**
   - Set `TenantId` for existing records based on business logic
   - SuperAdmin users should have `TenantId = null`
   - Other users should be assigned to appropriate tenants

3. **Apply Migration:**
   ```bash
   dotnet ef database update -c ApplicationDataContext
   ```

---

## Testing Checklist

- [ ] SuperAdmin can access all tenants
- [ ] SuperAdmin has `TenantId = null`
- [ ] Regular users get `TenantId` automatically assigned
- [ ] Tenant resolution works from header/subdomain
- [ ] Tenant resolution works from userId
- [ ] Entities implementing `ITenantEntity` get `TenantId` automatically
- [ ] Admin, Agent, Customer, Corporate entities have `TenantId`
- [ ] Tenant-specific entities (Lead, Prospect, etc.) have `TenantId`
- [ ] Global query filters work correctly with `TenantId`

---

## Summary

| Entity | Has TenantId? | SuperAdmin Handling | Auto-Assigned? |
|--------|---------------|---------------------|-----------------|
| ApplicationUser | ✅ Yes | `null` for SuperAdmin | ✅ Yes |
| ApplicationRole | ✅ Yes | Can be tenant-specific | ✅ Yes |
| Admin | ✅ Yes | `null` for SuperAdmin | ✅ Yes |
| Agent | ✅ Yes | Tenant-specific | ✅ Yes |
| Customer | ✅ Yes | Tenant-specific | ✅ Yes |
| Corporate | ✅ Yes | Tenant-specific | ✅ Yes |
| Lead | ✅ Yes (via TenantEntity) | N/A | ✅ Yes |
| Prospect | ✅ Yes (via TenantEntity) | N/A | ✅ Yes |
| Contact | ✅ Yes (via TenantEntity) | N/A | ✅ Yes |
| Quotation | ✅ Yes (via TenantEntity) | N/A | ✅ Yes |
| Notification | ✅ Yes (via TenantEntity) | N/A | ✅ Yes |
| AttendanceEntry | ✅ Yes (via TenantEntity) | N/A | ✅ Yes |

**Key Points:**
- ✅ All user-related entities now have `TenantId`
- ✅ SuperAdmin has special handling (null TenantId)
- ✅ Automatic assignment works for all `ITenantEntity` implementations
- ✅ Tenant resolution enhanced to support userId lookup

