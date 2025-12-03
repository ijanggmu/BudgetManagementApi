# GitHub Actions Workflows

This directory contains CI/CD workflows for the BeemaEdge project.

## Workflows Overview

### 1. `ci-cd.yml` - Main CI/CD Pipeline
**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop`
- Manual workflow dispatch

**Jobs:**
- **Build and Test**: Compiles the solution and runs unit tests
- **Docker Build**: Builds and pushes Docker images to registry
- **Security Scan**: Runs Trivy vulnerability scanner
- **Deploy Staging**: Deploys to staging environment (develop branch)
- **Deploy Production**: Deploys to production environment (main branch)
- **Notify**: Sends deployment status notifications

### 2. `docker-build.yml` - Docker Image Build
**Triggers:**
- Push to `main` or `develop` branches
- Push tags (v*)
- Pull requests
- Manual workflow dispatch

**Purpose:**
- Builds Docker images for the BeemaEdge API
- Pushes to GitHub Container Registry (ghcr.io)
- Supports multi-platform builds (amd64, arm64)
- Uses build cache for faster builds

### 3. `test.yml` - Test Suite
**Triggers:**
- Push to `main` or `develop` branches
- Pull requests
- Manual workflow dispatch

**Jobs:**
- **Unit Tests**: Runs unit tests and generates coverage reports
- **Integration Tests**: Runs integration tests with PostgreSQL service
- **Code Quality**: Runs SonarCloud analysis

### 4. `deploy-lightsail.yml` - Lightsail Deployment
**Triggers:**
- Manual workflow dispatch
- Push to `main` or `develop` branches
- Tag pushes

**Purpose:**
- Deploys application to Amazon Lightsail
- Supports staging and production environments
- Optional database migrations
- Health checks after deployment

### 5. `backup-databases.yml` - Database Backup
**Triggers:**
- Scheduled (daily at 2 AM UTC)
- Manual workflow dispatch

**Purpose:**
- Creates backups of all PostgreSQL databases
- Uploads backups to S3
- Cleans up old local backups

## Required GitHub Secrets

### AWS Credentials
- `AWS_ACCESS_KEY_ID` - AWS access key
- `AWS_SECRET_ACCESS_KEY` - AWS secret key
- `AWS_REGION` - AWS region (e.g., us-east-1)

### Lightsail Configuration
- `LIGHTSAIL_INSTANCE_IP` - Lightsail instance IP address
- `LIGHTSAIL_SSH_PRIVATE_KEY` - SSH private key for Lightsail instance
- `LIGHTSAIL_STAGING_INSTANCE` - Staging instance name
- `LIGHTSAIL_PRODUCTION_INSTANCE` - Production instance name

### Database Secrets
- `POSTGRES_USER` - Main database user
- `POSTGRES_PASSWORD` - Main database password
- `POSTGRES_DB` - Main database name
- `POSTGRES_AUDIT_USER` - Audit database user
- `POSTGRES_AUDIT_PASSWORD` - Audit database password
- `POSTGRES_AUDIT_DB` - Audit database name
- `POSTGRES_HANGFIRE_USER` - Hangfire database user
- `POSTGRES_HANGFIRE_PASSWORD` - Hangfire database password
- `POSTGRES_HANGFIRE_DB` - Hangfire database name

### Application Secrets
- `JWT_KEY` - JWT signing key (64-byte hex)
- `JWT_ISSUER` - JWT issuer
- `JWT_AUDIENCE` - JWT audience
- `HANGFIRE_USERNAME` - Hangfire dashboard username
- `HANGFIRE_PASSWORD` - Hangfire dashboard password
- `SEQ_ADMIN_USERNAME` - Seq admin username
- `SEQ_ADMIN_PASSWORD` - Seq admin password
- `SEQ_API_KEY` - Seq API key

### Frontend URLs
- `CORS_URL_1` - CORS allowed URL 1
- `CORS_URL_2` - CORS allowed URL 2
- `FRONTEND_URL_CUSTOMER_PORTAL` - Customer portal URL
- `FRONTEND_URL_AGENT_PORTAL` - Agent portal URL

### Optional Secrets
- `DOCKER_HUB_USERNAME` - Docker Hub username (if using Docker Hub)
- `DOCKER_HUB_PASSWORD` - Docker Hub password
- `DOCKER_REGISTRY` - Docker registry URL
- `SONAR_TOKEN` - SonarCloud token
- `SLACK_WEBHOOK_URL` - Slack webhook for notifications
- `S3_BACKUP_BUCKET` - S3 bucket for database backups
- `STAGING_URL` - Staging environment URL
- `PRODUCTION_URL` - Production environment URL

## Setting Up GitHub Secrets

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add each secret with its corresponding value

## Environment Protection Rules

For production deployments, set up environment protection rules:

1. Go to **Settings** → **Environments**
2. Create environments: `staging` and `production`
3. For `production`:
   - Enable **Required reviewers** (add team members)
   - Enable **Wait timer** (optional, e.g., 5 minutes)
   - Add **Deployment branches** (only `main`)

## Workflow Usage

### Automatic Deployment

**Staging:**
- Push to `develop` branch → Automatic deployment to staging

**Production:**
- Push to `main` branch → Automatic deployment to production (with approval)

### Manual Deployment

1. Go to **Actions** tab
2. Select **Deploy to Amazon Lightsail**
3. Click **Run workflow**
4. Select environment (staging/production)
5. Choose options (migrations, health check)
6. Click **Run workflow**

### Database Backup

**Automatic:**
- Runs daily at 2 AM UTC

**Manual:**
1. Go to **Actions** tab
2. Select **Database Backup**
3. Click **Run workflow**
4. Select environment
5. Click **Run workflow**

## Workflow Status Badges

Add to your README.md:

```markdown
![CI/CD](https://github.com/your-org/beemaedge/workflows/CI/CD%20Pipeline/badge.svg)
![Tests](https://github.com/your-org/beemaedge/workflows/Test%20Suite/badge.svg)
![Docker Build](https://github.com/your-org/beemaedge/workflows/Docker%20Build%20and%20Push/badge.svg)
```

## Troubleshooting

### Workflow Failures

1. **Check workflow logs:**
   - Go to **Actions** tab
   - Click on failed workflow
   - Review job logs

2. **Common issues:**
   - Missing secrets → Add required secrets
   - SSH connection failed → Verify SSH key and IP
   - Docker build failed → Check Dockerfile
   - Tests failed → Review test output
   - Deployment failed → Check Lightsail instance status

### Debugging

Enable debug logging:
- Add secret: `ACTIONS_STEP_DEBUG` = `true`
- Add secret: `ACTIONS_RUNNER_DEBUG` = `true`

## Best Practices

1. **Never commit secrets** - Always use GitHub Secrets
2. **Use environment protection** - Require approvals for production
3. **Monitor workflows** - Set up notifications for failures
4. **Regular backups** - Ensure backup workflow is running
5. **Test before deploy** - All tests must pass before deployment
6. **Review changes** - Use pull requests for code review

## Workflow Customization

To customize workflows:
1. Edit workflow files in `.github/workflows/`
2. Adjust triggers, jobs, and steps as needed
3. Test changes in a feature branch first
4. Merge to main/develop to activate

---

**Last Updated:** 2024-01-15  
**Version:** 1.0



