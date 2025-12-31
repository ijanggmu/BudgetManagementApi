# Tenant Filtering Explanation

## Global Query Filter vs Manual Filtering

### Entities with Global Query Filter

Entities that **inherit from `TenantEntity`** automatically get a **global query filter** applied in `ApplicationDataContext.cs` (lines 240-250). This means:

- **You DON'T need to manually add `TenantId` to queries** for these entities
- The filter is automatically applied: `WHERE TenantId = CurrentTenantId`
- Examples: `Lead`, `Quotation`, `QuotationItem`, `LeadActivity`, `Notification`, etc.

**Example:**
```csharp
// This automatically filters by TenantId
var leads = await _db.Set<Lead>()
    .Where(l => l.Status == LeadStatus.New)
    .ToListAsync();
```

### Entities WITHOUT Global Query Filter

Entities that **do NOT inherit from `TenantEntity`** require **manual filtering**:

- **You MUST manually add `TenantId` to queries**
- Examples: `CompanyBranding`, `Tenant`, `ApplicationUser`, `ApplicationRole`

**Example:**
```csharp
// CompanyBranding does NOT inherit from TenantEntity
// So we MUST manually filter by TenantId
var branding = await _db.Set<CompanyBranding>()
    .Where(b => b.TenantId == _tenant.TenantId)
    .FirstOrDefaultAsync();
```

## How to Check if an Entity Has Global Filter

1. Check if the entity class inherits from `TenantEntity`:
   ```csharp
   public class Lead : TenantEntity  // ✅ Has global filter
   public class CompanyBranding : ApplicationBaseEntity  // ❌ No global filter
   ```

2. Check `ApplicationDataContext.cs` - the filter is applied to all types where:
   ```csharp
   typeof(TenantEntity).IsAssignableFrom(t.ClrType)
   ```

## SuperAdmin Access

For **SuperAdmin** users who need to access data across all tenants:

1. **For TenantEntity types**: Use `.IgnoreQueryFilters()` to bypass the global filter
   ```csharp
   var allLeads = await _db.Set<Lead>()
       .IgnoreQueryFilters()  // Bypass global tenant filter
       .ToListAsync();
   ```

2. **For non-TenantEntity types**: Just query without TenantId filter
   ```csharp
   var allBranding = await _db.Set<CompanyBranding>()
       .ToListAsync();  // No filter needed since there's no global filter
   ```

## Best Practices

1. **Always check the entity inheritance** before writing queries
2. **For TenantEntity**: Let the global filter handle it, use `IgnoreQueryFilters()` for SuperAdmin
3. **For non-TenantEntity**: Always manually filter by `TenantId` for tenant-specific data
4. **For SuperAdmin**: Use role checks and `IgnoreQueryFilters()` when needed

## Current Implementation Status

### ✅ Has Global Filter (TenantEntity)
- Lead
- Quotation
- QuotationItem
- LeadActivity
- Notification
- AttendanceEntry
- RenewalReminder
- Contact
- Prospect

### ❌ No Global Filter (Manual Filtering Required)
- CompanyBranding
- Tenant
- ApplicationUser
- ApplicationRole

## Example: BrandingService

Since `CompanyBranding` does NOT inherit from `TenantEntity`, we manually filter:

```csharp
// Manual filtering required
var branding = await _db.Set<CompanyBranding>()
    .FirstOrDefaultAsync(x => x.TenantId == _tenant.TenantId);
```

This is correct and necessary!

