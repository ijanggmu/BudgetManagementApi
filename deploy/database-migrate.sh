#!/bin/bash

###############################################################################
# Database Migration Script for BeemaEdge
# 
# This script runs EF Core migrations on all three databases:
#   - Application Database (DefaultConnection)
#   - Audit Database (AuditDefaultConnection)
#   - Hangfire Database (HangFireDefaultConnection)
###############################################################################

set -e

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if running in Docker or locally
if [ -f /.dockerenv ] || [ -n "$DOCKER_CONTAINER" ]; then
    print_info "Running inside Docker container..."
    DOTNET_CMD="dotnet"
    PROJECT_PATH="/app/BeemaEdgeApi.dll"
else
    print_info "Running locally..."
    DOTNET_CMD="dotnet"
    PROJECT_PATH="src/Web/BeemaEdgeApi.csproj"
fi

print_info "Starting database migrations..."

# Migrate Application Database
print_info "Migrating Application Database (ApplicationDataContext)..."
if $DOTNET_CMD ef database update --context ApplicationDataContext --project "$PROJECT_PATH" 2>/dev/null || \
   docker-compose exec beemaedge-api dotnet ef database update --context ApplicationDataContext --project /app/BeemaEdgeApi.csproj 2>/dev/null; then
    print_info "✓ Application Database migration completed"
else
    print_error "✗ Application Database migration failed"
    exit 1
fi

# Migrate Audit Database
print_info "Migrating Audit Database (AuditDataContext)..."
if $DOTNET_CMD ef database update --context AuditDataContext --project "$PROJECT_PATH" 2>/dev/null || \
   docker-compose exec beemaedge-api dotnet ef database update --context AuditDataContext --project /app/BeemaEdgeApi.csproj 2>/dev/null; then
    print_info "✓ Audit Database migration completed"
else
    print_error "✗ Audit Database migration failed"
    exit 1
fi

# Migrate Hangfire Database
print_info "Migrating Hangfire Database (HangfireDataContext)..."
if $DOTNET_CMD ef database update --context HangfireDataContext --project "$PROJECT_PATH" 2>/dev/null || \
   docker-compose exec beemaedge-api dotnet ef database update --context HangfireDataContext --project /app/BeemaEdgeApi.csproj 2>/dev/null; then
    print_info "✓ Hangfire Database migration completed"
else
    print_error "✗ Hangfire Database migration failed"
    exit 1
fi

print_info "All database migrations completed successfully!"


