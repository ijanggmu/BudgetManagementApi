# BeemaEdge Amazon Lightsail Deployment - Summary

## 📦 What Has Been Created

### Core Deployment Files

1. **`docker-compose.production.yml`**
   - Production-ready Docker Compose configuration
   - Includes PostgreSQL (3 databases), Seq, and BeemaEdge API
   - Health checks, restart policies, and resource optimization
   - Network isolation and volume management

2. **`src/Web/Dockerfile`** (Updated)
   - Fixed project name from `CustomerPortalApi` to `BeemaEdgeApi`
   - Multi-stage build for optimized image size
   - Font installation for QuestPDF
   - Non-root user for security

3. **`env.production.template`**
   - Template for all environment variables
   - Database, JWT, Seq, CORS, and other configurations
   - Copy to `.env.production` and configure

### Deployment Scripts

All scripts are in `deploy/` directory:

1. **`lightsail-deploy.sh`**
   - Automated deployment to Lightsail
   - Installs Docker and Docker Compose
   - Transfers files and starts services
   - Health checks and status reporting

2. **`database-migrate.sh`**
   - Runs EF Core migrations on all 3 databases
   - Application, Audit, and Hangfire databases
   - Can run locally or in Docker

3. **`backup-databases.sh`**
   - Creates compressed backups of all databases
   - Timestamped backups with retention policy
   - Supports local and Docker environments

4. **`restore-databases.sh`**
   - Restores databases from backup files
   - Safety confirmation prompts
   - Restores all 3 databases

5. **`health-check.sh`**
   - Comprehensive health checks
   - Container status, API health, database connectivity
   - Disk and memory monitoring
   - Exit codes for automation

### Documentation

1. **`DEPLOYMENT_GUIDE.md`**
   - Complete step-by-step deployment guide
   - Prerequisites, configuration, troubleshooting
   - Security best practices
   - Maintenance procedures

2. **`QUICK_START.md`**
   - Quick reference for experienced users
   - 5-step deployment process
   - Common commands and troubleshooting

3. **`.dockerignore`**
   - Optimizes Docker build context
   - Excludes unnecessary files
   - Reduces build time and image size

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────┐
│         Amazon Lightsail Instance               │
│                                                 │
│  ┌──────────────────────────────────────────┐  │
│  │      Docker Network (beemaedge-network) │  │
│  │                                          │  │
│  │  ┌──────────────┐  ┌──────────────┐    │  │
│  │  │  PostgreSQL   │  │  PostgreSQL  │    │  │
│  │  │   (Main DB)   │  │  (Audit DB) │    │  │
│  │  └──────────────┘  └──────────────┘    │  │
│  │                                          │  │
│  │  ┌──────────────┐  ┌──────────────┐    │  │
│  │  │  PostgreSQL  │  │     Seq      │    │  │
│  │  │  (Hangfire)  │  │   (Logging) │    │  │
│  │  └──────────────┘  └──────────────┘    │  │
│  │                                          │  │
│  │  ┌──────────────────────────────────┐   │  │
│  │  │    BeemaEdge API (Port 8080)    │   │  │
│  │  └──────────────────────────────────┘   │  │
│  │                                          │  │
│  └──────────────────────────────────────────┘  │
│                                                 │
└─────────────────────────────────────────────────┘
```

## 🚀 Quick Start

1. **Prepare Environment:**
   ```bash
   cp env.production.template .env.production
   # Edit .env.production with your values
   ```

2. **Create Lightsail Instance:**
   - Minimum: 2GB RAM, 1 vCPU
   - Recommended: 4GB RAM, 2 vCPU

3. **Deploy:**
   ```bash
   chmod +x deploy/lightsail-deploy.sh
   export LIGHTSAIL_INSTANCE_NAME=beemaedge-production
   ./deploy/lightsail-deploy.sh
   ```

4. **Run Migrations:**
   ```bash
   chmod +x deploy/database-migrate.sh
   ./deploy/database-migrate.sh
   ```

5. **Verify:**
   ```bash
   curl http://<instance-ip>:8080/health
   ```

## 📋 Services & Ports

| Service | Port | Description |
|---------|------|-------------|
| BeemaEdge API | 8080 | Main API endpoint |
| PostgreSQL (Main) | 5432 | Application database |
| PostgreSQL (Audit) | 5433 | Audit database |
| PostgreSQL (Hangfire) | 5434 | Hangfire database |
| Seq UI | 5341 | Logging interface |
| Seq API | 5342 | Log ingestion |

## 🔐 Security Checklist

- [ ] Strong passwords for all databases
- [ ] JWT key generated (64-byte hex)
- [ ] Seq admin password set
- [ ] Hangfire password set
- [ ] Firewall rules configured
- [ ] SSH access restricted to your IP
- [ ] Seq UI access restricted
- [ ] `.env.production` not committed to git
- [ ] SSL/HTTPS configured (recommended)

## 📊 Monitoring

### Health Checks

```bash
# Run health check script
./deploy/health-check.sh

# Check API health
curl http://<instance-ip>:8080/health

# View container status
docker-compose -f docker-compose.production.yml ps
```

### Logs

```bash
# All services
docker-compose -f docker-compose.production.yml logs -f

# Specific service
docker-compose -f docker-compose.production.yml logs -f beemaedge-api

# Seq UI
http://<instance-ip>:5341
```

## 💾 Backup Strategy

### Manual Backup
```bash
./deploy/backup-databases.sh
```

### Automated Backup (Cron)
```bash
# Add to crontab
0 2 * * * /opt/beemaedge/deploy/backup-databases.sh
```

### Restore
```bash
./deploy/restore-databases.sh <timestamp>
# Example: ./deploy/restore-databases.sh 20240115_120000
```

## 🔄 Updates & Maintenance

### Update Application

```bash
# Pull latest code
git pull

# Rebuild and restart
docker-compose -f docker-compose.production.yml up -d --build

# Run migrations if needed
./deploy/database-migrate.sh
```

### Restart Services

```bash
# Restart all
docker-compose -f docker-compose.production.yml restart

# Restart specific service
docker-compose -f docker-compose.production.yml restart beemaedge-api
```

## 🐛 Troubleshooting

### Common Issues

1. **Containers not starting**
   - Check logs: `docker-compose logs`
   - Verify environment variables
   - Check port conflicts

2. **Database connection errors**
   - Verify containers are running
   - Check connection strings in `.env.production`
   - Test network: `docker network ls`

3. **Out of memory**
   - Check usage: `docker stats`
   - Upgrade Lightsail instance
   - Optimize PostgreSQL settings

4. **Migration failures**
   - Check database exists
   - Verify user permissions
   - Review migration history

## 📚 Documentation Files

- **`DEPLOYMENT_GUIDE.md`** - Complete deployment guide
- **`QUICK_START.md`** - Quick reference guide
- **`API_DOCUMENTATION.md`** - API endpoint documentation
- **`README.md`** - Project overview

## 🔗 Useful Links

- [AWS Lightsail Console](https://lightsail.aws.amazon.com/)
- [Docker Documentation](https://docs.docker.com/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Seq Documentation](https://docs.datalust.co/docs)

## 📝 Notes

- Scripts need to be made executable on Linux: `chmod +x deploy/*.sh`
- `.env.production` should never be committed to version control
- Regular backups are recommended (daily for production)
- Monitor disk space and memory usage
- Keep Docker images updated for security

## ✅ Pre-Deployment Checklist

- [ ] Lightsail instance created
- [ ] Firewall rules configured
- [ ] SSH access tested
- [ ] `.env.production` configured
- [ ] All passwords generated
- [ ] JWT key generated
- [ ] CORS URLs configured
- [ ] Frontend URLs configured
- [ ] Backup strategy planned
- [ ] Monitoring setup planned

---

**Ready to deploy?** Start with [QUICK_START.md](./QUICK_START.md) or [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md)

**Last Updated:** 2024-01-15  
**Version:** 1.0


