# BeemaEdge Amazon Lightsail Deployment Guide

This guide provides step-by-step instructions for deploying BeemaEdge to Amazon Lightsail with PostgreSQL and Seq logging.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Lightsail Instance Setup](#lightsail-instance-setup)
3. [Configuration](#configuration)
4. [Deployment](#deployment)
5. [Database Setup](#database-setup)
6. [Monitoring and Maintenance](#monitoring-and-maintenance)
7. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Tools

- **AWS CLI** - [Install AWS CLI](https://aws.amazon.com/cli/)
- **SSH Client** - For connecting to Lightsail instance
- **Docker** - Will be installed on Lightsail instance
- **Docker Compose** - Will be installed on Lightsail instance

### AWS Account Setup

1. Create an AWS account if you don't have one
2. Configure AWS CLI with your credentials:
   ```bash
   aws configure
   ```
3. Ensure you have permissions to create and manage Lightsail instances

### Local Setup

1. Clone the repository
2. Ensure you have access to the codebase
3. Prepare environment variables (see Configuration section)

---

## Lightsail Instance Setup

### Step 1: Create Lightsail Instance

1. **Via AWS Console:**
   - Go to [AWS Lightsail Console](https://lightsail.aws.amazon.com/)
   - Click "Create instance"
   - Choose:
     - **Platform**: Linux/Unix
     - **Blueprint**: Ubuntu 22.04 LTS
     - **Instance plan**: At least 2GB RAM, 1 vCPU (recommended: 4GB RAM, 2 vCPU for production)
     - **Instance name**: `beemaedge-production`
   - Click "Create instance"

2. **Via AWS CLI:**
   ```bash
   aws lightsail create-instances \
     --instance-names beemaedge-production \
     --availability-zone us-east-1a \
     --blueprint-id ubuntu_22_04 \
     --bundle-id medium_2_0
   ```

### Step 2: Configure Networking

1. **Open Required Ports:**
   - Go to Lightsail → Networking
   - Add firewall rules:
     - **Port 22** (SSH) - Your IP only
     - **Port 80** (HTTP) - 0.0.0.0/0
     - **Port 443** (HTTPS) - 0.0.0.0/0
     - **Port 8080** (API) - 0.0.0.0/0 (or restrict to your IP)
     - **Port 5341** (Seq UI) - Your IP only (or restrict access)

2. **Static IP (Recommended):**
   - Go to Lightsail → Networking → Create static IP
   - Attach it to your instance

### Step 3: Configure SSH Access

1. **Download SSH Key:**
   - Go to Lightsail → Account → SSH keys
   - Download your default key or create a new one

2. **Connect to Instance:**
   ```bash
   ssh -i ~/.ssh/lightsail-key.pem ubuntu@<instance-ip>
   ```

---

## Configuration

### Step 1: Generate Secrets

1. **Generate JWT Key:**
   ```bash
   openssl rand -hex 64
   ```

2. **Generate Database Passwords:**
   ```bash
   openssl rand -base64 32
   ```

3. **Generate Seq API Key:**
   - This will be generated after Seq is running (see below)

### Step 2: Create Environment File

1. Copy the template:
   ```bash
   cp env.production.template .env.production
   ```

2. Edit `.env.production` and update all values:
   - Database passwords
   - JWT key
   - CORS URLs
   - Frontend URLs
   - Other service configurations

3. **Important:** Never commit `.env.production` to version control!

### Step 3: Update appsettings.Production.json

Create or update `src/Web/appsettings.Production.json`:

```json
{
  "AllowedHosts": "*",
  "AppSettings": {
    "EnableSwagger": false
  },
  "Tenancy": {
    "Strategy": "SharedTable",
    "SchemaPrefix": "tenant_"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "SeqOptions": {
    "SeqUrl": "http://seq:5341",
    "EnableSerilog": true,
    "EnableSeq": true
  },
  "Jwt": {
    "Issuer": "BeemaEdgeApi",
    "Audience": "BeemaEdgeClient"
  },
  "RateLimitOptions": {
    "EnableRateLimiting": true,
    "PermitLimitInMinutes": 300,
    "PermitLimitInHours": 8000,
    "WindowInMinutes": 1,
    "WindowInHours": 1,
    "RejectionStatusCode": 429
  },
  "CacheOptions": {
    "IsEnabled": true,
    "CacheType": "InMemory",
    "AbsoluteExpirationInHours": 1,
    "SlidingExpirationInSeconds": 1
  }
}
```

---

## Deployment

### Option 1: Automated Deployment Script

1. **Make script executable:**
   ```bash
   chmod +x deploy/lightsail-deploy.sh
   ```

2. **Set environment variables:**
   ```bash
   export LIGHTSAIL_INSTANCE_NAME=beemaedge-production
   export SSH_USER=ubuntu
   ```

3. **Run deployment:**
   ```bash
   ./deploy/lightsail-deploy.sh
   ```

### Option 2: Manual Deployment

1. **Connect to Lightsail instance:**
   ```bash
   ssh -i ~/.ssh/lightsail-key.pem ubuntu@<instance-ip>
   ```

2. **Install Docker:**
   ```bash
   curl -fsSL https://get.docker.com -o get-docker.sh
   sudo sh get-docker.sh
   sudo usermod -aG docker ubuntu
   newgrp docker
   ```

3. **Install Docker Compose:**
   ```bash
   sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
   sudo chmod +x /usr/local/bin/docker-compose
   ```

4. **Create deployment directory:**
   ```bash
   sudo mkdir -p /opt/beemaedge
   sudo chown -R ubuntu:ubuntu /opt/beemaedge
   cd /opt/beemaedge
   ```

5. **Copy files to server:**
   - From your local machine:
   ```bash
   scp -i ~/.ssh/lightsail-key.pem docker-compose.production.yml ubuntu@<instance-ip>:/opt/beemaedge/
   scp -i ~/.ssh/lightsail-key.pem .env.production ubuntu@<instance-ip>:/opt/beemaedge/
   scp -i ~/.ssh/lightsail-key.pem -r src ubuntu@<instance-ip>:/opt/beemaedge/
   scp -i ~/.ssh/lightsail-key.pem BeemaEdge.sln ubuntu@<instance-ip>:/opt/beemaedge/
   ```

6. **On the server, start services:**
   ```bash
   cd /opt/beemaedge
   docker-compose -f docker-compose.production.yml up -d --build
   ```

---

## Database Setup

### Step 1: Run Migrations

After the containers are running, execute migrations:

**Option A: From local machine (via SSH):**
```bash
ssh -i ~/.ssh/lightsail-key.pem ubuntu@<instance-ip> "cd /opt/beemaedge && docker-compose exec beemaedge-api dotnet ef database update --context ApplicationDataContext"
ssh -i ~/.ssh/lightsail-key.pem ubuntu@<instance-ip> "cd /opt/beemaedge && docker-compose exec beemaedge-api dotnet ef database update --context AuditDataContext"
ssh -i ~/.ssh/lightsail-key.pem ubuntu@<instance-ip> "cd /opt/beemaedge && docker-compose exec beemaedge-api dotnet ef database update --context HangfireDataContext"
```

**Option B: Using migration script:**
```bash
chmod +x deploy/database-migrate.sh
./deploy/database-migrate.sh
```

### Step 2: Verify Databases

Connect to PostgreSQL containers and verify:

```bash
# Application Database
docker-compose exec postgres psql -U beemaedge -d beemaedge_db -c "\dt"

# Audit Database
docker-compose exec postgres-audit psql -U beemaedge_audit -d beemaedge_audit_db -c "\dt"

# Hangfire Database
docker-compose exec postgres-hangfire psql -U beemaedge_hangfire -d beemaedge_hangfire_db -c "\dt"
```

---

## Monitoring and Maintenance

### Health Checks

1. **API Health:**
   ```bash
   curl http://<instance-ip>:8080/health
   ```

2. **Container Status:**
   ```bash
   docker-compose -f docker-compose.production.yml ps
   ```

3. **View Logs:**
   ```bash
   # All services
   docker-compose -f docker-compose.production.yml logs -f
   
   # Specific service
   docker-compose -f docker-compose.production.yml logs -f beemaedge-api
   ```

### Seq Logging

1. **Access Seq UI:**
   - Navigate to: `http://<instance-ip>:5341`
   - Login with credentials from `.env.production`

2. **Generate API Key:**
   - Go to Settings → API Keys
   - Create a new API key
   - Update `SEQ_API_KEY` in `.env.production`
   - Restart the API container:
     ```bash
     docker-compose -f docker-compose.production.yml restart beemaedge-api
     ```

### Database Backups

1. **Manual Backup:**
   ```bash
   chmod +x deploy/backup-databases.sh
   ./deploy/backup-databases.sh
   ```

2. **Automated Backups (Cron):**
   ```bash
   # Add to crontab
   crontab -e
   
   # Daily backup at 2 AM
   0 2 * * * /opt/beemaedge/deploy/backup-databases.sh
   ```

### Updates and Redeployment

1. **Pull latest code:**
   ```bash
   git pull origin main
   ```

2. **Rebuild and restart:**
   ```bash
   docker-compose -f docker-compose.production.yml up -d --build
   ```

3. **Run migrations if needed:**
   ```bash
   ./deploy/database-migrate.sh
   ```

---

## Troubleshooting

### Common Issues

#### 1. Containers Not Starting

**Check logs:**
```bash
docker-compose -f docker-compose.production.yml logs
```

**Common causes:**
- Environment variables not set correctly
- Port conflicts
- Insufficient memory

#### 2. Database Connection Errors

**Verify:**
- Database containers are running: `docker-compose ps`
- Connection strings in `.env.production` are correct
- Network connectivity between containers

**Test connection:**
```bash
docker-compose exec beemaedge-api ping postgres
```

#### 3. Migration Failures

**Check:**
- Database exists and is accessible
- User has proper permissions
- Previous migrations are in sync

**Reset (CAUTION - Data Loss):**
```bash
docker-compose exec beemaedge-api dotnet ef database drop --context ApplicationDataContext --force
docker-compose exec beemaedge-api dotnet ef database update --context ApplicationDataContext
```

#### 4. Out of Memory

**Check memory usage:**
```bash
docker stats
```

**Solutions:**
- Upgrade Lightsail instance plan
- Optimize PostgreSQL settings
- Reduce container resource limits

#### 5. Seq Not Receiving Logs

**Verify:**
- Seq container is running
- API key is correct
- Network connectivity: `docker-compose exec beemaedge-api ping seq`

**Check Seq logs:**
```bash
docker-compose logs seq
```

### Performance Tuning

1. **PostgreSQL Optimization:**
   - Adjust `shared_buffers`, `max_connections` in docker-compose
   - Monitor query performance
   - Add indexes as needed

2. **Application Optimization:**
   - Enable response compression
   - Configure caching
   - Optimize database queries

3. **Resource Limits:**
   - Set memory limits for containers
   - Monitor CPU usage
   - Scale horizontally if needed

---

## Security Best Practices

1. **Firewall Rules:**
   - Restrict SSH (port 22) to your IP only
   - Restrict Seq UI (port 5341) to admin IPs
   - Use HTTPS for API (port 443) with reverse proxy

2. **Secrets Management:**
   - Never commit `.env.production` to git
   - Rotate passwords regularly
   - Use AWS Secrets Manager for production

3. **Database Security:**
   - Use strong passwords
   - Limit database user permissions
   - Enable SSL connections

4. **Container Security:**
   - Keep images updated
   - Run containers as non-root user
   - Scan images for vulnerabilities

---

## Next Steps

1. **Set up Reverse Proxy (Nginx):**
   - Configure Nginx for HTTPS
   - Set up SSL certificates (Let's Encrypt)
   - Route traffic to port 8080

2. **Set up Domain:**
   - Point DNS to Lightsail static IP
   - Configure subdomains for API, Seq, etc.

3. **Monitoring:**
   - Set up CloudWatch alarms
   - Configure log aggregation
   - Set up alerting

4. **Backup Strategy:**
   - Automate database backups
   - Store backups in S3
   - Test restore procedures

---

## Support

For issues or questions:
- Check logs: `docker-compose logs`
- Review Seq for application logs
- Check AWS Lightsail console for instance status
- Review this documentation

---

**Last Updated:** 2024-01-15  
**Version:** 1.0



