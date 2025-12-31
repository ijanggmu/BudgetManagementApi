# BeemaEdge API Endpoints Documentation

**Base URL:** `/api/v1`

**Authentication:** All endpoints (except those marked `[AllowAnonymous]`) require JWT Bearer token authentication.

---

## Table of Contents
1. [Admin Authentication](#admin-authentication)
2. [Admin Management](#admin-management)
3. [Tenant Management](#tenant-management)
4. [Role Management](#role-management)
5. [Lead Management](#lead-management)
6. [Quotation Management](#quotation-management)
7. [FoDo Management](#fodo-management)
8. [System Logs](#system-logs)
9. [Admin Profile](#admin-profile)
10. [Password Management](#password-management)
11. [Two-Factor Authentication](#two-factor-authentication)
12. [Menu Permissions](#menu-permissions)

---

## Admin Authentication

**Controller:** `AdminAuthController`  
**Base Route:** `/api/v1/AdminAuth`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/login` | Admin login | No |
| POST | `/login2FA` | Admin login with 2FA verification | No |
| POST | `/refresh` | Refresh authentication token | Yes |
| POST | `/logout` | Logout current session | Yes |

---

## Admin Management

**Controller:** `AdminController`  
**Base Route:** `/api/v1/Admin`

| Method | Endpoint | Description | Auth Required | Notes |
|--------|----------|-------------|---------------|-------|
| GET | `/` | Get all admins | Yes | Query param: `tenantId` (SuperAdmin only) |
| GET | `/{id}` | Get admin by ID | Yes | |
| POST | `/` | Create new admin | Yes | |
| PUT | `/{id}` | Update admin | Yes | |
| DELETE | `/{id}` | Delete admin (soft delete) | Yes | |

**Note:** 
- **Tenant Admin**: Only sees admins from their tenant
- **SuperAdmin**: Sees all admins across all tenants (can filter by `tenantId` query param)

---

## Tenant Management

**Controller:** `AdminTenantController`  
**Base Route:** `/api/v1/AdminTenant`

| Method | Endpoint | Description | Auth Required | Notes |
|--------|----------|-------------|---------------|-------|
| GET | `/` | Get all tenants (paginated) | Yes | Query param: `CommonPaginationRequestModel` |
| GET | `/{id}` | Get tenant by ID | Yes | |
| POST | `/` | Create new tenant | Yes | SuperAdmin only |
| PATCH | `/{id}` | Update tenant | Yes | SuperAdmin only |
| DELETE | `/{id}` | Delete tenant | Yes | SuperAdmin only |
| GET | `/dropdown` | Get active tenants for dropdown | Yes | Returns: Id, Name, Slug |

---

## Role Management

**Controller:** `AdminRoleController`  
**Base Route:** `/api/v1/AdminRole`

| Method | Endpoint | Description | Auth Required | Notes |
|--------|----------|-------------|---------------|-------|
| GET | `/types` | Get all system role types | Yes | Returns: Agent, Admin, FoDo |
| GET | `/names` | Get all role names | Yes | |
| POST | `/` | Get all roles (paginated) | Yes | Body: `CommonPaginationRequestModel` |
| POST | `/create` | Create new role | Yes | |
| GET | `/{id}` | Get role by ID | Yes | |
| PUT | `/{id}` | Update role | Yes | |
| DELETE | `/{id}` | Delete role | Yes | |

---

## Lead Management

**Controller:** `AdminLeadController`  
**Base Route:** `/api/v1/AdminLead`

| Method | Endpoint | Description | Auth Required | Notes |
|--------|----------|-------------|---------------|-------|
| GET | `/` | Get all leads (paginated) | Yes | Query params: `requestModel`, `status`, `from`, `to` |
| GET | `/{id}` | Get lead details by ID | Yes | Includes prospect and contact info |
| GET | `/tenant/{tenantId}` | Get leads by tenant ID | Yes | **SuperAdmin only** |

**Query Parameters:**
- `requestModel`: `CommonPaginationRequestModel` (pagination & Sieve filters)
- `status`: Optional - Filter by status (New, Qualified, Contacted, Quoted, Won, Lost)
- `from`: Optional - Filter leads created from this date
- `to`: Optional - Filter leads created until this date

**Note:**
- **Tenant Admin**: Only sees leads from their tenant
- **SuperAdmin**: Sees all leads across all tenants

---

## Quotation Management

**Controller:** `AdminQuotationController`  
**Base Route:** `/api/v1/AdminQuotation`

| Method | Endpoint | Description | Auth Required | Notes |
|--------|----------|-------------|---------------|-------|
| GET | `/` | Get all quotations (paginated) | Yes | Query params: `requestModel`, `status`, `from`, `to` |
| GET | `/{id}` | Get quotation details by ID | Yes | Includes quotation items |
| GET | `/tenant/{tenantId}` | Get quotations by tenant ID | Yes | **SuperAdmin only** |

**Query Parameters:**
- `requestModel`: `CommonPaginationRequestModel` (pagination & Sieve filters)
- `status`: Optional - Filter by status (Draft, Submitted, Approved, Declined, Accepted)
- `from`: Optional - Filter quotations created from this date
- `to`: Optional - Filter quotations created until this date

**Note:**
- **Tenant Admin**: Only sees quotations from their tenant
- **SuperAdmin**: Sees all quotations across all tenants

---

## FoDo Management

**Controller:** `AdminFodoController`  
**Base Route:** `/api/v1/AdminFodo`

| Method | Endpoint | Description | Auth Required | Notes |
|--------|----------|-------------|---------------|-------|
| GET | `/` | Get all fodos (Field Officer/Door Office Marketing) | Yes | Query param: `tenantId` (SuperAdmin only) |
| GET | `/{id}` | Get fodo by ID | Yes | |
| POST | `/` | Create new fodo | Yes | |
| PUT | `/{id}` | Update fodo | Yes | |
| DELETE | `/{id}` | Delete fodo (soft delete) | Yes | |

**Note:**
- **Tenant Admin**: Only sees fodos from their tenant
- **SuperAdmin**: Sees all fodos across all tenants (can filter by `tenantId` query param)

---

## System Logs

**Controller:** `SystemLogController`  
**Base Route:** `/api/v1/SystemLog`

| Method | Endpoint | Description | Auth Required | Notes |
|--------|----------|-------------|---------------|-------|
| GET | `/access` | Get system access logs (paginated) | Yes | Query param: `CommonPaginationRequestModel` |

**Response includes:**
- UserName, IpAddress, RequestPath, RequestMethod
- ResponseStatusCode, ResponseTimeInMS
- CorrelationId, Module, RequestHeader, RequestBody, ResponseBody
- At, EndAt, RequestHost, UserAgent, RequestQueryString

---

## Admin Profile

**Controller:** `AdminProfileController`  
**Base Route:** `/api/v1/AdminProfile`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get current admin user profile | Yes |
| PUT | `/` | Update admin user profile | Yes |

---

## Password Management

**Controller:** `AdminPasswordController`  
**Base Route:** `/api/v1/AdminPassword`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| PUT | `/change` | Change password for authenticated admin | Yes |
| POST | `/forget` | Request password reset OTP | No |
| POST | `/set` | Set password for user without password | Yes |
| POST | `/reset` | Reset password using OTP token | No |

---

## Two-Factor Authentication

**Controller:** `AdminTwoFactorController`  
**Base Route:** `/api/v1/AdminTwoFactor`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/Setup` | Setup 2FA for admin user | Yes |
| PUT | `/Disable` | Disable 2FA for admin user | Yes |
| GET | `/Verify/{code}` | Verify 2FA code | Yes |
| GET | `/BackupCodes` | Get all 2FA backup codes | Yes |

---

## Menu Permissions

**Controller:** `AdminMenuPermissionController`  
**Base Route:** `/api/v1/AdminMenuPermission`

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/GetMenu` | Get all menu items with permissions | Yes |
| GET | `/role/{roleId}` | Get menu permissions by role ID | Yes |
| POST | `/` | Assign permissions to a role | Yes |

---

## Common Request/Response Models

### CommonPaginationRequestModel
```json
{
  "pageNumber": 1,
  "pageSize": 10,
  "sorts": "fieldName",
  "filters": "fieldName@=value"
}
```

### Success Response Format
```json
{
  "isSuccess": true,
  "data": {},
  "pagination": {
    "totalItems": 100,
    "totalPages": 10,
    "pageSize": 10,
    "currentPage": 1
  }
}
```

### Error Response Format
```json
{
  "isSuccess": false,
  "error": "Error message",
  "errorCode": "ERROR_CODE"
}
```

---

## Role-Based Access Control

### SuperAdmin
- Can access **all tenants** data
- Can create/manage tenants
- Can filter by `tenantId` in admin/fodo endpoints
- Can access tenant-specific endpoints (e.g., `/tenant/{tenantId}`)

### Tenant Admin
- Can only access data from **their tenant**
- Cannot create/manage tenants
- Cannot filter by `tenantId`
- Cannot access tenant-specific endpoints

---

## Notes

1. **Base Route Pattern**: All controllers follow the pattern `/api/v1/{ControllerName}` (without "Controller" suffix)
2. **Pagination**: Most list endpoints support `CommonPaginationRequestModel` for pagination and Sieve filtering
3. **Soft Delete**: Delete operations are soft deletes (sets `IsDeleted = true`)
4. **Tenant Filtering**: Entities with `TenantId` are automatically filtered by tenant context (except for SuperAdmin)
5. **Query vs Body**: 
   - GET requests use `[FromQuery]` for parameters
   - POST/PUT requests use `[FromBody]` for request models

---

## Endpoint Summary by HTTP Method

### GET Endpoints (Read Operations)
- `GET /api/v1/Admin` - List admins
- `GET /api/v1/Admin/{id}` - Get admin by ID
- `GET /api/v1/AdminTenant` - List tenants
- `GET /api/v1/AdminTenant/{id}` - Get tenant by ID
- `GET /api/v1/AdminTenant/dropdown` - Get tenants for dropdown
- `GET /api/v1/AdminRole/types` - Get role types
- `GET /api/v1/AdminRole/names` - Get role names
- `GET /api/v1/AdminRole/{id}` - Get role by ID
- `GET /api/v1/AdminLead` - List leads
- `GET /api/v1/AdminLead/{id}` - Get lead by ID
- `GET /api/v1/AdminLead/tenant/{tenantId}` - Get leads by tenant (SuperAdmin only)
- `GET /api/v1/AdminQuotation` - List quotations
- `GET /api/v1/AdminQuotation/{id}` - Get quotation by ID
- `GET /api/v1/AdminQuotation/tenant/{tenantId}` - Get quotations by tenant (SuperAdmin only)
- `GET /api/v1/AdminFodo` - List fodos
- `GET /api/v1/AdminFodo/{id}` - Get fodo by ID
- `GET /api/v1/SystemLog/access` - Get access logs
- `GET /api/v1/AdminProfile` - Get admin profile
- `GET /api/v1/AdminTwoFactor/Setup` - Setup 2FA
- `GET /api/v1/AdminTwoFactor/Verify/{code}` - Verify 2FA code
- `GET /api/v1/AdminTwoFactor/BackupCodes` - Get backup codes
- `GET /api/v1/AdminMenuPermission/GetMenu` - Get menu
- `GET /api/v1/AdminMenuPermission/role/{roleId}` - Get permissions by role

### POST Endpoints (Create Operations)
- `POST /api/v1/AdminAuth/login` - Admin login
- `POST /api/v1/AdminAuth/login2FA` - Admin login with 2FA
- `POST /api/v1/AdminAuth/refresh` - Refresh token
- `POST /api/v1/AdminAuth/logout` - Logout
- `POST /api/v1/Admin` - Create admin
- `POST /api/v1/AdminTenant` - Create tenant (SuperAdmin only)
- `POST /api/v1/AdminRole` - List roles (with pagination)
- `POST /api/v1/AdminRole/create` - Create role
- `POST /api/v1/AdminPassword/forget` - Request password reset
- `POST /api/v1/AdminPassword/set` - Set password
- `POST /api/v1/AdminPassword/reset` - Reset password with OTP
- `POST /api/v1/AdminMenuPermission` - Assign permissions

### PUT Endpoints (Update Operations)
- `PUT /api/v1/Admin/{id}` - Update admin
- `PUT /api/v1/AdminRole/{id}` - Update role
- `PUT /api/v1/AdminFodo/{id}` - Update fodo
- `PUT /api/v1/AdminProfile` - Update admin profile
- `PUT /api/v1/AdminPassword/change` - Change password
- `PUT /api/v1/AdminTwoFactor/Disable` - Disable 2FA

### PATCH Endpoints (Partial Update)
- `PATCH /api/v1/AdminTenant/{id}` - Update tenant (SuperAdmin only)

### DELETE Endpoints (Delete Operations)
- `DELETE /api/v1/Admin/{id}` - Delete admin
- `DELETE /api/v1/AdminTenant/{id}` - Delete tenant (SuperAdmin only)
- `DELETE /api/v1/AdminRole/{id}` - Delete role
- `DELETE /api/v1/AdminFodo/{id}` - Delete fodo

---

**Last Updated:** Generated from codebase analysis  
**Version:** 1.0

