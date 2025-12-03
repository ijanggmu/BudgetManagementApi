#!/bin/bash

###############################################################################
# Database Restore Script for BeemaEdge
# 
# This script restores PostgreSQL databases from backup files
# Usage: ./restore-databases.sh <backup-file-prefix>
# Example: ./restore-databases.sh 20240115_120000
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
TIMESTAMP="${1}"

if [ -z "$TIMESTAMP" ]; then
    print_error "Usage: $0 <backup-timestamp>"
    print_info "Example: $0 20240115_120000"
    print_info "Available backups:"
    ls -1 "$BACKUP_DIR"/*.sql.gz 2>/dev/null | sed 's/.*_\([0-9_]*\)\.sql\.gz/\1/' | sort -u || echo "No backups found"
    exit 1
fi

# Database connection details
DB_HOST="${POSTGRES_HOST:-postgres}"
DB_PORT="${POSTGRES_PORT:-5432}"
DB_USER="${POSTGRES_USER:-beemaedge}"
DB_PASSWORD="${POSTGRES_PASSWORD}"
DB_NAME="beemaedge_db"

AUDIT_DB_HOST="${POSTGRES_AUDIT_HOST:-postgres-audit}"
AUDIT_DB_PORT="${POSTGRES_AUDIT_PORT:-5432}"
AUDIT_DB_USER="${POSTGRES_AUDIT_USER:-beemaedge_audit}"
AUDIT_DB_PASSWORD="${POSTGRES_AUDIT_PASSWORD}"
AUDIT_DB_NAME="beemaedge_audit_db"

HANGFIRE_DB_HOST="${POSTGRES_HANGFIRE_HOST:-postgres-hangfire}"
HANGFIRE_DB_PORT="${POSTGRES_HANGFIRE_PORT:-5432}"
HANGFIRE_DB_USER="${POSTGRES_HANGFIRE_USER:-beemaedge_hangfire}"
HANGFIRE_DB_PASSWORD="${POSTGRES_HANGFIRE_PASSWORD}"
HANGFIRE_DB_NAME="beemaedge_hangfire_db"

print_warn "WARNING: This will restore databases from backup timestamp: $TIMESTAMP"
print_warn "This operation will REPLACE existing data!"
read -p "Are you sure you want to continue? (yes/no): " CONFIRM

if [ "$CONFIRM" != "yes" ]; then
    print_info "Restore cancelled"
    exit 0
fi

# Function to restore a database
restore_database() {
    local DB_NAME=$1
    local DB_HOST=$2
    local DB_PORT=$3
    local DB_USER=$4
    local DB_PASSWORD=$5
    local BACKUP_FILE="$BACKUP_DIR/${DB_NAME}_${TIMESTAMP}.sql.gz"
    
    if [ ! -f "$BACKUP_FILE" ]; then
        print_error "Backup file not found: $BACKUP_FILE"
        return 1
    fi
    
    print_info "Restoring database: $DB_NAME from $BACKUP_FILE"
    
    if command -v psql &> /dev/null && command -v gunzip &> /dev/null; then
        # Local restore
        gunzip -c "$BACKUP_FILE" | PGPASSWORD="$DB_PASSWORD" psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME"
    elif docker ps | grep -q "$DB_HOST"; then
        # Docker container restore
        gunzip -c "$BACKUP_FILE" | docker exec -i "$DB_HOST" psql -U "$DB_USER" -d "$DB_NAME"
    else
        print_error "Cannot restore $DB_NAME: psql not found and container not running"
        return 1
    fi
    
    if [ $? -eq 0 ]; then
        print_info "✓ Database restored: $DB_NAME"
        return 0
    else
        print_error "✗ Restore failed for $DB_NAME"
        return 1
    fi
}

# Restore Application Database
restore_database "$DB_NAME" "$DB_HOST" "$DB_PORT" "$DB_USER" "$DB_PASSWORD" || exit 1

# Restore Audit Database
restore_database "$AUDIT_DB_NAME" "$AUDIT_DB_HOST" "$AUDIT_DB_PORT" "$AUDIT_DB_USER" "$AUDIT_DB_PASSWORD" || exit 1

# Restore Hangfire Database
restore_database "$HANGFIRE_DB_NAME" "$HANGFIRE_DB_HOST" "$HANGFIRE_DB_PORT" "$HANGFIRE_DB_USER" "$HANGFIRE_DB_PASSWORD" || exit 1

print_info "All databases restored successfully!"



