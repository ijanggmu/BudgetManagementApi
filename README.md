# Base Code

## Overview

This is a base API built with .NET 8, following n-tier architecture principles. The project leverages the 
Sieve package for efficient pagination and filtering, uses PostgreSQL as the database, and Entity Framework Core
as the Object-Relational Mapper (ORM). Additionally, Hangfire is integrated for background job processing, 
have Ratelimit,and Serilog is used for advanced logging.To ensure if the database are healthy or not got Health Check api as well.

## Table of Contents

- [WebApi Code](#webapi-code)
  - [Overview](#overview)
  - [Table of Contents](#table-of-contents)
  - [Getting Started](#getting-started)
  - [Architecture](#architecture)
    - [Database Contexts](#database-contexts)
      - [RateLimit Configuration](#ratelimit-configuration)
      - [Serilog Configuration](#serilog-configuration)
      - [Sieve Filter](#sieve-filter)
        - [Operators](#operators)
        - [Conditions in Sieve Filters](#conditions-in-sieve-filters)
        - [Filter Query Request Examples](#filter-query-request-examples)
  - [Filter Query Request Example](#filter-query-request-example)
  - [Global Search](#global-search)
  - [Running the Application](#running-the-application)
  - [Testing](#testing)
  - [API Documentation](#api-documentation)
  - [Contributing](#contributing)
  - [License](#license)

## Getting Started

To get a local copy up and running, follow the steps below.

## Architecture

The project follows the n-tier architecture, splitting responsibilities into different layers:
- **Presentation Layer**: Handles API requests and responses.
   - Web
- **Application Layer**: Contains business logic and service contracts.
-  - Business
-  - Model
- **Domain Layer**: Represents the core business entities and domain logic.
-  - Data
- **Infrastructure Layer**: Handles data access, third-party services, and external dependencies, including background job processing and logging.
- **SharedKernel**: Contains common model , helper,etc

### Database Contexts

The project utilizes three different database contexts:

- **ApplicationDataContext**: Core database operations, with migrations stored in the `Migrations/Core` folder.
  ```bash
    add-migration <message> -c ApplicationDataContext -o Migrations/Core
    ```
    For Example
     ```bash
    add-migration ":sparkles: added-feature" -c ApplicationDataContext -o Migrations/Core
    ```
- **AuditContext**: Handles auditing-related data, with migrations stored in the `Migrations/Audit` folder.
    ```bash
    add-migration <message> -c AuditDataContext -o Migrations/Core
    ```
    For Example
     ```bash
    add-migration ":sparkles: added-feature" -c AuditDataContext -o Migrations/Core
    ```
- **HangfireContext**: Manages Hangfire jobs, with migrations stored in the `Migrations/Hangfire` folder.
  ```bash
    add-migration <message> -c HangFireDataContext -o Migrations/Core
    ```
    For Example
     ```bash
    add-migration ":sparkles: added-feature" -c HangFireDataContext -o Migrations/Core
    ```

## Technologies Used

- **.NET 8**
- **Entity Framework Core**: ORM for database access
- **PostgreSQL**: Relational database
- **Sieve**: Library for pagination, filtering, and sorting
- **Hangfire**: Background job processing
- **Serilog**: Structured logging
- **Swagger**: API documentation

## Setting Up the Project

### Prerequisites

Ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/)

### Installation

1. Clone the repository:
    ```bash
    git clone https://repo.digitalhei.com/customerportal/customer-portal-backend.git
    ```
    ```bash
    cd your-repo-name
    ```

2. Restore NuGet packages:
    ```bash
    dotnet restore
    ```

3. Apply database migrations:
    ```bash
    dotnet ef database update --context ApplicationDataContext
    ```
    ```bash
    dotnet ef database update --context AuditContext
    ```
    ```bash
    dotnet ef database update --context HangfireContext
    ```

### Configuration

#### appsettings.json

The project uses a **common `appsettings.json`** for base configuration and **environment-specific `appsettings.{Environment}.json`** files for environment-specific configurations (e.g., `appsettings.Development.json`, `appsettings.Production.json`).

The configuration files are automatically loaded based on the environment. For example, when running in development, `appsettings.Development.json` will be merged with `appsettings.json`.

#### User Secrets

For sensitive information such as database connection strings, API keys, and other secrets, **User Secrets** are used during development. This avoids the need to store sensitive data directly in the `appsettings.json` files.

To manage User Secrets:

1. Right-click the project in Visual Studio and select **Manage User Secrets**.
2. Add your secrets in the `secrets.json` file that opens up.

Example `secrets.json`:

```json
{
  "ConnectionStrings": {
    "HangFireDefaultConnection": "User ID=postgres;Password=Admin@123;Host=localhost;Port=5432;Database=db_hangfires;Pooling=true;",
    "DefaultConnection": "User ID=postgres;Password=Admin@123;Host=localhost;Port=5432;Database=db_core;Pooling=true;",
    "AuditDefaultConnection": "User ID=postgres;Password=Admin@123;Host=localhost;Port=5432;Database=db_audit;Pooling=true;"
  },
  "JobConfig": {
    "HangfireUsername": "Hangfire-Admin",
    "HangfirePassword": "JtW;+5-p_heNPTds"
  },
  "AesConfig": {
    "Key": "e2fwHRm+6YLwmnaRJfqO0B5ubMHnQc8TcnvsChknFoE=",
    "IV": "oalCxl2jB3S5lxlwOFZNiQ=="
  }, 
  "Jwt:Key":"9c6b3246178c2fee36db797adbadfc073b54561868d21d5d84423c03ee19af57d1496113ed1828682905acb86ebbc3b5127a2e504e06881eb65601719c68ef71",
}
```

#### RateLimit Configuration

RateLimit is configured in the `appsettings.json` file. You can customize the request limit per minutes, hours in window ,response code ,enable disable the rate limiting function according to your needs.

Example configuration:

```json
 "RateLimitOptions": {
    "EnableRateLimiting": true,
    "PermitLimitInMinutes": 300,
    "PermitLimitInHours": 8000,
    "WindowInMinutes": 1,
    "WindowInHours": 1,
    "RejectionStatusCode": 429
  }
```
#### Serilog Configuration

Serilog is configured in the `appsettings.json` file. You can customize the logging level, output template, and sinks according to your needs.

Example configuration:

```json
"Serilog": {
  "MinimumLevel": "Information",
  "WriteTo": [
    {
      "Name": "Console"
    },
    {
      "Name": "File",
      "Args": {
        "path": "Logs/log-.txt",
        "rollingInterval": "Day"
      }
    }
  ],
  "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
  "Properties": {
    "Application": "BaseAPI"
  }
}
```
#### Sieve Filter

Sieve is a lightweight filtering, sorting, and pagination library for .NET. It allows you to construct complex queries with simple syntax, enabling powerful search and filter capabilities in your API. This document outlines the operators, conditions, and example requests that can be used with Sieve filters.

##### Operators

| Operator | Description                                      | Example Usage             |
|----------|--------------------------------------------------|---------------------------|
| `==`     | Equals                                           | `Name==John`              |
| `!=`     | Not equals                                       | `Status!=Active`          |
| `>`      | Greater than                                     | `Age>30`                  |
| `<`      | Less than                                        | `Age<30`                  |
| `>=`     | Greater than or equal to                         | `Age>=30`                 |
| `<=`     | Less than or equal to                            | `Age<=30`                 |
| `@=`     | Contains                                         | `Name@=John`              |
| `_=`     | Starts with                                      | `Name_=John`              |
| `_-=*`   | Ends with                                        | `Name_-=Smith`            |
| `!@=`    | Does not contain                                 | `Name!@=John`             |
| `!_=`    | Does not start with                              | `Name!_=John`             |
| `!_-=*`  | Does not end with                                | `Name!_-=Smith`           |
| `@=*`    | Case-insensitive string contains                 | `Name@=*john`             |
| `_=*`    | Case-insensitive string starts with              | `Name_*=john`             |
| `_-=*`   | Case-insensitive string ends with                | `Name_-=*smith`           |
| `==*`    | Case-insensitive string equals                   | `Name==*john`             |
| `!=*`    | Case-insensitive string not equals               | `Name!=*john`             |
| `!@=*`   | Case-insensitive string does not contain         | `Name!@=*john`            |
| `!_=*`   | Case-insensitive string does not start with      | `Name!_*=john`            |

##### Conditions in Sieve Filters

- `|` : OR
- `,` : AND
- `-` : Sort By Descending

##### Filter Query Request Examples

## Filter Query Request Example

```json
{
  "filters": "Name@=Ejan|Fitzgerald,IsDisable==false",
  "Sorts": "Name,-CreatedOn",
  "page": 1,
  "pageSize": 10
}
```

```json
{
  "filters": "CreatedOn>=startDate,CreatedOn<=endDate",
  "Sorts": "Name,-CreatedOn",
  "page": 1,
  "pageSize": 10
}
```

## Global Search

```json
{
  "query": "(Name|email)@=z",
  "Sorts": "Name,-CreatedOn",
  "page": 1,
  "pageSize": 10
}
```

```json
{
  "query": "(name|email)@=z",
  "filters": "isDisable==true",
  "Sorts": "Name,-CreatedOn",
  "pageNumber": 1,
  "pageSize": 10
}
```

## Running the Application
To run the application locally, use the following command:
 ```bash
    dotnet run
 ```

## Testing
To run unit tests, use the following command:
```bash
   dotnet test
 ```
## API Documentation
Swagger is used for API documentation. Once the application is running, you can access the documentation.
## Contributing
Contributions are welcome! Please open an issue or submit a pull request for any features, bug fixes, or suggestions.
## License
This project is licensed under the MIT License - see the LICENSE file for details.

### Authentication

The API uses JWT (JSON Web Token) based authentication with cookie-based token storage for enhanced security. Authentication is configured with the following features:

#### Authentication Flow
1. **Login**: Users authenticate with username/password
2. **Token Storage**: Access and refresh tokens are stored in HTTP-only secure cookies
3. **Auto Refresh**: Tokens are automatically refreshed when expired
4. **Logout**: Clears authentication cookies

#### Security Features
- HTTP-only cookies prevent XSS attacks
- Strict/None SameSite policy based on environment
- Secure flag enabled for HTTPS
- Short-lived access tokens (configurable)
- Refresh token rotation

#### Identity Configuration

```
{
  "Identity": {
    "Password": {
      "RequiredLength": 8,
      "RequireDigit": true,
      "RequireLowercase": true,
      "RequireUppercase": true,
      "RequireNonAlphanumeric": false
    },
    "Lockout": {
      "DefaultLockoutTimeSpan": "00:05:00",
      "MaxFailedAccessAttempts": 5
    }
  }
}
```

#### Authentication Endpoints

| Endpoint | Method | Description | Authentication |
|----------|--------|-------------|----------------|
| `/api/v1/Identity/Login` | POST | Authenticate user | No |
| `/api/v1/Identity/Register` | POST | Register new user | No |
| `/api/v1/Identity/Refresh` | GET | Refresh access token | No |
| `/api/v1/Identity/Logout` | POST | Logout user | Yes |
| `/api/v1/Identity/ChangePassword` | PUT | Change password | Yes |

#### Cookie Names
- `X-Access-Token`: JWT access token
- `X-Refresh-Token`: Refresh token
- `X-Username`: Authenticated username
- `X-Access-Token-ExpiryInSeconds`: Access token expiry
- `X-Refresh-ExpiryInSeconds`: Refresh token expiry

### CORS Configuration

CORS (Cross-Origin Resource Sharing) is configured to allow specific origins with credentials. Configure allowed origins in your environment-specific settings:

```

### JWT Authentication & HTTP-Only Cookie Implementation

#### Overview
The application implements a secure authentication system using JWTs (JSON Web Tokens) stored in HTTP-only cookies. This approach combines the benefits of JWT-based authentication with enhanced security features of HTTP-only cookies.

#### JWT Token Architecture

1. **Token Types**
   - **Access Token**: Short-lived token for API authentication
   - **Refresh Token**: Long-lived token for obtaining new access tokens
   
2. **Token Storage**
   ```
   HTTP-Only Cookies
   ├── X-Access-Token
   ├── X-Refresh-Token
   ├── X-Username
   ├── X-Access-Token-ExpiryInSeconds
   └── X-Refresh-ExpiryInSeconds
   ```

#### Security Implementation

1. **HTTP-Only Cookie Protection**
   - Cookies are marked as HTTP-only, preventing JavaScript access
   - Protects against XSS (Cross-Site Scripting) attacks
   - Cookies are only transmitted over HTTPS (Secure flag)
   ```csharp
   new CookieOptions
   {
       HttpOnly = true,
       SameSite = SameSiteMode.Strict,
       Secure = true,
       Expires = expiryTime
   }
   ```

2. **JWT Token Configuration**
   ```json
   {
     "Jwt": {
       "JWTExpiresInMinutes": "480",
       "RefreshExpiresInMinutes": "10080",
       "Key": "your-secret-key"
     }
   }
   ```

#### Authentication Flow Process

1. **Login Process**
   ```mermaid
   sequenceDiagram
      Client->>+API: POST /api/v1/Identity/Login
      API->>API: Validate Credentials
      API->>API: Generate JWT & Refresh Token
      API->>Client: Set HTTP-only Cookies
   ```

2. **API Request Flow**
   ```mermaid
   sequenceDiagram
      Client->>+API: Request with Cookie
      API->>API: Validate JWT from Cookie
      alt Valid Token
          API->>Client: Return Response
      else Invalid Token
          API->>Client: 401 Unauthorized
      end
   ```

3. **Token Refresh Process**
   ```mermaid
   sequenceDiagram
      Client->>+API: GET /api/v1/Identity/Refresh
      API->>API: Validate Refresh Token
      alt Valid Refresh Token
          API->>API: Generate New Tokens
          API->>Client: Set New HTTP-only Cookies
      else Invalid Refresh Token
          API->>Client: 401 Unauthorized
      end
   ```

#### Security Benefits

1. **XSS Protection**
   - HTTP-only cookies prevent JavaScript access to tokens
   - Mitigates risk of token theft via XSS attacks

2. **CSRF Protection**
   - SameSite cookie attribute prevents CSRF attacks
   - Additional CSRF tokens for sensitive operations

3. **Token Security**
   - Short-lived access tokens (configurable duration)
   - Refresh token rotation on use
   - Automatic token cleanup

#### Implementation Details

1. **Token Generation**
   ```csharp
   var tokenDescriptor = new SecurityTokenDescriptor
   {
       Subject = new ClaimsIdentity(claims),
       Expires = DateTime.UtcNow.AddMinutes(expiryTime),
       SigningCredentials = credentials
   };
   ```

2. **Cookie Settings**
   ```csharp
   context.Response.Cookies.Append("X-Access-Token", accessToken,
       new CookieOptions
       {
           HttpOnly = true,
           SameSite = SameSiteMode.Strict,
           Secure = true,
           Expires = expiryTime
       });
   ```

3. **Token Validation**
   ```csharp
   var tokenValidationParameters = new TokenValidationParameters
   {
       ValidateIssuerSigningKey = true,
       IssuerSigningKey = key,
       ValidateIssuer = false,
       ValidateAudience = false,
       ValidateLifetime = true,
       ClockSkew = TimeSpan.FromMinutes(1)
   };
   ```

#### Best Practices Implemented

1. **Token Management**
   - Automatic token refresh
   - Token revocation on logout
   - Concurrent session handling

2. **Security Headers**
   - Strict Transport Security (HSTS)
   - Content Security Policy (CSP)
   - X-Frame-Options

3. **Error Handling**
   - Secure error messages
   - Proper HTTP status codes
   - Audit logging for security events