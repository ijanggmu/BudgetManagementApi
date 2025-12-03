#!/bin/bash

###############################################################################
# Database Backup Script for BeemaEdge
# 
# This script creates backups of all three PostgreSQL databases
# Backups are stored with timestamps and can be restored using restore-databases.sh
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

# Configuration
BACKUP_DIR="${BACKUP_DIR:-./backups}"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
RETENTION_DAYS="${RETENTION_DAYS:-30}"

# Database connection details (from environment or docker-compose)
DB_HOST="${POSTGRES_HOST:-postgres}"
DB_PORT="${POSTGRES_PORT:-5432}"
DB_USER="${POSTGRES_USER:-beemaedge}"
DB_PASSWORD="${POSTGRES_PASSWORD}"

AUDIT_DB_HOST="${POSTGRES_AUDIT_HOST:-postgres-audit}"
AUDIT_DB_PORT="${POSTGRES_AUDIT_PORT:-5432}"
AUDIT_DB_USER="${POSTGRES_AUDIT_USER:-beemaedge_audit}"
AUDIT_DB_PASSWORD="${POSTGRES_AUDIT_PASSWORD}"

HANGFIRE_DB_HOST="${POSTGRES_HANGFIRE_HOST:-postgres-hangfire}"
HANGFIRE_DB_PORT="${POSTGRES_HANGFIRE_PORT:-5432}"
HANGFIRE_DB_USER="${POSTGRES_HANGFIRE_USER:-beemaedge_hangfire}"
HANGFIRE_DB_PASSWORD="${POSTGRES_HANGFIRE_PASSWORD}"

# Create backup directory
mkdir -p "$BACKUP_DIR"

print_info "Starting database backups..."
print_info "Backup directory: $BACKUP_DIR"
print_info "Timestamp: $TIMESTAMP"

# Function to backup a database
backup_database() {
    local DB_NAME=$1
    local DB_HOST=$2
    local DB_PORT=$3
    local DB_USER=$4
    local DB_PASSWORD=$5
    local BACKUP_FILE="$BACKUP_DIR/${DB_NAME}_${TIMESTAMP}.sql.gz"
    
    print_info "Backing up database: $DB_NAME"
    
    if command -v pg_dump &> /dev/null; then
        # Local pg_dump
        PGPASSWORD="$DB_PASSWORD" pg_dump -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" | gzip > "$BACKUP_FILE"
    elif docker ps | grep -q "$DB_HOST"; then
        # Docker container
        docker exec "$DB_HOST" pg_dump -U "$DB_USER" -d "$DB_NAME" | gzip > "$BACKUP_FILE"
    else
        print_error "Cannot backup $DB_NAME: pg_dump not found and container not running"
        return 1
    fi
    
    if [ -f "$BACKUP_FILE" ] && [ -s "$BACKUP_FILE" ]; then
        print_info "✓ Backup created: $BACKUP_FILE ($(du -h "$BACKUP_FILE" | cut -f1))"
        return 0
    else
        print_error "✗ Backup failed for $DB_NAME"
        return 1
    fi
}

# Backup Application Database
backup_database "beemaedge_db" "$DB_HOST" "$DB_PORT" "$DB_USER" "$DB_PASSWORD" || exit 1

# Backup Audit Database
backup_database "beemaedge_audit_db" "$AUDIT_DB_HOST" "$AUDIT_DB_PORT" "$AUDIT_DB_USER" "$AUDIT_DB_PASSWORD" || exit 1

# Backup Hangfire Database
backup_database "beemaedge_hangfire_db" "$HANGFIRE_DB_HOST" "$HANGFIRE_DB_PORT" "$HANGFIRE_DB_USER" "$HANGFIRE_DB_PASSWORD" || exit 1

# Cleanup old backups
print_info "Cleaning up backups older than $RETENTION_DAYS days..."
find "$BACKUP_DIR" -name "*.sql.gz" -type f -mtime +$RETENTION_DAYS -delete
print_info "Cleanup completed"

print_info "All backups completed successfully!"
print_info "Backup location: $BACKUP_DIR"



