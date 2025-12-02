#!/bin/bash

###############################################################################
# Health Check Script for BeemaEdge
# 
# This script checks the health of all services
# Can be used with monitoring systems or cron jobs
###############################################################################

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

EXIT_CODE=0

check_service() {
    local SERVICE_NAME=$1
    local CHECK_COMMAND=$2
    
    if eval "$CHECK_COMMAND" &> /dev/null; then
        echo -e "${GREEN}✓${NC} $SERVICE_NAME is healthy"
        return 0
    else
        echo -e "${RED}✗${NC} $SERVICE_NAME is unhealthy"
        EXIT_CODE=1
        return 1
    fi
}

echo "BeemaEdge Health Check"
echo "====================="

# Check Docker containers
if command -v docker &> /dev/null; then
    check_service "Docker" "docker ps"
    
    # Check specific containers
    check_service "PostgreSQL (Main)" "docker ps | grep -q beemaedge-postgres"
    check_service "PostgreSQL (Audit)" "docker ps | grep -q beemaedge-postgres-audit"
    check_service "PostgreSQL (Hangfire)" "docker ps | grep -q beemaedge-postgres-hangfire"
    check_service "Seq" "docker ps | grep -q beemaedge-seq"
    check_service "BeemaEdge API" "docker ps | grep -q beemaedge-api"
    
    # Check API health endpoint
    API_URL="${API_URL:-http://localhost:8080/health}"
    check_service "API Health Endpoint" "curl -f -s $API_URL > /dev/null"
    
    # Check database connections
    check_service "PostgreSQL Connection (Main)" "docker exec beemaedge-postgres pg_isready -U beemaedge"
    check_service "PostgreSQL Connection (Audit)" "docker exec beemaedge-postgres-audit pg_isready -U beemaedge_audit"
    check_service "PostgreSQL Connection (Hangfire)" "docker exec beemaedge-postgres-hangfire pg_isready -U beemaedge_hangfire"
    
    # Check Seq
    SEQ_URL="${SEQ_URL:-http://localhost:5341/api}"
    check_service "Seq API" "curl -f -s $SEQ_URL > /dev/null"
else
    echo -e "${YELLOW}⚠${NC} Docker not found, skipping container checks"
fi

# Check disk space
DISK_USAGE=$(df -h / | awk 'NR==2 {print $5}' | sed 's/%//')
if [ "$DISK_USAGE" -lt 80 ]; then
    echo -e "${GREEN}✓${NC} Disk space: ${DISK_USAGE}% used"
else
    echo -e "${RED}✗${NC} Disk space: ${DISK_USAGE}% used (warning: >80%)"
    EXIT_CODE=1
fi

# Check memory
if command -v free &> /dev/null; then
    MEM_USAGE=$(free | awk 'NR==2{printf "%.0f", $3*100/$2}')
    if [ "$MEM_USAGE" -lt 90 ]; then
        echo -e "${GREEN}✓${NC} Memory: ${MEM_USAGE}% used"
    else
        echo -e "${YELLOW}⚠${NC} Memory: ${MEM_USAGE}% used (warning: >90%)"
    fi
fi

echo "====================="
if [ $EXIT_CODE -eq 0 ]; then
    echo -e "${GREEN}All checks passed${NC}"
else
    echo -e "${RED}Some checks failed${NC}"
fi

exit $EXIT_CODE


