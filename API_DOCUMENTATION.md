# BeemaEdge API Documentation

**Base URL**: `https://api.beemaedge.com/api/v1`  
**Authentication**: Bearer Token (JWT)  
**Content-Type**: `application/json`

---

## Table of Contents

1. [Lead Management Engine (LMS)](#1-lead-management-engine-lms)
2. [Quotation Engine](#2-quotation-engine)
3. [Premium Calculation Engine](#3-premium-calculation-engine)
4. [Notification Engine](#4-notification-engine)
5. [Attendance Engine](#5-attendance-engine)
6. [Renewal Engine](#6-renewal-engine)
7. [Common Models](#common-models)

---

## 1. Lead Management Engine (LMS)

### 1.1 FO/DO Endpoints

#### Create Lead
**POST** `/api/v1/leads`

**Request Body:**
```json
{
  "fullName": "John Doe",
  "email": "john.doe@example.com",
  "phone": "+9779801234567",
  "productCode": "FIRE"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "lead-123",
    "prospectId": "prospect-456",
    "status": "New",
    "source": "Web",
    "ownerUserId": null,
    "createdOn": "2024-01-15T10:30:00Z",
    "prospect": {
      "id": "prospect-456",
      "primaryContactId": "contact-789",
      "primaryContact": {
        "id": "contact-789",
        "fullName": "John Doe",
        "email": "john.doe@example.com",
        "phone": "+9779801234567"
      }
    }
  }
}
```

---

#### List Leads
**GET** `/api/v1/leads`

**Query Parameters:**
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10, max: 600)
- `query` (string, optional) - Search query
- `filters` (string, optional) - Sieve filters
- `sorts` (string, optional) - Sort order
- `status` (string, optional) - Filter by status: `New`, `Qualified`, `Contacted`, `Quoted`, `Won`, `Lost`
- `from` (DateTime, optional) - Filter from date: `2024-01-01T00:00:00Z`
- `to` (DateTime, optional) - Filter to date: `2024-12-31T23:59:59Z`

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "lead-123",
      "prospectId": "prospect-456",
      "status": "New",
      "source": "Web",
      "ownerUserId": null,
      "createdOn": "2024-01-15T10:30:00Z",
      "prospect": { ... }
    }
  ],
  "pagination": {
    "totalItems": 100,
    "totalPages": 10,
    "pageSize": 10,
    "currentPage": 1
  }
}
```

---

#### Get Lead by ID
**GET** `/api/v1/leads/{id}`

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "lead-123",
    "prospectId": "prospect-456",
    "status": "New",
    "source": "Web",
    "ownerUserId": null,
    "createdOn": "2024-01-15T10:30:00Z",
    "prospect": { ... }
  }
}
```

---

#### Update Lead Status
**PATCH** `/api/v1/leads/{id}/status`

**Request Body:**
```json
{
  "status": "Qualified"
}
```

**Valid Status Values:**
- `New`
- `Qualified`
- `Contacted`
- `Quoted`
- `Won`
- `Lost`

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "lead-123",
    "status": "Qualified",
    ...
  }
}
```

---

#### Add Lead Activity
**POST** `/api/v1/leads/{id}/activities`

**Request Body:**
```json
{
  "kind": "call",
  "notes": "Discussed coverage options with customer"
}
```

**Valid Activity Kinds:**
- `note` - General notes
- `call` - Phone call
- `email` - Email communication

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "activity-123",
    "leadId": "lead-123",
    "kind": "call",
    "notes": "Discussed coverage options with customer",
    "when": "2024-01-15T14:30:00Z"
  }
}
```

---

#### Get Lead Activities
**GET** `/api/v1/leads/{id}/activities`

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "activity-123",
      "leadId": "lead-123",
      "kind": "call",
      "notes": "Discussed coverage options",
      "when": "2024-01-15T14:30:00Z"
    }
  ]
}
```

---

#### Get Quotations by Lead ID
**GET** `/api/v1/leads/{leadId}/quotations`

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "quote-123",
      "number": "QT-2024-001",
      "status": "Draft",
      ...
    }
  ]
}
```

---

### 1.2 Admin Endpoints

#### Get All Leads (Admin)
**GET** `/api/v1/admin/leads`

**Query Parameters:**
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10)
- `query` (string, optional)
- `filters` (string, optional)
- `sorts` (string, optional)
- `status` (string, optional)
- `from` (DateTime, optional)
- `to` (DateTime, optional)

**Response:** Same as List Leads

---

#### Get Lead Details (Admin)
**GET** `/api/v1/admin/leads/{id}`

**Response:** Same as Get Lead by ID

---

#### Get Leads by Tenant ID (SuperAdmin Only)
**GET** `/api/v1/admin/leads/tenant/{tenantId}`

**Query Parameters:** Same as Get All Leads

**Response:** Same as List Leads

---

## 2. Quotation Engine

### 2.1 FO/DO Endpoints

#### Create Quotation
**POST** `/api/v1/quotations`

**Request Body:**
```json
{
  "productId": "550e8400-e29b-41d4-a716-446655440000",
  "prospectId": "660e8400-e29b-41d4-a716-446655440000",
  "items": [
    {
      "coverageId": "770e8400-e29b-41d4-a716-446655440000",
      "sumInsured": 1000000
    }
  ]
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "quote-123",
    "number": "QT-2024-001",
    "status": "Draft",
    "productId": "550e8400-e29b-41d4-a716-446655440000",
    "prospectId": "660e8400-e29b-41d4-a716-446655440000",
    "totalPremium": null,
    "discountPercent": null,
    "validUntil": null,
    "pdfUrl": null,
    "createdOn": "2024-01-15T10:30:00Z",
    "items": [
      {
        "id": "item-123",
        "quotationId": "quote-123",
        "coverageId": "770e8400-e29b-41d4-a716-446655440000",
        "sumInsured": 1000000,
        "premium": 0
      }
    ]
  }
}
```

---

#### List Quotations
**GET** `/api/v1/quotations`

**Query Parameters:**
- `pageNumber` (int, optional)
- `pageSize` (int, optional)
- `query` (string, optional)
- `filters` (string, optional)
- `sorts` (string, optional)

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "quote-123",
      "number": "QT-2024-001",
      "status": "Draft",
      ...
    }
  ],
  "pagination": { ... }
}
```

---

#### Get Quotation by ID
**GET** `/api/v1/quotations/{id}`

**Response:** Same as Create Quotation response

---

#### Update Quotation
**PATCH** `/api/v1/quotations/{id}`

**Request Body:**
```json
{
  "status": "Submitted",
  "totalPremium": 50000,
  "discountPercent": 10,
  "validUntil": "2024-12-31",
  "items": [
    {
      "coverageId": "770e8400-e29b-41d4-a716-446655440000",
      "sumInsured": 1000000
    }
  ]
}
```

**Response:** Updated quotation object

---

#### Delete Quotation
**DELETE** `/api/v1/quotations/{id}`

**Response:**
```json
{
  "isSuccess": true,
  "data": true
}
```

---

#### Generate PDF
**GET** `/api/v1/quotations/{id}/pdf`

**Response:**
```json
{
  "isSuccess": true,
  "data": "https://storage.example.com/quotes/quote-123.pdf"
}
```

---

### 2.2 Admin Endpoints

#### Get All Quotations (Admin)
**GET** `/api/v1/admin/quotations`

**Query Parameters:**
- `pageNumber` (int)
- `pageSize` (int)
- `query` (string, optional)
- `filters` (string, optional)
- `sorts` (string, optional)
- `status` (string, optional) - `Draft`, `Submitted`, `Approved`, `Declined`, `Accepted`
- `from` (DateTime, optional)
- `to` (DateTime, optional)

**Response:** Same as List Quotations

---

#### Get Quotation Details (Admin)
**GET** `/api/v1/admin/quotations/{id}`

**Response:** Same as Get Quotation by ID

---

#### Get Quotations by Tenant ID (SuperAdmin Only)
**GET** `/api/v1/admin/quotations/tenant/{tenantId}`

**Query Parameters:** Same as Get All Quotations

**Response:** Same as List Quotations

---

## 3. Premium Calculation Engine

### 3.1 Common Endpoints

#### Calculate Premium
**POST** `/api/v1/premiums/calculate`

**Request Body:**
```json
{
  "portfolioAlias": "FIRE",
  "fiscalYear": "2081-82",
  "effectiveDate": "2024-04-01T00:00:00Z",
  "expiryDate": "2025-03-31T23:59:59Z",
  "sumInsured": 1000000,
  "fullSumInsured": 1000000,
  "policyPeriodInDays": 365,
  "motorPartial": null,
  "firePartial": null,
  ...
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "sumInsuredAmount": 1000000,
    "basicPremium": 5000,
    "grossPremiumAmount": 5650,
    "days": 365,
    "numberofPassengers": 0,
    "ageOfVehicle": 0,
    ...
  }
}
```

---

#### Calculate Endorsement Premium
**POST** `/api/v1/premiums/calculate-endorsement`

**Request Body:**
```json
{
  "endorsementModel": {
    "portfolioAlias": "FIRE",
    "fiscalYear": "2081-82",
    "effectiveDate": "2024-04-01T00:00:00Z",
    ...
  },
  "premiumCalculation": {
    "sumInsuredAmount": 1000000,
    "basicPremium": 5000,
    "grossPremiumAmount": 5650,
    ...
  }
}
```

**Response:** Adjusted premium calculation result

---

### 3.2 Admin Configuration Endpoints

#### Get All Configurations
**GET** `/api/v1/admin/premium-calculation/configurations`

**Query Parameters:**
- `portfolioAlias` (string, optional)
- `fiscalYear` (string, optional)

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "config-123",
      "portfolioAlias": "FIRE",
      "fiscalYear": "2081-82",
      "version": 1,
      "effectiveFrom": "2024-04-01",
      "effectiveTo": "2025-03-31",
      "isActive": true,
      "description": "Fire Insurance Configuration",
      "calculationEngineType": "FormulaBased",
      "createdOn": "2024-01-15T10:30:00Z",
      "parameters": [ ... ],
      "rules": [ ... ],
      "rateTables": [ ... ]
    }
  ]
}
```

---

#### Get Configuration by ID
**GET** `/api/v1/admin/premium-calculation/configurations/{id}`

**Response:** Single configuration object

---

#### Get Configuration by Portfolio and Fiscal Year
**GET** `/api/v1/admin/premium-calculation/configurations/portfolio/{portfolioAlias}/fiscal-year/{fiscalYear}`

**Query Parameters:**
- `effectiveDate` (DateTime, optional)

**Response:** Single configuration object

---

#### Create Configuration
**POST** `/api/v1/admin/premium-calculation/configurations`

**Request Body:**
```json
{
  "portfolioAlias": "FIRE",
  "fiscalYear": "2081-82",
  "effectiveFrom": "2024-04-01",
  "effectiveTo": "2025-03-31",
  "description": "Fire Insurance Configuration",
  "calculationEngineType": "FormulaBased"
}
```

**Response:** Created configuration object

---

#### Update Configuration
**PUT** `/api/v1/admin/premium-calculation/configurations/{id}`

**Request Body:**
```json
{
  "effectiveFrom": "2024-04-01",
  "effectiveTo": "2025-03-31",
  "description": "Updated description",
  "calculationEngineType": "FormulaBased"
}
```

**Response:** Updated configuration object

---

#### Delete Configuration
**DELETE** `/api/v1/admin/premium-calculation/configurations/{id}`

**Response:**
```json
{
  "isSuccess": true,
  "data": true
}
```

---

#### Activate Configuration
**POST** `/api/v1/admin/premium-calculation/configurations/{id}/activate`

**Request Body:**
```json
"2081-82"
```

**Response:**
```json
{
  "isSuccess": true,
  "data": true
}
```

---

#### Clone Configuration
**POST** `/api/v1/admin/premium-calculation/configurations/{id}/clone`

**Request Body:**
```json
"2082-83"
```

**Response:** Cloned configuration object

---

#### Validate Configuration
**POST** `/api/v1/admin/premium-calculation/configurations/{id}/validate`

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "isValid": true,
    "errors": []
  }
}
```

---

### 3.3 Admin Parameter Endpoints

#### Get Parameters by Configuration
**GET** `/api/v1/admin/premium-calculation/parameters/configuration/{configurationId}`

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "param-123",
      "configurationId": "config-123",
      "parameterKey": "BasicPremiumRate",
      "parameterName": "Basic Premium Rate (%)",
      "dataType": "decimal",
      "value": "0.5",
      "defaultValue": "0.5",
      "minValue": 0.1,
      "maxValue": 10.0,
      "isRequired": true,
      "displayOrder": 1,
      "category": "Basic"
    }
  ]
}
```

---

#### Get Parameter by ID
**GET** `/api/v1/admin/premium-calculation/parameters/{id}`

**Response:** Single parameter object

---

#### Create Parameter
**POST** `/api/v1/admin/premium-calculation/parameters/configuration/{configurationId}`

**Request Body:**
```json
{
  "parameterKey": "BasicPremiumRate",
  "parameterName": "Basic Premium Rate (%)",
  "dataType": "decimal",
  "value": "0.5",
  "defaultValue": "0.5",
  "minValue": 0.1,
  "maxValue": 10.0,
  "isRequired": true,
  "displayOrder": 1,
  "category": "Basic"
}
```

**Response:** Created parameter object

---

#### Update Parameter
**PUT** `/api/v1/admin/premium-calculation/parameters/{id}`

**Request Body:**
```json
{
  "parameterName": "Updated Name",
  "dataType": "decimal",
  "value": "0.6",
  "defaultValue": "0.6",
  "minValue": 0.1,
  "maxValue": 10.0,
  "isRequired": true,
  "displayOrder": 1,
  "category": "Basic"
}
```

**Response:** Updated parameter object

---

#### Delete Parameter
**DELETE** `/api/v1/admin/premium-calculation/parameters/{id}`

**Response:**
```json
{
  "isSuccess": true,
  "data": true
}
```

---

#### Bulk Update Parameters
**POST** `/api/v1/admin/premium-calculation/parameters/configuration/{configurationId}/bulk`

**Request Body:**
```json
[
  {
    "parameterKey": "BasicPremiumRate",
    "parameterName": "Basic Premium Rate (%)",
    "dataType": "decimal",
    "value": "0.5",
    "defaultValue": "0.5",
    "minValue": 0.1,
    "maxValue": 10.0,
    "isRequired": true,
    "displayOrder": 1,
    "category": "Basic"
  }
]
```

**Response:**
```json
{
  "isSuccess": true,
  "data": true
}
```

---

### 3.4 Admin Rule Endpoints

#### Get Rules by Configuration
**GET** `/api/v1/admin/premium-calculation/rules/configuration/{configurationId}`

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "rule-123",
      "configurationId": "config-123",
      "ruleName": "Calculate Basic Premium",
      "ruleType": "Formula",
      "expression": "SumInsured * BasicPremiumRate / 100",
      "condition": null,
      "priority": 1,
      "isActive": true
    }
  ]
}
```

---

#### Get Rule by ID
**GET** `/api/v1/admin/premium-calculation/rules/{id}`

**Response:** Single rule object

---

#### Create Rule
**POST** `/api/v1/admin/premium-calculation/rules/configuration/{configurationId}`

**Request Body:**
```json
{
  "ruleName": "Calculate Basic Premium",
  "ruleType": "Formula",
  "expression": "SumInsured * BasicPremiumRate / 100",
  "condition": null,
  "priority": 1,
  "isActive": true
}
```

**Valid Rule Types:**
- `Formula`
- `Condition`
- `Validation`
- `Discount`

**Response:** Created rule object

---

#### Update Rule
**PUT** `/api/v1/admin/premium-calculation/rules/{id}`

**Request Body:**
```json
{
  "ruleName": "Updated Rule Name",
  "ruleType": "Formula",
  "expression": "SumInsured * BasicPremiumRate / 100",
  "condition": "{\"Days\": {\"$lt\": 365}}",
  "priority": 2,
  "isActive": true
}
```

**Response:** Updated rule object

---

#### Delete Rule
**DELETE** `/api/v1/admin/premium-calculation/rules/{id}`

**Response:**
```json
{
  "isSuccess": true,
  "data": true
}
```

---

### 3.5 Admin Rate Table Endpoints

#### Get Rate Tables by Configuration
**GET** `/api/v1/admin/premium-calculation/rate-tables/configuration/{configurationId}`

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "table-123",
      "configurationId": "config-123",
      "tableName": "AgeBasedRates",
      "schemaJson": "{\"Age\":\"int\",\"Rate\":\"decimal\"}",
      "dataJson": "[{\"Age\":18,\"Rate\":0.5},{\"Age\":30,\"Rate\":0.6}]",
      "lookupKey": "Age"
    }
  ]
}
```

---

#### Get Rate Table by ID
**GET** `/api/v1/admin/premium-calculation/rate-tables/{id}`

**Response:** Single rate table object

---

#### Create Rate Table
**POST** `/api/v1/admin/premium-calculation/rate-tables/configuration/{configurationId}`

**Request Body:**
```json
{
  "tableName": "AgeBasedRates",
  "schemaJson": "{\"Age\":\"int\",\"Rate\":\"decimal\"}",
  "dataJson": "[{\"Age\":18,\"Rate\":0.5},{\"Age\":30,\"Rate\":0.6},{\"Age\":50,\"Rate\":0.7}]",
  "lookupKey": "Age"
}
```

**Response:** Created rate table object

---

#### Update Rate Table
**PUT** `/api/v1/admin/premium-calculation/rate-tables/{id}`

**Request Body:**
```json
{
  "tableName": "AgeBasedRates",
  "schemaJson": "{\"Age\":\"int\",\"Rate\":\"decimal\"}",
  "dataJson": "[{\"Age\":18,\"Rate\":0.5},{\"Age\":30,\"Rate\":0.6}]",
  "lookupKey": "Age"
}
```

**Response:** Updated rate table object

---

#### Delete Rate Table
**DELETE** `/api/v1/admin/premium-calculation/rate-tables/{id}`

**Response:**
```json
{
  "isSuccess": true,
  "data": true
}
```

---

## 4. Notification Engine

### 4.1 Common Endpoints

#### Get My Notifications
**GET** `/api/v1/notifications`

**Query Parameters:**
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10)
- `query` (string, optional)
- `filters` (string, optional)
- `sorts` (string, optional)

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "notif-123",
      "userId": "user-456",
      "title": "New Lead Assigned",
      "body": "You have been assigned a new lead",
      "channel": "Push",
      "payload": "{\"leadId\":\"lead-123\"}",
      "sentAt": "2024-01-15T10:30:00Z",
      "readAt": null,
      "isRead": false
    }
  ],
  "pagination": { ... }
}
```

---

#### Mark Notification as Read
**PATCH** `/api/v1/notifications/{id}/read`

**Response:**
```json
{
  "isSuccess": true,
  "data": true
}
```

---

## 5. Attendance Engine

### 5.1 Common Endpoints

#### Check In
**POST** `/api/v1/attendance/check-in`

**Request Body:**
```json
{
  "latitude": 27.7172,
  "longitude": 85.3240,
  "remarks": "Office check-in"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": "attendance-123",
    "userId": "user-456",
    "type": "CheckIn",
    "latitude": 27.7172,
    "longitude": 85.3240,
    "timestamp": "2024-01-15T09:00:00Z",
    "remarks": "Office check-in"
  }
}
```

---

#### Check Out
**POST** `/api/v1/attendance/check-out`

**Request Body:**
```json
{
  "latitude": 27.7172,
  "longitude": 85.3240,
  "remarks": "Office check-out"
}
```

**Response:** Similar to Check In with `type: "CheckOut"`

---

#### Get Daily Attendance
**GET** `/api/v1/attendance/daily`

**Query Parameters:**
- `date` (DateTime, optional) - Default: today

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "date": "2024-01-15",
    "checkIn": {
      "timestamp": "2024-01-15T09:00:00Z",
      "latitude": 27.7172,
      "longitude": 85.3240
    },
    "checkOut": {
      "timestamp": "2024-01-15T18:00:00Z",
      "latitude": 27.7172,
      "longitude": 85.3240
    },
    "totalHours": 9.0
  }
}
```

---

#### Get Monthly Attendance
**GET** `/api/v1/attendance/monthly`

**Query Parameters:**
- `year` (int, optional) - Default: current year
- `month` (int, optional) - Default: current month

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "year": 2024,
    "month": 1,
    "totalDays": 22,
    "presentDays": 20,
    "absentDays": 2,
    "dailyRecords": [
      {
        "date": "2024-01-15",
        "checkIn": "2024-01-15T09:00:00Z",
        "checkOut": "2024-01-15T18:00:00Z",
        "totalHours": 9.0
      }
    ]
  }
}
```

---

## 6. Renewal Engine

### 6.1 Common Endpoints

#### Trigger Renewal Reminders (Admin Only)
**POST** `/api/v1/renewals/trigger-reminders`

**Response:**
```json
{
  "message": "Reminders triggered"
}
```

---

#### Get My Renewal Reminders
**GET** `/api/v1/renewals/my-reminders`

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "reminder-123",
      "policyId": "policy-456",
      "dueDate": "2024-02-15",
      "premiumAmount": 50000,
      "reminderSentAt": "2024-01-15T10:30:00Z",
      "channel": "Email"
    }
  ]
}
```

---

## Common Models

### Pagination Request Model
```typescript
interface CommonPaginationRequestModel {
  pageNumber: number;      // Default: 1
  pageSize: number;         // Default: 10, Max: 600
  query?: string;           // Search query
  filters?: string;         // Sieve filters
  sorts?: string;           // Sort order
}
```

### Pagination Response
```typescript
interface Pagination {
  totalItems: number;
  totalPages: number;
  pageSize: number;
  currentPage: number;
}
```

### Standard API Response
```typescript
interface ApiResponse<T> {
  isSuccess: boolean;
  data?: T;
  error?: string;
  errorCode?: number;
  pagination?: Pagination;
}
```

### Error Response
```typescript
interface ErrorResponse {
  isSuccess: false;
  error: string;
  errorCode: number;
}
```

---

## Authentication

All endpoints (except public endpoints) require authentication via Bearer Token:

**Header:**
```
Authorization: Bearer {access_token}
```

---

## Error Codes

- `400` - Bad Request (validation errors)
- `401` - Unauthorized (missing/invalid token)
- `403` - Forbidden (insufficient permissions)
- `404` - Not Found (resource doesn't exist)
- `500` - Internal Server Error

---

## Notes for Frontend Implementation

1. **Base URL**: Use environment variable for API base URL
2. **Authentication**: Store JWT token and include in all requests
3. **Error Handling**: Check `isSuccess` field in all responses
4. **Pagination**: Use `CommonPaginationRequestModel` for list endpoints
5. **Date Format**: Use ISO 8601 format for dates: `2024-01-15T10:30:00Z`
6. **DateOnly Format**: Use `YYYY-MM-DD` format: `2024-01-15`
7. **Multi-tenant**: Tenant context is automatically handled by backend
8. **Role-based Access**: Some endpoints require Admin/SuperAdmin roles

---

## Next.js Implementation Example

```typescript
// lib/api.ts
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://api.beemaedge.com/api/v1';

export async function apiRequest<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<ApiResponse<T>> {
  const token = localStorage.getItem('access_token');
  
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
      ...options.headers,
    },
  });
  
  return response.json();
}

// Usage
const leads = await apiRequest<LeadResponseDto[]>('/leads', {
  method: 'GET',
});
```

---

**Last Updated**: 2024-01-15  
**Version**: 1.0

