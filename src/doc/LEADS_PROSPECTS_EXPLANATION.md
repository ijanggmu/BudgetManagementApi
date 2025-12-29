# Leads, Prospects, and Contacts - Concept Explanation

## Overview

This application uses a **three-tier hierarchy** for managing potential customers in the sales/insurance pipeline:

```
Contact → Prospect → Lead
```

Each tier represents a different stage of customer engagement and data organization.

---

## 1. **Contact** (Base Level)

### Definition
A **Contact** is the most basic entity - it represents a **person** with contact information.

### Purpose
- Stores personal information about an individual
- Acts as a reusable entity that can be associated with multiple Prospects
- Contains basic demographic and contact data

### Properties
```csharp
public class Contact : TenantEntity
{
    public string FullName { get; set; }      // Person's full name
    public string Email { get; set; }         // Email address
    public string Phone { get; set; }         // Phone number
    public string AddressId { get; set; }     // Reference to Address entity
    public Address Address { get; set; }      // Physical address details
}
```

### Key Characteristics
- ✅ **Multi-tenant**: Each tenant has their own contacts
- ✅ **Reusable**: One contact can be associated with multiple prospects
- ✅ **Addressable**: Can have a physical address
- ✅ **Base entity**: Inherits from `TenantEntity` (has Id, CreatedOn, etc.)

### Example Use Cases
- Storing customer information
- Maintaining a contact database
- Reusing contact information across different business contexts

---

## 2. **Prospect** (Middle Level)

### Definition
A **Prospect** represents a **potential business opportunity** or **potential customer** who has shown interest in your products/services.

### Purpose
- Groups one or more contacts together for a business opportunity
- Represents a potential sale or business relationship
- Acts as a container for contact information in a business context

### Properties
```csharp
public class Prospect : TenantEntity
{
    public string PrimaryContactId { get; set; }    // Reference to main contact
    public Contact PrimaryContact { get; set; }     // The main contact person
}
```

### Key Characteristics
- ✅ **Business-focused**: Represents a business opportunity
- ✅ **Contact-based**: Always has at least one primary contact
- ✅ **Expandable**: Can potentially have multiple contacts (future enhancement)
- ✅ **Tenant-scoped**: Each tenant manages their own prospects

### Relationship with Contact
- A Prospect **must have** a Primary Contact
- One Contact can be the primary contact for multiple Prospects
- This allows the same person to be involved in different business opportunities

### Example Scenarios
1. **Individual Prospect**: John Doe (Contact) wants to buy health insurance → One Prospect with John as Primary Contact
2. **Family Prospect**: The Smith family wants family insurance → One Prospect with Mr. Smith as Primary Contact (could expand to include Mrs. Smith later)
3. **Corporate Prospect**: ABC Company wants group insurance → One Prospect with HR Manager as Primary Contact

---

## 3. **Lead** (Top Level - Sales Pipeline)

### Definition
A **Lead** represents a **sales opportunity** that is actively being pursued. It's the entry point into the sales pipeline.

### Purpose
- Tracks potential sales through a defined workflow
- Manages the sales process from initial interest to closure
- Records activities and interactions with the prospect
- Assigns ownership and tracks progress

### Properties
```csharp
public class Lead : TenantEntity
{
    public string ProspectId { get; set; }           // Reference to the prospect
    public Prospect Prospect { get; set; }          // The prospect being pursued
    public LeadStatus Status { get; set; }           // Current stage in pipeline
    public string Source { get; set; }               // Where the lead came from
    public Guid? OwnerUserId { get; set; }          // Assigned sales person/agent
}
```

### Lead Status Workflow
The application uses a **state machine** for lead progression:

```
New → Qualified → Contacted → Quoted → Won/Lost
```

#### Status Definitions:
1. **New**: Initial lead created, not yet reviewed
2. **Qualified**: Lead has been reviewed and deemed worth pursuing
3. **Contacted**: Initial contact has been made with the prospect
4. **Quoted**: A quote/proposal has been sent to the prospect
5. **Won**: Lead converted to a sale/customer
6. **Lost**: Lead did not convert (rejected, went elsewhere, etc.)

### Key Characteristics
- ✅ **Sales-focused**: Part of the sales pipeline
- ✅ **Status-tracked**: Has a defined workflow
- ✅ **Activity-logged**: Can have multiple activities (calls, emails, notes)
- ✅ **Assignable**: Can be assigned to a specific user/agent
- ✅ **Source-tracked**: Records where the lead originated (Web, Referral, etc.)

### Lead Activities
Each lead can have multiple **LeadActivity** records:

```csharp
public class LeadActivity : TenantEntity
{
    public string LeadId { get; set; }              // Which lead this activity belongs to
    public string Kind { get; set; }                // Type: "note", "call", "email"
    public string Notes { get; set; }               // Activity details
    public DateTimeOffset When { get; set; }         // When the activity occurred
}
```

**Activity Types:**
- **note**: General notes or observations
- **call**: Phone call with the prospect
- **email**: Email communication

---

## Data Flow & Relationships

### Creation Flow
When a new lead is created, the system automatically creates the hierarchy:

```
1. Create Contact (FullName, Email, Phone)
   ↓
2. Create Prospect (with PrimaryContact = Contact)
   ↓
3. Create Lead (with Prospect, Status = New, Source = "Web")
```

### Entity Relationships Diagram

```
┌─────────────┐
│   Contact   │ (1 person, contact info)
└──────┬──────┘
       │ (PrimaryContact)
       │
┌──────▼──────┐
│  Prospect   │ (1 business opportunity)
└──────┬──────┘
       │ (Prospect)
       │
┌──────▼──────┐
│    Lead     │ (1 sales opportunity)
└──────┬──────┘
       │
┌──────▼──────────┐
│ LeadActivity    │ (Many activities per lead)
└─────────────────┘
```

### Database Relationships
- **Contact** → **Prospect**: One-to-Many (one contact can be primary for many prospects)
- **Prospect** → **Lead**: One-to-One (one prospect = one lead, but a prospect could theoretically have multiple leads)
- **Lead** → **LeadActivity**: One-to-Many (one lead has many activities)

---

## Business Logic & Rules

### Lead Status Transitions
The application enforces **state machine rules**:

✅ **Allowed Transitions:**
- New → Qualified → Contacted → Quoted → Won/Lost
- Can move forward through the pipeline
- Can move to Lost from any stage

❌ **Restricted Transitions:**
- Cannot move from `Lost` directly to `Contacted` (must go through proper workflow)
- Status changes are validated before saving

### Multi-Tenancy
- All entities inherit from `TenantEntity`
- Each tenant has isolated data
- Global query filters automatically apply tenant filtering
- No cross-tenant data leakage

### Ownership & Assignment
- Leads can be assigned to specific users (`OwnerUserId`)
- This allows:
  - Sales team management
  - Performance tracking
  - Workload distribution
  - Follow-up responsibility

---

## API Endpoints

### Lead Management
- `POST /api/v1/leads` - Create a new lead (creates Contact → Prospect → Lead)
- `GET /api/v1/leads` - List leads with filtering (status, date range)
- `GET /api/v1/leads/{id}` - Get lead details with prospect and contact info
- `PATCH /api/v1/leads/{id}/status` - Update lead status
- `POST /api/v1/leads/{id}/activities` - Add activity (call, email, note)
- `GET /api/v1/leads/{id}/activities` - Get all activities for a lead

### Query Parameters
- `status`: Filter by lead status (New, Qualified, Contacted, Quoted, Won, Lost)
- `from`: Filter leads created from this date
- `to`: Filter leads created until this date
- Pagination: Standard pagination with Sieve filtering

---

## Use Cases & Examples

### Example 1: Web Form Submission
```
1. Customer fills form on website
   → Creates Contact: "John Doe", "john@email.com", "123-456-7890"
   → Creates Prospect: Links to John's contact
   → Creates Lead: Status=New, Source="Web"
   
2. Sales team reviews lead
   → Updates Lead Status: New → Qualified
   → Adds Activity: Kind="note", Notes="Interested in health insurance"
   
3. Sales agent contacts customer
   → Updates Lead Status: Qualified → Contacted
   → Adds Activity: Kind="call", Notes="Discussed coverage options"
   
4. Quote sent
   → Updates Lead Status: Contacted → Quoted
   → Adds Activity: Kind="email", Notes="Sent quote for Plan A"
   
5. Customer accepts
   → Updates Lead Status: Quoted → Won
   → Adds Activity: Kind="note", Notes="Customer accepted, policy created"
```

### Example 2: Referral Lead
```
1. Existing customer refers a friend
   → Creates Contact: "Jane Smith", "jane@email.com", "987-654-3210"
   → Creates Prospect: Links to Jane's contact
   → Creates Lead: Status=New, Source="Referral", OwnerUserId=assignedAgentId
   
2. Agent follows up
   → Updates Lead Status: New → Contacted
   → Adds Activity: Kind="call", Notes="Thanked for referral, discussed needs"
```

### Example 3: Lost Lead
```
1. Lead reaches Quoted stage
2. Customer decides not to proceed
   → Updates Lead Status: Quoted → Lost
   → Adds Activity: Kind="note", Notes="Customer chose competitor"
```

---

## Best Practices

### ✅ DO:
- Always create leads through the proper API endpoint (creates full hierarchy)
- Use activities to track all interactions
- Update lead status as it progresses
- Assign leads to appropriate team members
- Use proper status transitions

### ❌ DON'T:
- Don't manually create Contacts/Prospects without Leads (unless for specific business needs)
- Don't skip status transitions (e.g., New → Quoted)
- Don't forget to log activities for important interactions
- Don't leave leads unassigned for long periods

---

## Technical Implementation

### Transactions
All write operations use **database transactions** to ensure data integrity:
- `CreateLeadAsync`: Transaction wraps Contact → Prospect → Lead creation
- `AddActivityAsync`: Transaction ensures activity is properly linked
- `UpdateStatusAsync`: Transaction ensures status change is atomic

### Async/Await
All database operations use **async/await** patterns:
- `AddAsync()` - Async entity addition
- `SaveChangesAsync()` - Async save operations
- `ToListAsync()` - Async query execution
- `FirstOrDefaultAsync()` - Async single record retrieval

### Error Handling
- Transactions rollback on errors
- Proper exception logging
- Result pattern for operation outcomes
- Validation before state changes

---

## Summary

| Entity | Purpose | Key Property | Relationship |
|--------|---------|--------------|--------------|
| **Contact** | Person's information | FullName, Email, Phone | Can be primary contact for many Prospects |
| **Prospect** | Business opportunity | PrimaryContactId | Can be associated with one Lead |
| **Lead** | Sales pipeline entry | Status, Source, OwnerUserId | Has many Activities |

**The hierarchy ensures:**
- ✅ Clean separation of concerns
- ✅ Reusable contact information
- ✅ Proper sales pipeline tracking
- ✅ Complete activity history
- ✅ Multi-tenant data isolation

