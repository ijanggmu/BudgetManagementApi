#!/bin/bash

###############################################################################
# BeemaEdge Amazon Lightsail Deployment Script
# 
# This script automates the deployment of BeemaEdge to Amazon Lightsail
# Prerequisites:
#   - AWS CLI configured with appropriate credentials
#   - Lightsail instance created and accessible via SSH
#   - Docker and Docker Compose installed on Lightsail instance
###############################################################################

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
INSTANCE_NAME="${LIGHTSAIL_INSTANCE_NAME:-beemaedge-production}"
SSH_USER="${SSH_USER:-ubuntu}"
DEPLOY_DIR="${DEPLOY_DIR:-/opt/beemaedge}"
BACKUP_DIR="${BACKUP_DIR:-/opt/beemaedge-backups}"

echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}BeemaEdge Lightsail Deployment${NC}"
echo -e "${GREEN}========================================${NC}"

# Function to print colored messages
print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if AWS CLI is installed
if ! command -v aws &> /dev/null; then
    print_error "AWS CLI is not installed. Please install it first."
    exit 1
fi

# Check if .env.production exists
if [ ! -f ".env.production" ]; then
    print_error ".env.production file not found!"
    print_info "Please copy .env.production.example to .env.production and configure it."
    exit 1
fi

# Get Lightsail instance IP
print_info "Getting Lightsail instance IP..."
INSTANCE_IP=$(aws lightsail get-instance --instance-name "$INSTANCE_NAME" --query 'instance.publicIpAddress' --output text 2>/dev/null || echo "")

if [ -z "$INSTANCE_IP" ]; then
    print_error "Could not find Lightsail instance: $INSTANCE_NAME"
    print_info "Please ensure the instance exists and AWS credentials are configured."
    exit 1
fi

print_info "Instance IP: $INSTANCE_IP"

# Test SSH connection
print_info "Testing SSH connection..."
if ! ssh -o ConnectTimeout=10 -o StrictHostKeyChecking=no "$SSH_USER@$INSTANCE_IP" "echo 'SSH connection successful'" &> /dev/null; then
    print_error "Cannot connect to instance via SSH"
    print_info "Please ensure:"
    print_info "  1. SSH key is configured in Lightsail"
    print_info "  2. Security group allows SSH (port 22)"
    print_info "  3. Instance is running"
    exit 1
fi

print_info "SSH connection successful!"

# Create deployment directory structure on remote
print_info "Setting up deployment directory on remote server..."
ssh "$SSH_USER@$INSTANCE_IP" << 'ENDSSH'
    set -e
    sudo mkdir -p /opt/beemaedge
    sudo mkdir -p /opt/beemaedge-backups
    sudo chown -R $USER:$USER /opt/beemaedge
    sudo chown -R $USER:$USER /opt/beemaedge-backups
ENDSSH

# Copy files to remote server
print_info "Copying files to remote server..."

# Create a temporary directory for files to copy
TEMP_DIR=$(mktemp -d)
trap "rm -rf $TEMP_DIR" EXIT

# Copy necessary files
cp docker-compose.production.yml "$TEMP_DIR/"
cp .env.production "$TEMP_DIR/"
cp -r src "$TEMP_DIR/"
cp BeemaEdge.sln "$TEMP_DIR/" 2>/dev/null || true

# Copy Dockerfile
mkdir -p "$TEMP_DIR/src/Web"
cp src/Web/Dockerfile "$TEMP_DIR/src/Web/"

# Create deployment package
print_info "Creating deployment package..."
tar -czf "$TEMP_DIR/beemaedge-deploy.tar.gz" -C "$TEMP_DIR" .

# Transfer to remote
print_info "Transferring files to remote server..."
scp "$TEMP_DIR/beemaedge-deploy.tar.gz" "$SSH_USER@$INSTANCE_IP:/tmp/"

# Extract and deploy on remote
print_info "Deploying on remote server..."
ssh "$SSH_USER@$INSTANCE_IP" << ENDSSH
    set -e
    
    # Extract files
    cd $DEPLOY_DIR
    tar -xzf /tmp/beemaedge-deploy.tar.gz
    rm /tmp/beemaedge-deploy.tar.gz
    
    # Ensure Docker and Docker Compose are installed
    if ! command -v docker &> /dev/null; then
        echo "Installing Docker..."
        curl -fsSL https://get.docker.com -o get-docker.sh
        sudo sh get-docker.sh
        sudo usermod -aG docker $USER
        rm get-docker.sh
    fi
    
    if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
        echo "Installing Docker Compose..."
        sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-\$(uname -s)-\$(uname -m)" -o /usr/local/bin/docker-compose
        sudo chmod +x /usr/local/bin/docker-compose
    fi
    
    # Stop existing containers if running
    if [ -f docker-compose.production.yml ]; then
        echo "Stopping existing containers..."
        docker-compose -f docker-compose.production.yml down || true
    fi
    
    # Pull latest images
    echo "Pulling latest images..."
    docker-compose -f docker-compose.production.yml pull || docker compose -f docker-compose.production.yml pull
    
    # Build and start containers
    echo "Building and starting containers..."
    docker-compose -f docker-compose.production.yml up -d --build || docker compose -f docker-compose.production.yml up -d --build
    
    # Wait for services to be healthy
    echo "Waiting for services to be healthy..."
    sleep 30
    
    # Check service status
    docker-compose -f docker-compose.production.yml ps || docker compose -f docker-compose.production.yml ps
    
    echo "Deployment completed!"
ENDSSH

print_info "Deployment completed successfully!"
print_info "Application should be available at: http://$INSTANCE_IP:8080"
print_info "Seq UI should be available at: http://$INSTANCE_IP:5341"


