```markdown
# Himalayan Everest Insurance – FO/DO App  
## Backend (.NET) Technical Kickoff & Best Practices

**Document Version:** 0.1  
**Prepared By:** Backend Team  
**Target Stack:** .NET 8, C# 12, ASP.NET Core Web API, EF Core  
**Last Updated:** _TBD_

This document defines how we will design and implement the backend for the FO/DO mobile application in .NET, following modern backend best practices.  
Scope is aligned with the business proposal for the FO/DO mobile app and the backend engine layer (LMS, Premium Calculation, Notification, Quotation, Attendance, Renewals, Admin Panel). fileciteturn0file0

---

## 1. Goals & Scope

### 1.1 Goals

- Provide a secure, scalable, and maintainable backend for:
  - FO/DO Mobile App (React Native)
  - Admin Web Panel (React)
- Expose REST APIs for:
  - Lead Management (LMS)
  - Premium Calculation
  - Notification Management
  - Quotation Management
  - Policy Renewal
  - Attendance (geo-tagged)
  - Reporting & KPIs (target vs. achievement, expiring policies, etc.)
- Integrate with **HEI Core** (existing insurance system) for:
  - Authentication / SSO / Tokens
  - Retrieval of client, policy, claims, and KPI data
- Meet non-functional requirements:
  - Performance (<3s API response for typical operations)
  - Security (encryption, RBAC)
  - Availability (99.5%+)
  - Auditability (logs, audit trails)

### 1.2 Out of Scope (for MVP)

- Offline mode
- Advanced supervisor dashboards
- Core system changes (no modifications to HEI Core logic)
- AI/ML & predictive analytics
- Multi-language support (English only in Phase 1)

---

## 2. Architecture Overview

### 2.1 Architectural Style

- **Clean / Hexagonal Architecture** with clear separation of concerns:
  - **API Layer** (Presentation): HTTP controllers / minimal APIs
  - **Application Layer**: Use cases, commands/queries, DTOs, validation
  - **Domain Layer**: Entities, value objects, domain services, business rules
  - **Infrastructure Layer**: EF Core, external API clients (HEI Core, SMS, email, Firebase), repositories, caching, logging

Benefits:
- Testable business logic (domain and application layers)
- Infrastructure replaceable (e.g., different DB or notification provider)
- Clear ownership for each module (LMS, Premium, etc.)

### 2.2 High-Level Components

- **API Gateway / Backend API**  
  ASP.NET Core Web API exposing endpoints for the mobile and admin clients.
- **LMS Engine**  
  Lead lifecycle management (create, update status, follow-ups, filters).
- **Premium Calculation Engine**  
  Implements premium rules (imported from HEI). Calculation done locally but aligned with HEI Core.
- **Notification Engine**  
  Sends push (via Firebase), SMS (via configured gateway), email; logs notification history.
- **Quotation Engine**  
  Generates and stores quotations, produces PDFs for sharing and syncs with HEI Core.
- **Attendance & Geo Engine**  
  Manages check-in/out, geo-coordinates, and basic geofencing logic.
- **Reporting & KPI Module**  
  Aggregates internal data and consumes HEI Core APIs for targets, achievements, expiring policies, etc.
- **Integration Layer**  
  Typed HTTP clients for HEI Core, SMS gateway, SMTP, Firebase, Google Maps (geocoding / geofencing if needed).

---

## 3. Technology Stack

### 3.1 Core Backend

- **Runtime:** .NET 8 (LTS)
- **Language:** C# 12
- **Framework:** ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **Database:**
  - Primary choice: **postgres** (aligned with existing proposal) 
- **Message & Background Jobs:**  
  - Built-in **Hangfire** for scheduled jobs (renewal reminders, KPI sync).

### 3.2 Integrations & Infra

- **HEI Core APIs:** REST over HTTPS, token-based auth
- **Notifications:**
  - Firebase Cloud Messaging (FCM) for push
  - SMS Gateway for OTP / reminders
  - SMTP for email (quotations)
- **Maps / Geo:** Google Maps API for geocoding / optional geofencing
- **Hosting Options:**
  - Linux containers (Kubernetes / Docker)
  - Or Windows/IIS if required

### 3.3 Cross-Cutting

- **Authentication & Authorization:** JWT + Claims-based RBAC
- **Validation:** FluentValidation (or ASP.NET Core model validation attributes)
- **Logging:** Serilog (structured logging, sinks for console + file + centralized log)
- **Configuration:** ASP.NET Core configuration providers (appsettings + env vars + secret store)
- **API Documentation:** Swashbuckle/Swagger

---

## 4. Solution Structure

Proposed .NET solution layout:

```text
Hei.FodoApp.sln
/src
  Hei.Fodo.Api           -> ASP.NET Core Web API (controllers / minimal APIs)
/src
  Hei.Fodo.Application   -> Use cases, DTOs, interfaces, validators
/src
  Hei.Fodo.Domain        -> Entities, value objects, domain services, enums
/src
  Hei.Fodo.Infrastructure-> EF Core, repositories, migrations, integrations
/tests
  Hei.Fodo.UnitTests
  Hei.Fodo.IntegrationTests
  Hei.Fodo.ComponentTests (optional)
```

### 4.1 Namespaces & Modules

Inside each layer, group by bounded context / module:

- `Lms` (Lead Management)
- `Premiums`
- `Notifications`
- `Quotations`
- `Attendance`
- `Renewals`
- `Reporting`
- `Identity` (Auth, users, roles)
- `Shared` (common primitives, exceptions, utilities)

Example:

```text
Hei.Fodo.Application
  /Lms
    Commands/
    Queries/
    Dtos/
  /Premiums
  /Notifications
  ...
```

---

## 5. Domain & Modules

### 5.1 Identity & Access

Responsibilities:

- Manage FO/DO and Admin identities (view-only user management via HEI Core; no local password store if SSO).
- Role-based access:
  - **Role.Admin**
  - **Role.FO**
  - **Role.DO**

Key entities:

- `User` (Id, ExternalIdFromHEICore, Name, Email, Role, Region, Status)
- `RefreshToken` (optional, if implementing refresh tokens)

Best Practices:

- Prefer **token/SSO** from HEI Core; the backend trusts HEI Core as the IdP.
- Use ASP.NET Core `Authorize` attribute with policies (e.g., `RequireRole("Admin")`).

---

### 5.2 Lead Management (LMS)

Responsibilities:

- CRUD on leads
- Status transitions: `New`, `Contacted`, `InProgress`, `ClosedWon`, `ClosedLost`
- Follow-up scheduling and notes

Core entities:

- `Lead` (Id, AssignedToUserId, Name, ContactInfo, Region, Status, CreatedAt, UpdatedAt)
- `LeadActivity` (Id, LeadId, Type, Remarks, NextFollowUpAt, CreatedAt)

Best Practices:

- Implement all write operations as **Commands** and read operations as **Queries**.
- Enforce status transition rules at the domain layer (e.g., cannot go from `ClosedLost` → `InProgress` without explicit reopen).

---

### 5.3 Premium Calculation Engine

Responsibilities:

- Calculate premium based on age, sum assured, term, payment mode, riders, etc.
- Use HEI-provided tables/rates (configurable via Admin UI; core formula remains fixed).

Core entities/value objects:

- `PremiumRequest` (ProductCode, Age, SumAssured, Term, PaymentMode, Riders[])
- `PremiumResult` (BasePremium, RiderPremiums, Taxes, TotalPremium, Breakdown)

Best Practices:

- Keep premium logic in **domain services**, not in controllers.
- Load rates from DB or configuration tables, not hard-coded constants.
- Wrap calculations in unit-tested, deterministic functions.

---

### 5.4 Notification Engine

Responsibilities:

- Orchestrate push, SMS, and email notifications:
  - New leads
  - Renewals / premium reminders
  - Assignment changes
- Maintain notification history and read status.

Entities:

- `Notification` (Id, UserId, Channel, Title, Body, Payload, SentAt, ReadAt)
- `NotificationTemplate` (Name, Channel, Subject, BodyTemplate, Placeholders)

Best Practices:

- Use a background job queue for sending notifications (avoid long-running controller actions).
- Implement retry with exponential backoff for external providers.

---

### 5.5 Quotation Engine

Responsibilities:

- Generate quotations (premium + benefits overview).
- Store and sync quotations with HEI Core.
- Generate PDFs for client sharing (email / WhatsApp).

Entities:

- `Quotation` (Id, LeadId, ProductCode, PremiumResult, ValidTill, Status, PdfUrl)

Best Practices:

- Keep PDF generation in infrastructure layer (e.g., using `DinkToPdf` or any approved library).
- Store PDFs in object storage (S3-compatible / blob) and only persist URLs in DB.

---

### 5.6 Policy Renewal & Attendance

**Policy Renewal:**

- List policies near renewal / overdue
- Trigger reminders (Notification Engine)
- Show premium dues and history (data from HEI Core)

**Attendance:**

- Check-in/out with geo-location
- Daily and monthly summaries

Entities:

- `AttendanceEntry` (Id, UserId, Type: CheckIn/CheckOut, Latitude, Longitude, Timestamp)
- `RenewalReminder` (Id, PolicyId, UserId, DueDate, ReminderSentAt, Channel)

Best Practices:

- Validate geo-coordinates and optionally geofence by region.
- Ensure idempotency for repeated check-ins (e.g., no duplicate for same user within X minutes).

---

## 6. API Design Standards

### 6.1 General Guidelines

- RESTful endpoints, resource-oriented, plural nouns:
  - `GET /api/leads`
  - `POST /api/leads`
  - `PATCH /api/leads/{id}/status`
- Use **API versioning**:
  - `api/v1/leads`
- Use JSON for requests and responses.
- Standard response envelope (optional):

```json
{
  "data": { },
  "errors": [],
  "traceId": "..."
}
```

### 6.2 Error Handling

- Use HTTP status codes appropriately:
  - 200/201 – Success
  - 400 – Validation errors
  - 401 – Unauthorized
  - 403 – Forbidden
  - 404 – Not found
  - 409 – Conflict
  - 500 – Unexpected server errors
- Return validation errors in a standard format.

### 6.3 Authentication & Authorization

- Accept **Bearer JWT** tokens issued by HEI Core (or local IdP implementing same).
- Validate token on every request using ASP.NET Core authentication middleware.
- Implement policies:
  - `[Authorize(Policy = "AdminOnly")]`
  - `[Authorize(Policy = "SalesStaff")]`

---

## 7. Persistence & Data Access

### 7.1 EF Core Usage

- One `DbContext` per logical database (likely single DB for MVP).
- Fluent API for configuration; avoid data annotations for everything except simple constraints.
- Use **migrations** for schema evolution; migrations are generated and applied via CI/CD or manual (depending on ops).

### 7.2  Query Pattern


  - Direct EF Core usage inside Application layer via interfaces (`IAppDbContext`).
- Read-heavy queries (dashboards, KPIs) may use:
  - Raw SQL / Dapper **read models** for performance where necessary.

### 7.3 Multi-Tenancy & Audit

- Even if single-tenant, include audit columns by convention:
  - `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `IsDeleted`
- Implement soft delete for critical aggregates (Leads, Quotations).

---

## 8. Integration with HEI Core & External Services

### 8.1 HEI Core
- use refit
- Use **typed HttpClient** (`IHttpClientFactory`) with:
  - Centralized configuration of base URLs and timeouts.
  - Retry policies and circuit breakers via **Polly**.
- Define integration interfaces inside Application layer, implement in Infrastructure:
  - `IHeiCoreAuthClient`
  - `IHeiCorePolicyClient`
  - `IHeiCoreKpiClient`
- Ensure correlation IDs are passed between systems (e.g., via headers).

### 8.2 Third-Party Services

- **SMS Gateway**: implement `ISmsSender` with provider-specific class.
- **Email (SMTP)**: implement `IEmailSender`.
- **Firebase**: `IPushNotificationSender`.
- **Google Maps**: `IGeoService` if additional geocoding or distance computation required.

Configuration must be environment-specific and never hard-coded.

---

## 9. Non-Functional Requirements Implementation

### 9.1 Performance

- Use asynchronous APIs (`async/await`) end-to-end.
- Avoid N+1 queries; utilize `Include`, projections, or explicit queries.
- Introduce caching where appropriate:
  - Static reference data (e.g., product definitions, rate tables).
  - Short-lived caching for heavy KPI queries (with HEI approval).

### 9.2 Security

- Enforce HTTPS everywhere.
- Store secrets (connection strings, API keys) in:
  - Secret manager / key vault (not in Git).
- Implement input validation and output encoding.
- Log security events (login failures, suspicious patterns) without storing sensitive PII unnecessarily.

### 9.3 Observability

- Structured logs (JSON), with `traceId`, `userId`, `endpoint`, `duration`.
- Metrics:
  - Request count, latency, error rates per endpoint.
  - Scheduler job success/failure counts.
- Health checks using ASP.NET Core Health Checks:
  - `/health/live`
  - `/health/ready`

---

## 10. Environments & DevOps

### 10.1 Environments

- `local` – developer environment
- `dev` – integrated development & QA
- `uat` – user acceptance testing (connected to HEI UAT Core)
- `prod` – production

Each environment has:

- Separate DB instance
- Separate app settings
- HEI Core endpoint per environment

### 10.2 CI/CD

- Use build pipelines to:
  - Restore, build, run unit tests
  - Run static analysis (e.g., SonarQube/Analyzers)
  - Package into container image or deployment artifact
- Deployment pipeline:
  - Apply migrations
  - Deploy API to target environment
  - Run smoke tests

---

## 11. Coding Standards & Guidelines

- Follow **Microsoft C# Coding Conventions**.
- Use nullable reference types (`#nullable enable`).
- Keep controllers thin; move logic to application layer (commands/queries).
- One public class per file; meaningful names.
- Avoid static state (except constants); rely on DI for dependencies.
- Enforce code review for all PRs; no direct commits to main branch.

Example folder for a module (`Lms`):

```text
Hei.Fodo.Application/Lms
  CreateLead
    CreateLeadCommand.cs
    CreateLeadCommandHandler.cs
    CreateLeadValidator.cs
  GetLeads
    GetLeadsQuery.cs
    GetLeadsQueryHandler.cs
    LeadDto.cs
```

---

## 13. Definition of Done (Backend Story)

A backend user story is considered **Done** when:

1. API contract is defined and documented in Swagger.
2. Domain and application logic are implemented.
3. Input validation and authorization rules are implemented.
4. Unit tests added and passing.
5. Integration tests added (if cross-module or external calls involved).
6. Logging, error handling, and metrics added where relevant.
7. Code reviewed and merged to main.
8. Deployed to `dev` and successfully smoke-tested.
9. API change communicated to mobile/admin teams.

---



---
```