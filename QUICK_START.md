# BeemaEdge Quick Start - Amazon Lightsail Deployment

This is a quick reference guide for deploying BeemaEdge to Amazon Lightsail. For detailed instructions, see [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md).

## Prerequisites Checklist

- [ ] AWS account with Lightsail access
- [ ] AWS CLI installed and configured
- [ ] SSH access to Lightsail instance
- [ ] Docker and Docker Compose (will be installed automatically)

## Quick Deployment (5 Steps)

### 1. Create Lightsail Instance

```bash
# Via AWS Console: https://lightsail.aws.amazon.com/
# Or via CLI:
aws lightsail create-instances \
  --instance-names beemaedge-production \
  --blueprint-id ubuntu_22_04 \
  --bundle-id medium_2_0
```

**Recommended Instance Size:**
- Minimum: 2GB RAM, 1 vCPU
- Recommended: 4GB RAM, 2 vCPU

### 2. Configure Environment

```bash
# Copy template
cp env.production.template .env.production

# Edit and set all values
nano .env.production  # or use your preferred editor
```

**Required Values to Set:**
- `POSTGRES_PASSWORD` - Strong password for main database
- `POSTGRES_AUDIT_PASSWORD` - Strong password for audit database
- `POSTGRES_HANGFIRE_PASSWORD` - Strong password for Hangfire database
- `SEQ_ADMIN_PASSWORD` - Password for Seq admin user
- `JWT_KEY` - Generate with: `openssl rand -hex 64`
- `HANGFIRE_PASSWORD` - Password for Hangfire dashboard
- `CORS_URL_1`, `CORS_URL_2` - Your frontend domains
- `FRONTEND_URL_CUSTOMER_PORTAL` - Customer portal URL
- `FRONTEND_URL_AGENT_PORTAL` - Agent portal URL

### 3. Configure Firewall

In Lightsail Console → Networking:
- Port 22 (SSH) - Your IP only
- Port 8080 (API) - 0.0.0.0/0 or restricted
- Port 5341 (Seq) - Your IP only

### 4. Deploy

**Option A: Automated (Recommended)**
```bash
chmod +x deploy/lightsail-deploy.sh
export LIGHTSAIL_INSTANCE_NAME=beemaedge-production
./deploy/lightsail-deploy.sh
```

**Option B: Manual**
```bash
# SSH to instance
ssh -i ~/.ssh/lightsail-key.pem ubuntu@<instance-ip>

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker ubuntu
newgrp docker

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Copy files (from local machine)
scp -i ~/.ssh/lightsail-key.pem docker-compose.production.yml .env.production ubuntu@<instance-ip>:/opt/beemaedge/
scp -i ~/.ssh/lightsail-key.pem -r src BeemaEdge.sln ubuntu@<instance-ip>:/opt/beemaedge/

# On server
cd /opt/beemaedge
docker-compose -f docker-compose.production.yml up -d --build
```

### 5. Run Database Migrations

```bash
# From local machine
chmod +x deploy/database-migrate.sh
./deploy/database-migrate.sh

# Or manually via SSH
ssh -i ~/.ssh/lightsail-key.pem ubuntu@<instance-ip>
cd /opt/beemaedge
docker-compose exec beemaedge-api dotnet ef database update --context ApplicationDataContext
docker-compose exec beemaedge-api dotnet ef database update --context AuditDataContext
docker-compose exec beemaedge-api dotnet ef database update --context HangfireDataContext
```

## Verify Deployment

```bash
# Check API health
curl http://<instance-ip>:8080/health

# Check containers
ssh -i ~/.ssh/lightsail-key.pem ubuntu@<instance-ip>
docker-compose -f docker-compose.production.yml ps

# View logs
docker-compose -f docker-compose.production.yml logs -f beemaedge-api
```

## Access Services

- **API**: `http://<instance-ip>:8080`
- **Seq UI**: `http://<instance-ip>:5341`
- **Hangfire Dashboard**: `http://<instance-ip>:8080/hangfire-admin-dashboard`

## Common Commands

```bash
# View logs
docker-compose -f docker-compose.production.yml logs -f

# Restart services
docker-compose -f docker-compose.production.yml restart

# Stop services
docker-compose -f docker-compose.production.yml down

# Update and redeploy
git pull
docker-compose -f docker-compose.production.yml up -d --build

# Backup databases
./deploy/backup-databases.sh

# Health check
./deploy/health-check.sh
```

## Troubleshooting

**Containers not starting?**
```bash
docker-compose -f docker-compose.production.yml logs
```

**Database connection errors?**
- Check `.env.production` connection strings
- Verify containers are running: `docker-compose ps`
- Check network: `docker network ls`

**Out of memory?**
- Upgrade Lightsail instance
- Check usage: `docker stats`

**Need help?**
- Check [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md) for detailed troubleshooting
- Review logs in Seq UI
- Check container logs: `docker-compose logs`

## Next Steps

1. **Set up Domain & HTTPS**
   - Point DNS to Lightsail static IP
   - Configure Nginx reverse proxy
   - Set up SSL with Let's Encrypt

2. **Configure Monitoring**
   - Set up automated backups
   - Configure health check alerts
   - Set up log aggregation

3. **Security Hardening**
   - Restrict firewall rules
   - Rotate passwords regularly
   - Enable database SSL

---

**For detailed documentation, see [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md)**


