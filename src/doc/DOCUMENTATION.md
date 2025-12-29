# Project Documentation

## Architecture Overview

This project follows Clean Architecture principles with CQRS and Mediator patterns. Here's a breakdown of the key components:

### Project Structure
- **Web**: API endpoints and configuration
- **Business**: Application logic and use cases
- **Data**: Database entities and EF Core configuration
- **SharedKernel**: Common utilities and constants
- **Infrastructure**: External services integration

### Key Features

1. **CQRS with MediatR**
   - Commands for write operations
   - Queries for read operations
   - Validation using FluentValidation
   - Pipeline behaviors for cross-cutting concerns

2. **Database**
   - PostgreSQL with Entity Framework Core
   - Audit logging
   - Soft delete functionality

3. **API Features**
   - API versioning
   - Swagger documentation
   - Response compression
   - Rate limiting
   - Correlation ID tracking

4. **Monitoring**
   - Health checks
   - Serilog logging
   - Request/Response logging

### Design Patterns
- Repository Pattern
- CQRS Pattern
- Mediator Pattern
- Unit of Work
- Specification Pattern

## Getting Started

1. Install dependencies 