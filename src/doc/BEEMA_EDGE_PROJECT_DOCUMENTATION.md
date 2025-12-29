# BeemaEdge - Complete Project Documentation

**Version:** 1.0  
**Last Updated:** 2024  
**Project Manager Documentation**

---

## Table of Contents

1. [System Overview](#system-overview)
2. [Role-Based Architecture](#role-based-architecture)
3. [SuperAdmin Module Documentation](#superadmin-module-documentation)
4. [Tenant Admin Module Documentation](#tenant-admin-module-documentation)
5. [Marketing Executive Module Documentation](#marketing-executive-module-documentation)
6. [API Endpoints - Complete Reference](#api-endpoints---complete-reference)
7. [Request/Response Models](#requestresponse-models)
8. [Frontend Implementation Guide](#frontend-implementation-guide)
9. [Menu Structure Recommendations](#menu-structure-recommendations)

---

## System Overview

BeemaEdge is a multi-tenant insurance management system with role-based access control. The system supports three main roles:

- **SuperAdmin**: System-wide administration across all tenants
- **Tenant Admin (Admin)**: Tenant-specific administration
- **Marketing Executive (FoDo)**: Field operations and lead management

### Key Features
- Multi-tenant architecture with tenant isolation
- Role-based permission system
- Lead and quotation management
- Premium calculation engine
- Attendance tracking
- Notification system
- Comprehensive reporting

---

## Role-Based Architecture

### Role Hierarchy

```
SuperAdmin (Level 999)
  └─ Can access all tenants
  └─ Can create/manage tenants
  └─ Can view all data across tenants

Tenant Admin (Level 500)
  └─ Tenant-specific access
  └─ Manages tenant users and settings
  └─ Views tenant-specific data only

Marketing Executive (Level 300)
  └─ Own leads and quotations
  └─ Attendance tracking
  └─ Limited to assigned data
```

### Permission System

The system uses a hierarchical permission structure:
- Format: `ParentMenuId-ChildMenuId-OperationType`
- Operations: View (1), Create (2), Update (3), Delete (4), Export (5)
- Permissions are assigned to roles
- Users inherit permissions through their roles

---

## SuperAdmin Module Documentation

### Overview
SuperAdmin has system-wide access and can manage all tenants, admins, roles, and view aggregated data across all tenants.

### Modules and Tasks

#### 1. Dashboard
**Description:** System-wide statistics overview

**Metrics:**
- TotalTenant: Count of all active tenants
- TotalLeads: Count of all leads across all tenants
- TotalQuotation: Count of all quotations across all tenants
- TotalAdmin: Count of all admin users across all tenants

**API Endpoints:**
- `GET /api/v1/reporting/dashboard` - Get dashboard statistics

**Frontend Requirements:**
- Display metrics in cards/widgets
- Show trends (optional: charts)
- Real-time or near-real-time updates

---

#### 2. Administration

##### 2.1 Tenant Management
**Description:** CRUD operations for tenants

**Tasks:**
- Create Tenant
- View All Tenants (paginated)
- View Tenant Details
- Update Tenant
- Delete Tenant (soft delete)
- View Tenant Dropdown (for filters)

**API Endpoints:**
- `GET /api/v1/AdminTenant` - List all tenants (paginated)
- `GET /api/v1/AdminTenant/{id}` - Get tenant by ID
- `POST /api/v1/AdminTenant` - Create new tenant
- `PATCH /api/v1/AdminTenant/{id}` - Update tenant
- `DELETE /api/v1/AdminTenant/{id}` - Delete tenant
- `GET /api/v1/AdminTenant/dropdown` - Get tenants for dropdown

**Request Models:**
```typescript
// Create Tenant
interface CreateTenantDto {
  name: string;
  slug: string;
  companyBranding: {
    logoUrl?: string;
    paletteJson?: string;
    typographyJson?: string;
    version?: number;
  };
  adminUser: {
    email: string;
    username: string;
    fullName: string;
    password: string;
  };
  isActive?: boolean;
  themeVersion?: number;
}

// Update Tenant
interface UpdateTenantDto {
  name?: string;
  slug?: string;
  isActive?: boolean;
  themeVersion?: number;
}
```

**Response Models:**
```typescript
interface TenantResponseDto {
  id: string;
  name: string;
  slug: string;
  isActive: boolean;
  themeVersion: number;
  createdOn: string;
  branding?: BrandingResponseDto;
}
```

---

##### 2.2 Role Management
**Description:** CRUD operations for roles and permission management

**Tasks:**
- Create Role
- View All Roles (paginated)
- View Role Details
- Update Role
- Delete Role
- Manage Permissions for Role
- View Role Types
- View Role Names

**API Endpoints:**
- `POST /api/v1/AdminRole` - List all roles (paginated)
- `POST /api/v1/AdminRole/create` - Create new role
- `GET /api/v1/AdminRole/{id}` - Get role by ID
- `PUT /api/v1/AdminRole/{id}` - Update role
- `DELETE /api/v1/AdminRole/{id}` - Delete role
- `GET /api/v1/AdminRole/types` - Get all system role types
- `GET /api/v1/AdminRole/names` - Get all role names
- `GET /api/v1/AdminMenuPermission/GetMenu` - Get all menu items with permissions
- `GET /api/v1/AdminMenuPermission/role/{roleId}` - Get menu permissions by role ID
- `POST /api/v1/AdminMenuPermission` - Assign permissions to a role

**Request Models:**
```typescript
// Create Role
interface CreateRoleDto {
  name: string;
  description?: string;
  roleType: string; // "Agent" | "Admin" | "FoDo"
  roleLevel: number;
}

// Update Role
interface UpdateRoleDto {
  name?: string;
  description?: string;
  roleType?: string;
  roleLevel?: number;
}

// Assign Permissions
interface AssignPermissionsDto {
  roleId: string;
  permissions: string[]; // Array of permission strings like "18-12-1", "18-12-2"
}
```

---

##### 2.3 Admin Management
**Description:** CRUD operations for admin users

**Tasks:**
- Create Admin
- View All Admins (paginated, can filter by tenantId)
- View Admin Details
- Update Admin
- Delete Admin (soft delete)
- Change Tenant Admin Password

**API Endpoints:**
- `GET /api/v1/Admin` - List all admins (query param: `tenantId` for filtering)
- `GET /api/v1/Admin/{id}` - Get admin by ID
- `POST /api/v1/Admin` - Create new admin
- `PUT /api/v1/Admin/{id}` - Update admin
- `DELETE /api/v1/Admin/{id}` - Delete admin
- `PUT /api/v1/Admin/{id}/change-password` - Change admin password (if implemented)

**Request Models:**
```typescript
// Create Admin
interface CreateAdminDto {
  fullName: string;
  email: string;
  username: string;
  phoneNumber?: string;
  password: string;
  confirmPassword: string;
  roles?: string[];
}

// Update Admin
interface UpdateAdminDto {
  fullName?: string;
  email?: string;
  phoneNumber?: string;
  isDisabled?: boolean;
  roles?: string[];
}
```

**Response Models:**
```typescript
interface AdminResponseDto {
  id: string;
  fullName: string;
  email: string;
  phoneNumber?: string;
  username: string;
  userId: string;
  tenantId?: string;
  tenantName: string;
  roles: string[];
  isDisabled: boolean;
  emailConfirmed: boolean;
  createdOn: string;
}
```

---

#### 3. Sales and Marketing

##### 3.1 Marketing Executive Management
**Description:** CRUD operations for Marketing Executives (FoDo)

**Tasks:**
- Create Marketing Executive
- View All Marketing Executives (paginated, can filter by tenantId)
- View Marketing Executive Details
- Update Marketing Executive
- Delete Marketing Executive
- Change Marketing Executive Password
- Import Marketing Executives from Excel

**API Endpoints:**
- `POST /api/v1/AdminMarketingExecutive` - List all marketing executives (paginated)
- `GET /api/v1/AdminMarketingExecutive/{id}` - Get marketing executive by ID
- `POST /api/v1/AdminMarketingExecutive/create` - Create new marketing executive
- `PUT /api/v1/AdminMarketingExecutive/{id}` - Update marketing executive
- `DELETE /api/v1/AdminMarketingExecutive/{id}` - Delete marketing executive
- `PUT /api/v1/AdminMarketingExecutive/{id}/change-password` - Change password
- `PUT /api/v1/AdminMarketingExecutive/{id}/toggle-status` - Enable/disable user
- `GET /api/v1/AdminMarketingExecutive/{id}/access-logs` - Get access logs
- `GET /api/v1/AdminMarketingExecutive/{id}/leads` - Get leads for marketing executive
- `GET /api/v1/AdminMarketingExecutive/{id}/quotations` - Get quotations for marketing executive
- `POST /api/v1/AdminMarketingExecutive/import` - Import from Excel

---

##### 3.2 Leads Management
**Description:** View and manage all leads across tenants

**Tasks:**
- View All Leads (paginated, filterable by status, date range, tenant)
- View Lead Details
- View Leads by Tenant (SuperAdmin only)

**API Endpoints:**
- `GET /api/v1/AdminLead` - List all leads (query params: `requestModel`, `status`, `from`, `to`)
- `GET /api/v1/AdminLead/{id}` - Get lead details by ID
- `GET /api/v1/AdminLead/tenant/{tenantId}` - Get leads by tenant ID (SuperAdmin only)

**Query Parameters:**
- `requestModel`: CommonPaginationRequestModel (pagination & Sieve filters)
- `status`: Optional - Filter by status (New, Qualified, Contacted, Quoted, Won, Lost)
- `from`: Optional - Filter leads created from this date
- `to`: Optional - Filter leads created until this date

---

##### 3.3 Quotation Management
**Description:** View and manage all quotations across tenants

**Tasks:**
- View All Quotations (paginated, filterable by status, date range, tenant)
- View Quotation Details
- View Quotations by Tenant (SuperAdmin only)

**API Endpoints:**
- `GET /api/v1/AdminQuotation` - List all quotations (query params: `requestModel`, `status`, `from`, `to`)
- `GET /api/v1/AdminQuotation/{id}` - Get quotation details by ID
- `GET /api/v1/AdminQuotation/tenant/{tenantId}` - Get quotations by tenant ID (SuperAdmin only)

**Query Parameters:**
- `requestModel`: CommonPaginationRequestModel
- `status`: Optional - Filter by status (Draft, Submitted, Approved, Declined, Accepted)
- `from`: Optional - Filter quotations created from this date
- `to`: Optional - Filter quotations created until this date

---

#### 4. System

##### 4.1 System Access Logs
**Description:** View system access logs

**Tasks:**
- View System Access Logs (paginated)

**API Endpoints:**
- `GET /api/v1/SystemLog/access` - Get system access logs (query param: `CommonPaginationRequestModel`)

**Response includes:**
- UserName, IpAddress, RequestPath, RequestMethod
- ResponseStatusCode, ResponseTimeInMS
- CorrelationId, Module, RequestHeader, RequestBody, ResponseBody
- At, EndAt, RequestHost, UserAgent, RequestQueryString

---

#### 5. User Profile & Security

##### 5.1 Login
**API Endpoints:**
- `POST /api/v1/AdminAuth/login` - Admin login
- `POST /api/v1/AdminAuth/login2FA` - Admin login with 2FA verification
- `POST /api/v1/AdminAuth/refresh` - Refresh authentication token
- `POST /api/v1/AdminAuth/logout` - Logout current session

**Request Models:**
```typescript
interface LoginRequestDto {
  username: string;
  password: string;
  tenantSlug?: string; // For tenant-specific login
}

interface Login2FARequestDto {
  username: string;
  password: string;
  code: string; // 2FA code
  tenantSlug?: string;
}
```

**Response Models:**
```typescript
interface LoginResponseDto {
  accessToken: string;
  accessTokenExpiryInSeconds: number;
  refreshToken: string;
  refreshTokenExpiryInSeconds: number;
  userId: string;
  userName: string;
  roles: string[];
  tenantId?: string;
  tenantName?: string;
}
```

---

##### 5.2 Change Password
**API Endpoints:**
- `PUT /api/v1/AdminPassword/change` - Change password for authenticated admin

**Request Models:**
```typescript
interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}
```

---

##### 5.3 Profile
**API Endpoints:**
- `GET /api/v1/AdminProfile` - Get current admin user profile
- `PUT /api/v1/AdminProfile` - Update admin user profile

**Request Models:**
```typescript
interface UpdateProfileDto {
  fullName?: string;
  email?: string;
  phoneNumber?: string;
}
```

---

##### 5.4 Profile Update
Same as Profile section above.

---

## Tenant Admin Module Documentation

### Overview
Tenant Admin manages their specific tenant's operations, users, and data. They have access to tenant-specific data only.

### Modules and Tasks

#### 1. Dashboard
**Description:** Tenant-specific statistics overview

**Metrics:**
- TotalMarketingExecutives: Count of marketing executives in tenant
- Total Leads: Count of leads in tenant
- TotalQuotation: Count of quotations in tenant
- Log: Recent activity logs

**API Endpoints:**
- `GET /api/v1/reporting/dashboard` - Get dashboard statistics (tenant-filtered)

**Frontend Requirements:**
- Display tenant-specific metrics
- Show recent activity feed
- Quick access to common tasks

---

#### 2. Administration

##### 2.1 Organization Details
**Description:** Manage organization information, signation, and underwriting name

**Tasks:**
- View Organization Details
- Update Organization Details
- Manage Signation Information
- Manage Underwriting Name

**Note:** This may be part of Tenant settings or Entity settings. Check `AdminEntityController` or `AdminBrandingController`.

**API Endpoints:**
- `GET /api/v1/AdminEntity` - Get entity settings (if implemented)
- `PUT /api/v1/AdminEntity/{id}` - Update entity settings (if implemented)
- `GET /api/v1/AdminBranding` - Get branding/organization details
- `PUT /api/v1/AdminBranding` - Update branding/organization details

---

##### 2.2 Branch Management
**Description:** CRUD operations for branches

**Tasks:**
- Create Branch
- View All Branches (paginated)
- View Branch Details
- Update Branch
- Delete Branch
- Import Branches from Excel

**API Endpoints:**
- `POST /api/v1/AdminBranch` - List all branches (paginated)
- `GET /api/v1/AdminBranch/{id}` - Get branch by ID
- `POST /api/v1/AdminBranch/create` - Create new branch
- `PUT /api/v1/AdminBranch/{id}` - Update branch
- `DELETE /api/v1/AdminBranch/{id}` - Delete branch
- `POST /api/v1/AdminBranch/import` - Import from Excel

**Request Models:**
```typescript
interface CreateBranchDto {
  branchName: string;
  branchCode: string;
  province: string;
  district: string;
  municipality: string;
  ward: number;
  isActive?: boolean;
}

interface UpdateBranchDto {
  branchName?: string;
  branchCode?: string;
  province?: string;
  district?: string;
  municipality?: string;
  ward?: number;
  isActive?: boolean;
}
```

**Response Models:**
```typescript
interface BranchResponseDto {
  id: string;
  branchName: string;
  branchCode: string;
  province: string;
  district: string;
  municipality: string;
  ward: number;
  isActive: boolean;
  tenantId: string;
  createdOn: string;
}
```

---

##### 2.3 Designation Management
**Description:** CRUD operations for designations

**Tasks:**
- Create Designation
- View All Designations (paginated)
- View Designation Details
- Update Designation
- Delete Designation
- Import Designations from Excel

**API Endpoints:**
- `POST /api/v1/AdminDesignation` - List all designations (paginated)
- `GET /api/v1/AdminDesignation/{id}` - Get designation by ID
- `POST /api/v1/AdminDesignation/create` - Create new designation
- `PUT /api/v1/AdminDesignation/{id}` - Update designation
- `DELETE /api/v1/AdminDesignation/{id}` - Delete designation
- `POST /api/v1/AdminDesignation/import` - Import from Excel

**Request Models:**
```typescript
interface CreateDesignationDto {
  title: string;
  description?: string;
}

interface UpdateDesignationDto {
  title?: string;
  description?: string;
}
```

**Response Models:**
```typescript
interface DesignationResponseDto {
  id: string;
  title: string;
  description?: string;
  tenantId: string;
  createdOn: string;
}
```

---

##### 2.4 Marketing Executive Management
**Description:** CRUD operations for Marketing Executives within tenant

**Tasks:**
- Create Marketing Executive
- View All Marketing Executives (paginated, tenant-filtered)
- View Marketing Executive Details
- Update Marketing Executive
- Delete Marketing Executive
- Change Marketing Executive Password
- Import Marketing Executives from Excel

**API Endpoints:**
Same as SuperAdmin Marketing Executive Management, but data is automatically filtered by tenant.

---

##### 2.5 Role Management
**Description:** CRUD operations for roles within tenant

**Tasks:**
- Create Role
- View All Roles (paginated, tenant-filtered)
- View Role Details
- Update Role
- Delete Role
- Manage Permissions for Role

**API Endpoints:**
Same as SuperAdmin Role Management, but roles are tenant-scoped.

---

##### 2.6 Admin Management
**Description:** CRUD operations for admin users within tenant

**Tasks:**
- Create Admin
- View All Admins (paginated, tenant-filtered)
- View Admin Details
- Update Admin
- Delete Admin
- Change Tenant Admin Password

**API Endpoints:**
Same as SuperAdmin Admin Management, but data is automatically filtered by tenant.

---

#### 3. Reporting

##### 3.1 Lead Reporting
**Description:** Reports and analytics for leads

**Tasks:**
- View Lead Reports
- Export Lead Data
- Filter by date range, status, marketing executive

**API Endpoints:**
- `GET /api/v1/AdminLead` - List leads with filters (can be used for reporting)
- `GET /api/v1/reporting/leads` - Lead reports (if implemented)

---

##### 3.2 Quotation Reporting
**Description:** Reports and analytics for quotations

**Tasks:**
- View Quotation Reports
- Export Quotation Data
- Filter by date range, status, marketing executive

**API Endpoints:**
- `GET /api/v1/AdminQuotation` - List quotations with filters (can be used for reporting)
- `GET /api/v1/reporting/quotations` - Quotation reports (if implemented)

---

##### 3.3 Attendance Reporting
**Description:** Reports and analytics for attendance

**Tasks:**
- View Attendance Reports
- Export Attendance Data
- Filter by date range, marketing executive

**API Endpoints:**
- `GET /api/v1/AdminAttendance` - List attendance records (if implemented)
- `GET /api/v1/reporting/attendance` - Attendance reports (if implemented)

---

#### 4. System

##### 4.1 Logs
**Description:** View system logs for tenant

**Tasks:**
- View System Access Logs (tenant-filtered)

**API Endpoints:**
- `GET /api/v1/SystemLog/access` - Get system access logs (tenant-filtered)

---

##### 4.2 Notifications
**Description:** View and manage notifications

**Tasks:**
- View Notifications
- Mark Notification as Read
- View Notification History

**API Endpoints:**
- `GET /api/v1/notifications` - Get my notifications (paginated)
- `PATCH /api/v1/notifications/{id}/read` - Mark notification as read

**Request Models:**
```typescript
// Pagination is via query params
interface NotificationResponseDto {
  id: string;
  title: string;
  message: string;
  type: string;
  isRead: boolean;
  createdAt: string;
  userId: string;
}
```

---

##### 4.3 Notice Board
**Description:** View and manage notice board announcements

**Tasks:**
- View Notice Board
- Create Notice (if permission allows)
- Update Notice (if permission allows)
- Delete Notice (if permission allows)

**Note:** Notice Board functionality may need to be implemented. Check if there's a NoticeBoard entity or if it's part of Notifications.

**API Endpoints:**
- `GET /api/v1/noticeboard` - Get notice board items (if implemented)
- `POST /api/v1/noticeboard` - Create notice (if implemented)
- `PUT /api/v1/noticeboard/{id}` - Update notice (if implemented)
- `DELETE /api/v1/noticeboard/{id}` - Delete notice (if implemented)

---

#### 5. User Profile & Security

Same as SuperAdmin:
- Login
- Change Password
- Profile
- Profile Update

---

## Marketing Executive Module Documentation

### Overview
Marketing Executive manages their own leads, quotations, attendance, and views notifications. They have limited access to only their assigned data.

### Modules and Tasks

#### 1. Dashboard
**Description:** Personal analytics and funnel

**Metrics:**
- Analytics: Lead conversion rates, quotation success rates
- Funnel: Lead → Quotation → Won pipeline visualization

**API Endpoints:**
- `GET /api/v1/reporting/dashboard` - Get dashboard statistics (user-filtered)

**Frontend Requirements:**
- Display personal metrics
- Show conversion funnel
- Quick access to recent leads/quotations

---

#### 2. Lead Management
**Description:** CRUD operations for own leads

**Tasks:**
- Create Lead
- View All Leads (own leads only, paginated)
- View Lead Details
- Update Lead Status
- Add Lead Activity
- View Lead Activities

**API Endpoints:**
- `POST /api/v1/Lead` - Create new lead
- `GET /api/v1/Lead` - List my leads (query params: `requestModel`, `status`, `from`, `to`)
- `GET /api/v1/Lead/{id}` - Get lead by ID
- `PATCH /api/v1/Lead/{id}/status` - Update lead status
- `POST /api/v1/Lead/{id}/activities` - Add activity to lead
- `GET /api/v1/Lead/{id}/activities` - Get lead activities

**Request Models:**
```typescript
interface CreateLeadPublicDto {
  fullName: string;
  email: string;
  phone?: string;
  productCode: string;
}

interface UpdateLeadStatusDto {
  status: string; // "New" | "Qualified" | "Contacted" | "Quoted" | "Won" | "Lost"
}

interface LeadActivityDto {
  kind: string;
  notes: string;
}
```

**Response Models:**
```typescript
interface LeadResponseDto {
  id: string;
  prospectId: string;
  status: string;
  source: string;
  ownerUserId: string;
  createdOn: string;
  prospect?: ProspectResponseDto;
}

interface LeadActivityResponseDto {
  id: string;
  leadId: string;
  kind: string;
  notes: string;
  when: string;
}
```

---

#### 3. Quotation Management
**Description:** CRUD operations for own quotations

**Tasks:**
- Create Quotation
- View All Quotations (own quotations only, paginated)
- View Quotation Details
- Update Quotation
- Delete Quotation
- Generate Quotation PDF

**API Endpoints:**
- `POST /api/v1/Quotation` - Create new quotation
- `GET /api/v1/Quotation` - List my quotations (query param: `requestModel`)
- `GET /api/v1/Quotation/{id}` - Get quotation by ID
- `PATCH /api/v1/Quotation/{id}` - Update quotation
- `DELETE /api/v1/Quotation/{id}` - Delete quotation
- `GET /api/v1/Quotation/{id}/pdf` - Generate and download quotation PDF

**Request Models:**
```typescript
interface CreateQuotationDto {
  productId: string;
  prospectId: string;
  items: QuotationItemDto[];
}

interface QuotationItemDto {
  coverageId: string;
  sumInsured: number;
}

interface UpdateQuotationDto {
  status?: string;
  totalPremium?: number;
  discountPercent?: number;
  validUntil?: string; // DateOnly
  items?: QuotationItemDto[];
}
```

**Response Models:**
```typescript
interface QuotationResponseDto {
  id: string;
  number: string;
  status: string;
  productId: string;
  prospectId: string;
  totalPremium?: number;
  discountPercent?: number;
  validUntil?: string;
  pdfUrl?: string;
  createdOn: string;
  items: QuotationItemResponseDto[];
}

interface QuotationItemResponseDto {
  id: string;
  quotationId: string;
  coverageId: string;
  sumInsured: number;
  premium: number;
}
```

---

#### 4. Premium Calculator
**Description:** Interactive premium calculation UI

**Tasks:**
- Calculate Premium
- View Premium Breakdown
- Interactive Calculator UI

**API Endpoints:**
- `POST /api/v1/premiums/calculate` - Calculate premium

**Request Models:**
```typescript
interface PremiumCalculateRequestModel {
  productCode: string;
  age: number;
  sumAssured: number;
  term: number;
  paymentMode: string;
  riders?: string[];
  // Additional fields based on product requirements
}
```

**Response Models:**
```typescript
interface PremiumCalculateResponseModel {
  basePremium: number;
  riderPremiums: { [key: string]: number };
  taxes: number;
  totalPremium: number;
  breakdown: any; // Detailed breakdown
}
```

**Frontend Requirements:**
- Interactive form with real-time calculation
- Show premium breakdown
- Allow parameter adjustments
- Display results clearly

---

#### 5. Attendance
**Description:** Attendance tracking and management

**Tasks:**
- Check In
- Check Out
- View Attendance List
- View Daily Attendance
- View Monthly Attendance
- Check Today's Status

**API Endpoints:**
- `POST /api/v1/Attendance/check-in` - Check in for the day
- `POST /api/v1/Attendance/check-out` - Check out for the day
- `GET /api/v1/Attendance/daily` - Get daily attendance (query param: `date`)
- `GET /api/v1/Attendance/monthly` - Get monthly attendance (query params: `year`, `month`)
- `GET /api/v1/Attendance/today/status` - Check if checked in today

**Request Models:**
```typescript
interface AttendanceDto {
  latitude: number;
  longitude: number;
  remarks?: string;
}
```

**Response Models:**
```typescript
interface AttendanceResponseDto {
  id: string;
  userId: string;
  userName?: string;
  fullName?: string;
  type: string; // "CheckIn" | "CheckOut"
  latitude: number;
  longitude: number;
  timestamp: string;
  remarks?: string;
  tenantId?: string;
  tenantName?: string;
  createdOn: string;
}
```

**Frontend Requirements:**
- Location-based check-in/check-out
- Show attendance calendar
- Display today's status prominently
- Reminder if not checked in

---

#### 6. Notification & Notice Board
**Description:** View notifications and notice board

**Tasks:**
- View Notifications
- Mark Notification as Read
- View Notice Board

**API Endpoints:**
- `GET /api/v1/notifications` - Get my notifications (paginated)
- `PATCH /api/v1/notifications/{id}/read` - Mark notification as read
- `GET /api/v1/noticeboard` - Get notice board items (if implemented)

---

#### 7. Reminder on Every Login
**Description:** Reminder if not checked in

**Tasks:**
- Show reminder on login if not checked in today
- Quick check-in action from reminder

**Implementation:**
- Frontend: Check `GET /api/v1/Attendance/today/status` on login
- Show modal/notification if not checked in
- Provide quick check-in button

---

#### 8. User Profile & Security

Same as other roles:
- Login (Marketing Executive Auth)
- Change Password
- Profile
- Profile Update

**API Endpoints:**
- `POST /api/v1/MarketingExecutiveAuth/login` - Marketing Executive login
- `POST /api/v1/MarketingExecutiveAuth/login2FA` - Login with 2FA
- `PUT /api/v1/MarketingExecutivePassword/change` - Change password
- `GET /api/v1/MarketingExecutiveProfile` - Get profile
- `PUT /api/v1/MarketingExecutiveProfile` - Update profile

---

## API Endpoints - Complete Reference

### Base URL
```
/api/v1
```

### Authentication
All endpoints (except login) require JWT Bearer token:
```
Authorization: Bearer {token}
```

### Common Request Models

#### CommonPaginationRequestModel
```typescript
interface CommonPaginationRequestModel {
  pageNumber: number;
  pageSize: number;
  sorts?: string; // "fieldName" or "fieldName desc"
  filters?: string; // Sieve filter syntax: "fieldName@=value"
}
```

#### Success Response Format
```typescript
interface SuccessApiResponse<T> {
  isSuccess: true;
  data: T;
  pagination?: {
    totalItems: number;
    totalPages: number;
    pageSize: number;
    currentPage: number;
  };
}
```

#### Error Response Format
```typescript
interface ErrorApiResponse {
  isSuccess: false;
  error: string;
  errorCode: string;
}
```

---

### Endpoint Summary by Role

#### SuperAdmin Endpoints

| Module | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| Dashboard | GET | `/api/v1/reporting/dashboard` | Get dashboard stats |
| Tenant | GET | `/api/v1/AdminTenant` | List tenants |
| Tenant | GET | `/api/v1/AdminTenant/{id}` | Get tenant |
| Tenant | POST | `/api/v1/AdminTenant` | Create tenant |
| Tenant | PATCH | `/api/v1/AdminTenant/{id}` | Update tenant |
| Tenant | DELETE | `/api/v1/AdminTenant/{id}` | Delete tenant |
| Role | POST | `/api/v1/AdminRole` | List roles |
| Role | POST | `/api/v1/AdminRole/create` | Create role |
| Role | GET | `/api/v1/AdminRole/{id}` | Get role |
| Role | PUT | `/api/v1/AdminRole/{id}` | Update role |
| Role | DELETE | `/api/v1/AdminRole/{id}` | Delete role |
| Role | GET | `/api/v1/AdminMenuPermission/GetMenu` | Get menu |
| Role | GET | `/api/v1/AdminMenuPermission/role/{roleId}` | Get role permissions |
| Role | POST | `/api/v1/AdminMenuPermission` | Assign permissions |
| Admin | GET | `/api/v1/Admin` | List admins |
| Admin | GET | `/api/v1/Admin/{id}` | Get admin |
| Admin | POST | `/api/v1/Admin` | Create admin |
| Admin | PUT | `/api/v1/Admin/{id}` | Update admin |
| Admin | DELETE | `/api/v1/Admin/{id}` | Delete admin |
| Marketing Executive | POST | `/api/v1/AdminMarketingExecutive` | List marketing executives |
| Marketing Executive | GET | `/api/v1/AdminMarketingExecutive/{id}` | Get marketing executive |
| Marketing Executive | POST | `/api/v1/AdminMarketingExecutive/create` | Create marketing executive |
| Marketing Executive | PUT | `/api/v1/AdminMarketingExecutive/{id}` | Update marketing executive |
| Marketing Executive | DELETE | `/api/v1/AdminMarketingExecutive/{id}` | Delete marketing executive |
| Lead | GET | `/api/v1/AdminLead` | List leads |
| Lead | GET | `/api/v1/AdminLead/{id}` | Get lead |
| Lead | GET | `/api/v1/AdminLead/tenant/{tenantId}` | Get leads by tenant |
| Quotation | GET | `/api/v1/AdminQuotation` | List quotations |
| Quotation | GET | `/api/v1/AdminQuotation/{id}` | Get quotation |
| Quotation | GET | `/api/v1/AdminQuotation/tenant/{tenantId}` | Get quotations by tenant |
| System | GET | `/api/v1/SystemLog/access` | Get access logs |

#### Tenant Admin Endpoints

| Module | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| Dashboard | GET | `/api/v1/reporting/dashboard` | Get dashboard stats |
| Branch | POST | `/api/v1/AdminBranch` | List branches |
| Branch | GET | `/api/v1/AdminBranch/{id}` | Get branch |
| Branch | POST | `/api/v1/AdminBranch/create` | Create branch |
| Branch | PUT | `/api/v1/AdminBranch/{id}` | Update branch |
| Branch | DELETE | `/api/v1/AdminBranch/{id}` | Delete branch |
| Branch | POST | `/api/v1/AdminBranch/import` | Import branches |
| Designation | POST | `/api/v1/AdminDesignation` | List designations |
| Designation | GET | `/api/v1/AdminDesignation/{id}` | Get designation |
| Designation | POST | `/api/v1/AdminDesignation/create` | Create designation |
| Designation | PUT | `/api/v1/AdminDesignation/{id}` | Update designation |
| Designation | DELETE | `/api/v1/AdminDesignation/{id}` | Delete designation |
| Designation | POST | `/api/v1/AdminDesignation/import` | Import designations |
| Marketing Executive | POST | `/api/v1/AdminMarketingExecutive` | List marketing executives |
| Marketing Executive | GET | `/api/v1/AdminMarketingExecutive/{id}` | Get marketing executive |
| Marketing Executive | POST | `/api/v1/AdminMarketingExecutive/create` | Create marketing executive |
| Marketing Executive | PUT | `/api/v1/AdminMarketingExecutive/{id}` | Update marketing executive |
| Marketing Executive | DELETE | `/api/v1/AdminMarketingExecutive/{id}` | Delete marketing executive |
| Lead | GET | `/api/v1/AdminLead` | List leads (tenant-filtered) |
| Lead | GET | `/api/v1/AdminLead/{id}` | Get lead |
| Quotation | GET | `/api/v1/AdminQuotation` | List quotations (tenant-filtered) |
| Quotation | GET | `/api/v1/AdminQuotation/{id}` | Get quotation |
| Reporting | GET | `/api/v1/reporting/dashboard` | Get reports |
| System | GET | `/api/v1/SystemLog/access` | Get access logs |
| Notification | GET | `/api/v1/notifications` | Get notifications |
| Notification | PATCH | `/api/v1/notifications/{id}/read` | Mark as read |

#### Marketing Executive Endpoints

| Module | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| Dashboard | GET | `/api/v1/reporting/dashboard` | Get dashboard stats |
| Lead | POST | `/api/v1/Lead` | Create lead |
| Lead | GET | `/api/v1/Lead` | List my leads |
| Lead | GET | `/api/v1/Lead/{id}` | Get lead |
| Lead | PATCH | `/api/v1/Lead/{id}/status` | Update lead status |
| Lead | POST | `/api/v1/Lead/{id}/activities` | Add activity |
| Lead | GET | `/api/v1/Lead/{id}/activities` | Get activities |
| Quotation | POST | `/api/v1/Quotation` | Create quotation |
| Quotation | GET | `/api/v1/Quotation` | List my quotations |
| Quotation | GET | `/api/v1/Quotation/{id}` | Get quotation |
| Quotation | PATCH | `/api/v1/Quotation/{id}` | Update quotation |
| Quotation | DELETE | `/api/v1/Quotation/{id}` | Delete quotation |
| Quotation | GET | `/api/v1/Quotation/{id}/pdf` | Get PDF |
| Premium Calculator | POST | `/api/v1/premiums/calculate` | Calculate premium |
| Attendance | POST | `/api/v1/Attendance/check-in` | Check in |
| Attendance | POST | `/api/v1/Attendance/check-out` | Check out |
| Attendance | GET | `/api/v1/Attendance/daily` | Get daily attendance |
| Attendance | GET | `/api/v1/Attendance/monthly` | Get monthly attendance |
| Attendance | GET | `/api/v1/Attendance/today/status` | Check today's status |
| Notification | GET | `/api/v1/notifications` | Get notifications |
| Notification | PATCH | `/api/v1/notifications/{id}/read` | Mark as read |

---

## Request/Response Models

### Common Models

#### Pagination Response
```typescript
interface PaginationInfo {
  totalItems: number;
  totalPages: number;
  pageSize: number;
  currentPage: number;
}
```

#### Standard API Response
```typescript
interface ApiResponse<T> {
  isSuccess: boolean;
  data?: T;
  pagination?: PaginationInfo;
  error?: string;
  errorCode?: string;
}
```

---

## Frontend Implementation Guide

### Recommended Technology Stack

- **Framework:** React, Vue.js, or Angular
- **State Management:** Redux, Zustand, or Pinia
- **UI Library:** Material-UI, Ant Design, or Tailwind CSS
- **HTTP Client:** Axios or Fetch API
- **Routing:** React Router, Vue Router, or Angular Router
- **Form Handling:** React Hook Form, Formik, or VeeValidate
- **Date Handling:** date-fns or moment.js
- **Charts:** Chart.js, Recharts, or D3.js

---

### Menu Structure Recommendations

#### Option 1: Single Application with Role-Based Menu Rendering (Recommended)

**Advantages:**
- Single codebase to maintain
- Consistent UI/UX across roles
- Easier to share components
- Centralized authentication

**Implementation:**
```typescript
// Menu configuration based on role
const menuConfig = {
  SuperAdmin: [
    { name: 'Dashboard', path: '/dashboard', icon: 'dashboard' },
    {
      name: 'Administration',
      icon: 'admin',
      children: [
        { name: 'Tenants', path: '/admin/tenants' },
        { name: 'Roles', path: '/admin/roles' },
        { name: 'Admin Management', path: '/admin/admins' }
      ]
    },
    {
      name: 'Sales & Marketing',
      icon: 'sales',
      children: [
        { name: 'Marketing Executives', path: '/admin/marketing-executives' },
        { name: 'Leads', path: '/admin/leads' },
        { name: 'Quotations', path: '/admin/quotations' }
      ]
    },
    { name: 'System', path: '/admin/system/logs', icon: 'system' }
  ],
  TenantAdmin: [
    { name: 'Dashboard', path: '/dashboard', icon: 'dashboard' },
    {
      name: 'Administration',
      icon: 'admin',
      children: [
        { name: 'Organization Details', path: '/admin/organization' },
        { name: 'Branches', path: '/admin/branches' },
        { name: 'Designations', path: '/admin/designations' },
        { name: 'Marketing Executives', path: '/admin/marketing-executives' }
      ]
    },
    {
      name: 'Reporting',
      icon: 'reporting',
      children: [
        { name: 'Leads', path: '/admin/reports/leads' },
        { name: 'Quotations', path: '/admin/reports/quotations' },
        { name: 'Attendance', path: '/admin/reports/attendance' }
      ]
    },
    {
      name: 'System',
      icon: 'system',
      children: [
        { name: 'Logs', path: '/admin/system/logs' },
        { name: 'Notifications', path: '/admin/system/notifications' },
        { name: 'Notice Board', path: '/admin/system/noticeboard' }
      ]
    }
  ],
  MarketingExecutive: [
    { name: 'Dashboard', path: '/dashboard', icon: 'dashboard' },
    { name: 'Leads', path: '/leads', icon: 'leads' },
    { name: 'Quotations', path: '/quotations', icon: 'quotations' },
    { name: 'Premium Calculator', path: '/premium-calculator', icon: 'calculator' },
    { name: 'Attendance', path: '/attendance', icon: 'attendance' },
    {
      name: 'Notifications',
      icon: 'notifications',
      children: [
        { name: 'Notifications', path: '/notifications' },
        { name: 'Notice Board', path: '/noticeboard' }
      ]
    }
  ]
};

// Render menu based on user role
function renderMenu(userRole: string) {
  return menuConfig[userRole] || [];
}
```

#### Option 2: Separate Applications per Role

**Advantages:**
- Complete isolation
- Independent deployments
- Smaller bundle sizes per app

**Disadvantages:**
- Code duplication
- More maintenance
- Inconsistent UI/UX

**Recommendation:** Use Option 1 (Single Application with Role-Based Menu)

---

### Frontend Architecture Recommendations

#### 1. Folder Structure
```
src/
├── components/
│   ├── common/          # Shared components
│   ├── admin/           # Admin-specific components
│   └── marketing/      # Marketing Executive components
├── pages/
│   ├── admin/           # Admin pages
│   ├── marketing/       # Marketing Executive pages
│   └── shared/          # Shared pages (login, etc.)
├── services/
│   ├── api/             # API service layer
│   └── auth/            # Authentication service
├── store/               # State management
├── hooks/               # Custom hooks
├── utils/               # Utility functions
└── types/               # TypeScript types
```

#### 2. API Service Layer
```typescript
// services/api/leadService.ts
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || '/api/v1';

export const leadService = {
  create: async (data: CreateLeadPublicDto) => {
    const response = await axios.post(`${API_BASE_URL}/Lead`, data);
    return response.data;
  },
  
  list: async (params: CommonPaginationRequestModel) => {
    const response = await axios.get(`${API_BASE_URL}/Lead`, { params });
    return response.data;
  },
  
  getById: async (id: string) => {
    const response = await axios.get(`${API_BASE_URL}/Lead/${id}`);
    return response.data;
  },
  
  updateStatus: async (id: string, status: string) => {
    const response = await axios.patch(`${API_BASE_URL}/Lead/${id}/status`, { status });
    return response.data;
  },
  
  addActivity: async (id: string, activity: LeadActivityDto) => {
    const response = await axios.post(`${API_BASE_URL}/Lead/${id}/activities`, activity);
    return response.data;
  },
  
  getActivities: async (id: string) => {
    const response = await axios.get(`${API_BASE_URL}/Lead/${id}/activities`);
    return response.data;
  }
};
```

#### 3. Authentication & Authorization
```typescript
// services/auth/authService.ts
import axios from 'axios';

export const authService = {
  login: async (credentials: LoginRequestDto) => {
    const response = await axios.post('/api/v1/AdminAuth/login', credentials);
    const { accessToken, refreshToken } = response.data.data;
    
    // Store tokens
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    
    // Set default authorization header
    axios.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
    
    return response.data;
  },
  
  logout: async () => {
    await axios.post('/api/v1/AdminAuth/logout');
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    delete axios.defaults.headers.common['Authorization'];
  },
  
  getCurrentUser: () => {
    // Decode JWT token or call user profile endpoint
    return JSON.parse(localStorage.getItem('user') || '{}');
  }
};

// Axios interceptor for token refresh
axios.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Try to refresh token
      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        try {
          const response = await axios.post('/api/v1/AdminAuth/refresh', { refreshToken });
          const { accessToken } = response.data.data;
          localStorage.setItem('accessToken', accessToken);
          axios.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
          return axios(error.config);
        } catch (refreshError) {
          // Refresh failed, redirect to login
          authService.logout();
          window.location.href = '/login';
        }
      }
    }
    return Promise.reject(error);
  }
);
```

#### 4. Permission-Based Component Rendering
```typescript
// components/common/PermissionGuard.tsx
import { useAuth } from '../hooks/useAuth';

interface PermissionGuardProps {
  permission: string;
  children: React.ReactNode;
  fallback?: React.ReactNode;
}

export const PermissionGuard: React.FC<PermissionGuardProps> = ({
  permission,
  children,
  fallback = null
}) => {
  const { user } = useAuth();
  const hasPermission = user?.permissions?.includes(permission);
  
  return hasPermission ? <>{children}</> : <>{fallback}</>;
};

// Usage
<PermissionGuard permission="18-12-1">
  <TenantList />
</PermissionGuard>
```

#### 5. Role-Based Route Protection
```typescript
// hooks/useRoleRoute.ts
import { useAuth } from './useAuth';
import { useNavigate } from 'react-router-dom';

export const useRoleRoute = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  
  const checkRole = (allowedRoles: string[]) => {
    if (!user || !allowedRoles.includes(user.role)) {
      navigate('/unauthorized');
      return false;
    }
    return true;
  };
  
  return { checkRole };
};

// Route component
const AdminRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { checkRole } = useRoleRoute();
  const { user } = useAuth();
  
  useEffect(() => {
    checkRole(['SuperAdmin', 'Admin']);
  }, []);
  
  if (!user || !['SuperAdmin', 'Admin'].includes(user.role)) {
    return <Navigate to="/unauthorized" />;
  }
  
  return <>{children}</>;
};
```

---

### UI/UX Recommendations

#### 1. Dashboard Design
- Use card-based layout for metrics
- Include charts for trends (Chart.js, Recharts)
- Show recent activity feed
- Quick action buttons for common tasks

#### 2. List/Table Views
- Implement pagination
- Add filters and search
- Enable sorting
- Bulk actions where applicable
- Export functionality

#### 3. Form Design
- Use consistent form layouts
- Implement validation
- Show loading states
- Success/error notifications
- Auto-save for long forms (optional)

#### 4. Attendance Module
- Use geolocation API for check-in/check-out
- Show map with location pin
- Calendar view for attendance history
- Prominent reminder if not checked in

#### 5. Premium Calculator
- Real-time calculation
- Show breakdown in expandable sections
- Allow parameter adjustments
- Save calculation history (optional)

---

## Menu Structure Recommendations

### Answer: Should you make different menus for different roles?

**Yes, you should implement role-based menu rendering within a single application.**

**Recommended Approach:**
1. **Single Application** with role-based menu configuration
2. **Dynamic Menu Rendering** based on user role and permissions
3. **Permission-Based Menu Items** - Hide/show menu items based on permissions
4. **Consistent UI/UX** across all roles

### Implementation Strategy

```typescript
// 1. Define menu structure with permissions
interface MenuItem {
  id: string;
  name: string;
  path: string;
  icon: string;
  permission?: string; // Permission required to see this menu
  roles?: string[]; // Roles that can see this menu
  children?: MenuItem[];
}

// 2. Menu configuration
const menuItems: MenuItem[] = [
  {
    id: 'dashboard',
    name: 'Dashboard',
    path: '/dashboard',
    icon: 'dashboard',
    roles: ['SuperAdmin', 'Admin', 'MarketingExecutive']
  },
  {
    id: 'administration',
    name: 'Administration',
    icon: 'admin',
    roles: ['SuperAdmin', 'Admin'],
    children: [
      {
        id: 'tenants',
        name: 'Tenants',
        path: '/admin/tenants',
        permission: '18-12-1', // TenantsView
        roles: ['SuperAdmin']
      },
      {
        id: 'roles',
        name: 'Roles',
        path: '/admin/roles',
        permission: '18-2-1', // RolesView
        roles: ['SuperAdmin', 'Admin']
      },
      {
        id: 'admins',
        name: 'Admin Management',
        path: '/admin/admins',
        permission: '18-4-1', // AdminManagementView
        roles: ['SuperAdmin', 'Admin']
      },
      {
        id: 'branches',
        name: 'Branches',
        path: '/admin/branches',
        permission: '18-14-1', // BranchView
        roles: ['Admin']
      },
      {
        id: 'designations',
        name: 'Designations',
        path: '/admin/designations',
        permission: '18-15-1', // DesignationView
        roles: ['Admin']
      },
      {
        id: 'marketing-executives',
        name: 'Marketing Executives',
        path: '/admin/marketing-executives',
        permission: '19-13-1', // MarketingExecutivesView
        roles: ['SuperAdmin', 'Admin']
      }
    ]
  },
  {
    id: 'sales-marketing',
    name: 'Sales & Marketing',
    icon: 'sales',
    roles: ['SuperAdmin', 'Admin'],
    children: [
      {
        id: 'admin-leads',
        name: 'Leads',
        path: '/admin/leads',
        permission: '19-5-1', // AdminLeadsView
        roles: ['SuperAdmin', 'Admin']
      },
      {
        id: 'admin-quotations',
        name: 'Quotations',
        path: '/admin/quotations',
        permission: '19-6-1', // AdminQuotationsView
        roles: ['SuperAdmin', 'Admin']
      }
    ]
  },
  {
    id: 'leads',
    name: 'Leads',
    path: '/leads',
    icon: 'leads',
    permission: '19-14-1', // MarketingExecutiveLeadsView
    roles: ['MarketingExecutive']
  },
  {
    id: 'quotations',
    name: 'Quotations',
    path: '/quotations',
    icon: 'quotations',
    permission: '19-15-1', // MarketingExecutiveQuotationsView
    roles: ['MarketingExecutive']
  },
  {
    id: 'premium-calculator',
    name: 'Premium Calculator',
    path: '/premium-calculator',
    icon: 'calculator',
    permission: '20-9-10-1', // PremiumOverviewView
    roles: ['MarketingExecutive']
  },
  {
    id: 'attendance',
    name: 'Attendance',
    path: '/attendance',
    icon: 'attendance',
    permission: '19-16-1', // CommonAttendanceView
    roles: ['MarketingExecutive']
  },
  {
    id: 'system',
    name: 'System',
    icon: 'system',
    roles: ['SuperAdmin', 'Admin'],
    children: [
      {
        id: 'logs',
        name: 'Logs',
        path: '/admin/system/logs',
        permission: '21-15-1', // LogsView
        roles: ['SuperAdmin', 'Admin']
      },
      {
        id: 'notifications',
        name: 'Notifications',
        path: '/admin/system/notifications',
        permission: '20-14-1', // NotificationsView
        roles: ['Admin']
      },
      {
        id: 'noticeboard',
        name: 'Notice Board',
        path: '/admin/system/noticeboard',
        roles: ['Admin']
      }
    ]
  },
  {
    id: 'notifications-me',
    name: 'Notifications',
    path: '/notifications',
    icon: 'notifications',
    permission: '20-14-1', // NotificationsView
    roles: ['MarketingExecutive']
  }
];

// 3. Filter menu based on role and permissions
function getFilteredMenu(userRole: string, userPermissions: string[]): MenuItem[] {
  return menuItems
    .filter(item => {
      // Check if role is allowed
      if (item.roles && !item.roles.includes(userRole)) {
        return false;
      }
      
      // Check if permission is required and user has it
      if (item.permission && !userPermissions.includes(item.permission)) {
        return false;
      }
      
      // Filter children recursively
      if (item.children) {
        item.children = getFilteredMenu(userRole, userPermissions);
        // Remove parent if no children remain
        if (item.children.length === 0) {
          return false;
        }
      }
      
      return true;
    });
}

// 4. Use in component
const Sidebar: React.FC = () => {
  const { user } = useAuth();
  const filteredMenu = getFilteredMenu(user.role, user.permissions);
  
  return (
    <nav>
      {filteredMenu.map(item => (
        <MenuItem key={item.id} item={item} />
      ))}
    </nav>
  );
};
```

### Benefits of This Approach

1. **Single Codebase**: Easier maintenance and updates
2. **Consistent UI/UX**: Same design system across roles
3. **Permission-Based**: Menu items respect user permissions
4. **Flexible**: Easy to add new roles or menu items
5. **Type-Safe**: TypeScript ensures correctness

---

## Additional Recommendations

### 1. API Response Handling
- Implement consistent error handling
- Show user-friendly error messages
- Handle network errors gracefully
- Implement retry logic for failed requests

### 2. Loading States
- Show loading indicators for async operations
- Use skeleton screens for better UX
- Implement optimistic updates where appropriate

### 3. Data Caching
- Cache frequently accessed data
- Implement stale-while-revalidate pattern
- Clear cache on logout

### 4. Security
- Never store sensitive data in localStorage
- Use httpOnly cookies for tokens (if possible)
- Implement CSRF protection
- Validate all user inputs

### 5. Performance
- Implement code splitting
- Lazy load routes
- Optimize bundle size
- Use memoization for expensive computations

---

## Conclusion

This documentation provides a comprehensive guide for implementing the BeemaEdge frontend and backend integration. The recommended approach is to use a **single application with role-based menu rendering** for better maintainability and consistency.

For any questions or clarifications, please refer to the API endpoints documentation or contact the development team.

---

---

## Codebase Architecture Analysis

### Project Structure

The BeemaEdge solution follows a clean architecture pattern with the following layers:

```
BeemaEdge/
├── src/
│   ├── Web/                    # API Layer (ASP.NET Core)
│   │   ├── Controllers/        # API Controllers organized by role
│   │   ├── Filters/            # Authorization & Action Filters
│   │   ├── Middleware/         # Custom Middleware (Tenant Resolution, etc.)
│   │   └── Extensions/         # Service Extensions
│   ├── Business/               # Business Logic Layer
│   │   ├── Business.Common/   # Shared business services
│   │   ├── Business.AdminPortalApi/  # Admin-specific services
│   │   └── Business.CustomerPortalApi/ # Customer-specific services
│   ├── Data/                   # Data Access Layer
│   │   ├── Context/            # DbContext implementations
│   │   └── Entities/           # Entity models
│   ├── Models/                 # DTOs and Request/Response Models
│   ├── SharedKernel/           # Shared constants, interfaces
│   └── Infrastructure/         # Infrastructure services
```

### Key Implementation Details

#### 1. Multi-Tenancy Implementation
- **Tenant Resolution**: `TenantResolutionMiddleware` extracts tenant from subdomain or header
- **Tenant Context**: `ITenantContext` provides current tenant information
- **Tenant Filtering**: Automatic filtering via `ApplicationDataContext` SaveChanges
- **SuperAdmin Handling**: SuperAdmin users have `TenantId = null` and can access all tenants

#### 2. Authentication & Authorization
- **JWT Token Service**: `TokenService` generates JWT tokens with role claims
- **Permission System**: `PermissionAttribute` checks permissions from `RoleClaims`
- **Role-Based Access**: Three system roles: SuperAdmin (999), Admin (500), FoDo/MarketingExecutive (300)
- **2FA Support**: Two-factor authentication endpoints available

#### 3. Controllers Organization
Controllers are organized by role and module:
- `/V1/Admin/*` - SuperAdmin and Tenant Admin endpoints
- `/V1/MarketingExecutive/*` - Marketing Executive endpoints
- `/V1/Common/*` - Shared endpoints (Attendance, Notifications, etc.)
- `/V1/Individual/*` - Customer portal endpoints

#### 4. Business Services
Key services identified:
- `ITenantAdminService` - Tenant management
- `ILeadService` - Lead management
- `IQuotationService` - Quotation management
- `IAttendanceService` - Attendance tracking
- `INotificationService` - Notification system
- `IReportingService` - Reporting and analytics
- `IFodoService` - Marketing Executive management
- `IAdminService` - Admin user management
- `IBranchService` - Branch management
- `IDesignationService` - Designation management

#### 5. Database Contexts
- `ApplicationDataContext` - Main application database
- `AuditDataContext` - Audit logging database
- `HangfireDataContext` - Background job database

#### 6. Permission Constants
Permissions are defined in `SharedKernel.Constant.Permission.MenuPermissionConstant`:
- Format: `ParentMenuId-ChildMenuId-OperationType`
- Example: `TenantsView`, `TenantsCreate`, `TenantsUpdate`, `TenantsDelete`, `TenantsExport`

### API Response Wrapper
All API responses follow a consistent format:
```csharp
{
  "isSuccess": true/false,
  "data": { ... },
  "pagination": { ... },  // For paginated responses
  "error": "Error message",  // For errors
  "errorCode": "ERROR_CODE"  // For errors
}
```

### Pagination & Filtering
- Uses `CommonPaginationRequestModel` for pagination
- Sieve library for advanced filtering and sorting
- Query syntax: `fieldName@=value` for filters, `fieldName` or `fieldName desc` for sorts

### Excel Export & PDF Generation
- `IExcelExportService` - Excel export functionality
- `IQuotationPdfService` - PDF generation for quotations
- Export endpoints available for various entities

### Background Jobs
- Hangfire integration for background processing
- Job dashboard available at configured endpoint

### Health Checks
- Health check endpoints for monitoring
- Database connectivity checks

### Rate Limiting
- Rate limiting middleware configured
- Configurable per endpoint

---

## Implementation Status

### ✅ Completed Backend Features
- Multi-tenant architecture with tenant isolation
- Role-based authentication and authorization
- Permission-based access control
- Lead management (CRUD, activities, status updates)
- Quotation management (CRUD, PDF generation)
- Attendance tracking (check-in/check-out)
- Notification system
- Reporting service
- Excel export functionality
- PDF generation for quotations
- Branch management
- Designation management
- Admin user management
- Marketing Executive management
- Tenant management (SuperAdmin only)
- Role management with permission assignment
- System logging
- Two-factor authentication
- Password management
- Profile management

### 🔄 Partially Implemented
- Notice Board (may need implementation)
- Advanced reporting dashboards
- Renewal reminders
- Premium calculation engine (basic structure exists)

### ❌ Not Yet Implemented (Frontend Required)
- All frontend UI components
- Frontend routing and navigation
- Frontend state management
- Frontend form validation
- Frontend charts and visualizations
- Frontend file upload UI
- Frontend notification UI
- Frontend attendance calendar view

---

## Development Guidelines

### Backend Development
1. **Controller Pattern**: Inherit from appropriate base controller (`BaseAdminApiController`, `BaseMarketingExecutiveApiController`)
2. **Permission Checks**: Use `[Permission]` attribute on controller actions
3. **Service Layer**: Business logic should be in service layer, not controllers
4. **Error Handling**: Use `Result<T>` pattern from `SharedKernel.Operation`
5. **Tenant Filtering**: Services automatically filter by tenant (except SuperAdmin)
6. **Pagination**: Always use `CommonPaginationRequestModel` for list endpoints
7. **Response Format**: Use `HandleResult()` method from base controller

### Frontend Development
1. **API Client**: Create typed API client using generated types from `FRONTEND_TYPES.ts`
2. **Error Handling**: Implement consistent error handling for API responses
3. **Permission Guards**: Check permissions before rendering components
4. **Role-Based Routing**: Implement route guards based on user role
5. **State Management**: Use centralized state management for user, tenant, permissions
6. **Form Validation**: Implement client-side validation matching backend validators
7. **Loading States**: Show loading indicators for all async operations

---

**Document Version:** 2.0  
**Last Updated:** 2024  
**Maintained By:** Project Management Team  
**Enhanced With:** Codebase Analysis



