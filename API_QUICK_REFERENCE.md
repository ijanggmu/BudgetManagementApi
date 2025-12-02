# BeemaEdge API Quick Reference Guide

## Base URL
```
https://api.beemaedge.com/api/v1
```

## Authentication
```typescript
Headers: {
  'Authorization': 'Bearer {token}',
  'Content-Type': 'application/json'
}
```

---

## Quick Endpoint Reference

### Lead Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/leads` | Create lead |
| GET | `/leads` | List leads (with filters) |
| GET | `/leads/{id}` | Get lead by ID |
| PATCH | `/leads/{id}/status` | Update lead status |
| POST | `/leads/{id}/activities` | Add activity |
| GET | `/leads/{id}/activities` | Get activities |
| GET | `/leads/{leadId}/quotations` | Get quotations for lead |
| GET | `/admin/leads` | Admin: List all leads |
| GET | `/admin/leads/{id}` | Admin: Get lead details |
| GET | `/admin/leads/tenant/{tenantId}` | SuperAdmin: Get leads by tenant |

---

### Quotation

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/quotations` | Create quotation |
| GET | `/quotations` | List quotations |
| GET | `/quotations/{id}` | Get quotation by ID |
| PATCH | `/quotations/{id}` | Update quotation |
| DELETE | `/quotations/{id}` | Delete quotation |
| GET | `/quotations/{id}/pdf` | Generate PDF |
| GET | `/admin/quotations` | Admin: List all quotations |
| GET | `/admin/quotations/{id}` | Admin: Get quotation details |
| GET | `/admin/quotations/tenant/{tenantId}` | SuperAdmin: Get quotations by tenant |

---

### Premium Calculation

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/premiums/calculate` | Calculate premium |
| POST | `/premiums/calculate-endorsement` | Calculate endorsement premium |

**Admin Configuration:**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/premium-calculation/configurations` | List configurations |
| GET | `/admin/premium-calculation/configurations/{id}` | Get configuration |
| GET | `/admin/premium-calculation/configurations/portfolio/{alias}/fiscal-year/{year}` | Get by portfolio & fiscal year |
| POST | `/admin/premium-calculation/configurations` | Create configuration |
| PUT | `/admin/premium-calculation/configurations/{id}` | Update configuration |
| DELETE | `/admin/premium-calculation/configurations/{id}` | Delete configuration |
| POST | `/admin/premium-calculation/configurations/{id}/activate` | Activate configuration |
| POST | `/admin/premium-calculation/configurations/{id}/clone` | Clone configuration |
| POST | `/admin/premium-calculation/configurations/{id}/validate` | Validate configuration |

**Parameters:**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/premium-calculation/parameters/configuration/{configId}` | Get parameters |
| GET | `/admin/premium-calculation/parameters/{id}` | Get parameter |
| POST | `/admin/premium-calculation/parameters/configuration/{configId}` | Create parameter |
| PUT | `/admin/premium-calculation/parameters/{id}` | Update parameter |
| DELETE | `/admin/premium-calculation/parameters/{id}` | Delete parameter |
| POST | `/admin/premium-calculation/parameters/configuration/{configId}/bulk` | Bulk update |

**Rules:**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/premium-calculation/rules/configuration/{configId}` | Get rules |
| GET | `/admin/premium-calculation/rules/{id}` | Get rule |
| POST | `/admin/premium-calculation/rules/configuration/{configId}` | Create rule |
| PUT | `/admin/premium-calculation/rules/{id}` | Update rule |
| DELETE | `/admin/premium-calculation/rules/{id}` | Delete rule |

**Rate Tables:**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/premium-calculation/rate-tables/configuration/{configId}` | Get rate tables |
| GET | `/admin/premium-calculation/rate-tables/{id}` | Get rate table |
| POST | `/admin/premium-calculation/rate-tables/configuration/{configId}` | Create rate table |
| PUT | `/admin/premium-calculation/rate-tables/{id}` | Update rate table |
| DELETE | `/admin/premium-calculation/rate-tables/{id}` | Delete rate table |

---

### Notification

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/notifications` | Get my notifications |
| PATCH | `/notifications/{id}/read` | Mark as read |

---

### Attendance

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/attendance/check-in` | Check in |
| POST | `/attendance/check-out` | Check out |
| GET | `/attendance/daily` | Get daily attendance |
| GET | `/attendance/monthly` | Get monthly attendance |

---

### Renewal

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/renewals/trigger-reminders` | Trigger reminders (Admin) |
| GET | `/renewals/my-reminders` | Get my reminders |

---

## Common Query Parameters

### Pagination
```typescript
{
  pageNumber: number;    // Default: 1
  pageSize: number;       // Default: 10, Max: 600
  query?: string;         // Search query
  filters?: string;       // Sieve filters
  sorts?: string;         // Sort order
}
```

### Date Filters
```typescript
{
  from?: string;  // ISO 8601: "2024-01-01T00:00:00Z"
  to?: string;    // ISO 8601: "2024-12-31T23:59:59Z"
}
```

---

## Status Enums

### Lead Status
```typescript
type LeadStatus = 'New' | 'Qualified' | 'Contacted' | 'Quoted' | 'Won' | 'Lost';
```

### Quotation Status
```typescript
type QuotationStatus = 'Draft' | 'Submitted' | 'Approved' | 'Declined' | 'Accepted';
```

### Activity Kind
```typescript
type ActivityKind = 'note' | 'call' | 'email';
```

### Rule Type
```typescript
type RuleType = 'Formula' | 'Condition' | 'Validation' | 'Discount';
```

### Notification Channel
```typescript
type NotificationChannel = 'Email' | 'SMS' | 'Push';
```

---

## Example Usage (Next.js)

### Setup
```typescript
// lib/api-client.ts
import { apiClient } from '@/lib/api-client';

// .env.local
NEXT_PUBLIC_API_URL=https://api.beemaedge.com/api/v1
```

### Create Lead
```typescript
const response = await apiClient.leads.create({
  fullName: 'John Doe',
  email: 'john@example.com',
  phone: '+9779801234567',
  productCode: 'FIRE'
});

if (response.isSuccess) {
  console.log('Lead created:', response.data);
} else {
  console.error('Error:', response.error);
}
```

### List Leads with Filters
```typescript
const response = await apiClient.leads.list({
  pageNumber: 1,
  pageSize: 20,
  status: 'New',
  from: '2024-01-01T00:00:00Z',
  to: '2024-12-31T23:59:59Z'
});
```

### Calculate Premium
```typescript
const response = await apiClient.premiums.calculate({
  portfolioAlias: 'FIRE',
  fiscalYear: '2081-82',
  sumInsured: 1000000,
  policyPeriodInDays: 365
});
```

### Create Quotation
```typescript
const response = await apiClient.quotations.create({
  productId: '550e8400-e29b-41d4-a716-446655440000',
  prospectId: '660e8400-e29b-41d4-a716-446655440000',
  items: [
    {
      coverageId: '770e8400-e29b-41d4-a716-446655440000',
      sumInsured: 1000000
    }
  ]
});
```

### Admin: Create Premium Configuration
```typescript
const response = await apiClient.adminPremiumConfigs.create({
  portfolioAlias: 'FIRE',
  fiscalYear: '2081-82',
  effectiveFrom: '2024-04-01',
  effectiveTo: '2025-03-31',
  description: 'Fire Insurance Configuration',
  calculationEngineType: 'FormulaBased'
});
```

### Admin: Add Parameter
```typescript
const response = await apiClient.adminPremiumParameters.create(
  'config-123',
  {
    parameterKey: 'BasicPremiumRate',
    parameterName: 'Basic Premium Rate (%)',
    dataType: 'decimal',
    value: '0.5',
    defaultValue: '0.5',
    minValue: 0.1,
    maxValue: 10.0,
    isRequired: true,
    displayOrder: 1,
    category: 'Basic'
  }
);
```

### Admin: Add Rule
```typescript
const response = await apiClient.adminPremiumRules.create(
  'config-123',
  {
    ruleName: 'Calculate Basic Premium',
    ruleType: 'Formula',
    expression: 'SumInsured * BasicPremiumRate / 100',
    priority: 1,
    isActive: true
  }
);
```

---

## Error Handling

```typescript
try {
  const response = await apiClient.leads.create(data);
  
  if (!response.isSuccess) {
    // Handle API error
    console.error('API Error:', response.error);
    console.error('Error Code:', response.errorCode);
    return;
  }
  
  // Success
  console.log('Success:', response.data);
} catch (error) {
  // Handle network error
  console.error('Network Error:', error);
}
```

---

## Response Format

### Success Response
```json
{
  "isSuccess": true,
  "data": { ... },
  "pagination": { ... }  // Only for list endpoints
}
```

### Error Response
```json
{
  "isSuccess": false,
  "error": "Error message",
  "errorCode": 400
}
```

---

## Date Formats

- **DateTime**: ISO 8601 format - `"2024-01-15T10:30:00Z"`
- **DateOnly**: YYYY-MM-DD format - `"2024-01-15"`

---

## Next Steps

1. Copy `FRONTEND_TYPES.ts` to your Next.js project as `types/api.ts`
2. Copy `NEXTJS_API_CLIENT_EXAMPLE.ts` to your Next.js project as `lib/api-client.ts`
3. Set environment variable: `NEXT_PUBLIC_API_URL`
4. Implement authentication token storage
5. Start using the API client!

---

**For detailed documentation, see `API_DOCUMENTATION.md`**

