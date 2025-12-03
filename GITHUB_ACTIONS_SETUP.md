# GitHub Actions CI/CD Setup Guide

This guide explains how to set up and use the GitHub Actions workflows for BeemaEdge.

## 📋 Overview

The repository includes comprehensive CI/CD workflows for:
- ✅ Automated testing
- ✅ Docker image building
- ✅ Security scanning
- ✅ Automated deployment to Lightsail
- ✅ Database backups
- ✅ Code quality checks

## 🚀 Quick Setup

### Step 1: Configure GitHub Secrets

Go to your repository → **Settings** → **Secrets and variables** → **Actions** → **New repository secret**

Add the following secrets:

#### AWS & Lightsail
```
AWS_ACCESS_KEY_ID=your-aws-access-key
AWS_SECRET_ACCESS_KEY=your-aws-secret-key
AWS_REGION=us-east-1
LIGHTSAIL_INSTANCE_IP=your-lightsail-ip
LIGHTSAIL_SSH_PRIVATE_KEY=your-ssh-private-key
LIGHTSAIL_STAGING_INSTANCE=beemaedge-staging
LIGHTSAIL_PRODUCTION_INSTANCE=beemaedge-production
```

#### Database
```
POSTGRES_USER=beemaedge
POSTGRES_PASSWORD=your-strong-password
POSTGRES_DB=beemaedge_db
POSTGRES_AUDIT_USER=beemaedge_audit
POSTGRES_AUDIT_PASSWORD=your-strong-password
POSTGRES_AUDIT_DB=beemaedge_audit_db
POSTGRES_HANGFIRE_USER=beemaedge_hangfire
POSTGRES_HANGFIRE_PASSWORD=your-strong-password
POSTGRES_HANGFIRE_DB=beemaedge_hangfire_db
```

#### Application
```
JWT_KEY=your-64-byte-hex-key
JWT_ISSUER=BeemaEdgeApi
JWT_AUDIENCE=BeemaEdgeClient
HANGFIRE_USERNAME=Hangfire-Admin
HANGFIRE_PASSWORD=your-strong-password
SEQ_ADMIN_USERNAME=admin
SEQ_ADMIN_PASSWORD=your-strong-password
SEQ_API_KEY=your-seq-api-key
```

#### Frontend URLs
```
CORS_URL_1=https://yourdomain.com
CORS_URL_2=https://api.yourdomain.com
FRONTEND_URL_CUSTOMER_PORTAL=https://customer.yourdomain.com
FRONTEND_URL_AGENT_PORTAL=https://agent.yourdomain.com
```

#### Optional
```
S3_BACKUP_BUCKET=your-backup-bucket
STAGING_URL=https://staging.yourdomain.com
PRODUCTION_URL=https://api.yourdomain.com
SLACK_WEBHOOK_URL=your-slack-webhook
SONAR_TOKEN=your-sonar-token
```

### Step 2: Set Up Environments

1. Go to **Settings** → **Environments**
2. Create `staging` environment
3. Create `production` environment with:
   - **Required reviewers**: Add team members
   - **Deployment branches**: Only `main`

### Step 3: Generate SSH Key for Lightsail

```bash
# Generate SSH key pair
ssh-keygen -t rsa -b 4096 -f ~/.ssh/lightsail_deploy -N ""

# Add public key to Lightsail instance
cat ~/.ssh/lightsail_deploy.pub
# Copy and add to Lightsail instance authorized_keys

# Add private key to GitHub Secrets
cat ~/.ssh/lightsail_deploy
# Copy and add as LIGHTSAIL_SSH_PRIVATE_KEY secret
```

### Step 4: Push Code

```bash
git add .
git commit -m "Add GitHub Actions workflows"
git push origin main
```

## 📊 Workflow Details

### Main CI/CD Pipeline (`ci-cd.yml`)

**Triggers:**
- Push to `main` or `develop`
- Pull requests
- Manual dispatch

**Jobs:**
1. **Build and Test** - Compiles and tests
2. **Docker Build** - Builds Docker images
3. **Security Scan** - Vulnerability scanning
4. **Deploy Staging** - Auto-deploys `develop` branch
5. **Deploy Production** - Auto-deploys `main` branch (with approval)

### Docker Build (`docker-build.yml`)

Builds and pushes Docker images to GitHub Container Registry.

**Image location:**
```
ghcr.io/your-org/beemaedge/beemaedge-api:latest
ghcr.io/your-org/beemaedge/beemaedge-api:main
ghcr.io/your-org/beemaedge/beemaedge-api:develop
```

### Test Suite (`test.yml`)

Runs comprehensive tests:
- Unit tests with coverage
- Integration tests
- Code quality (SonarCloud)

### Deploy to Lightsail (`deploy-lightsail.yml`)

Manual deployment workflow with options:
- Environment selection (staging/production)
- Run migrations toggle
- Health check toggle

### Database Backup (`backup-databases.yml`)

- Scheduled: Daily at 2 AM UTC
- Manual: On-demand backups
- Uploads to S3

## 🎯 Usage Examples

### Automatic Deployment

**Staging:**
```bash
git checkout develop
git commit -m "New feature"
git push origin develop
# → Automatically deploys to staging
```

**Production:**
```bash
git checkout main
git merge develop
git push origin main
# → Requires approval, then deploys to production
```

### Manual Deployment

1. Go to **Actions** tab
2. Select **Deploy to Amazon Lightsail**
3. Click **Run workflow**
4. Choose:
   - Branch: `main` or `develop`
   - Environment: `staging` or `production`
   - Run migrations: ✅
   - Health check: ✅
5. Click **Run workflow**

### Manual Backup

1. Go to **Actions** tab
2. Select **Database Backup**
3. Click **Run workflow**
4. Select environment
5. Click **Run workflow**

## 🔍 Monitoring Workflows

### View Workflow Status

1. Go to **Actions** tab
2. Click on workflow run
3. View job details and logs

### Workflow Status Badges

Add to README.md:

```markdown
![CI/CD](https://github.com/your-org/beemaedge/workflows/CI/CD%20Pipeline/badge.svg)
![Tests](https://github.com/your-org/beemaedge/workflows/Test%20Suite/badge.svg)
```

### Notifications

Configure notifications:
- GitHub notifications (default)
- Slack webhook (if `SLACK_WEBHOOK_URL` is set)
- Email notifications (GitHub settings)

## 🐛 Troubleshooting

### Workflow Fails to Start

**Issue:** Workflow doesn't trigger
- **Solution:** Check branch names match workflow triggers

### Deployment Fails

**Issue:** SSH connection failed
- **Solution:** 
  - Verify `LIGHTSAIL_SSH_PRIVATE_KEY` secret
  - Check `LIGHTSAIL_INSTANCE_IP` is correct
  - Ensure Lightsail firewall allows SSH

**Issue:** Docker build fails
- **Solution:**
  - Check Dockerfile syntax
  - Verify all files are in repository
  - Check build logs

**Issue:** Database migration fails
- **Solution:**
  - Verify database credentials
  - Check database containers are running
  - Review migration logs

### Tests Fail

**Issue:** Unit tests fail
- **Solution:**
  - Review test output
  - Fix failing tests
  - Check test dependencies

**Issue:** Integration tests fail
- **Solution:**
  - Verify PostgreSQL service is running
  - Check connection strings
  - Review test setup

## 🔐 Security Best Practices

1. **Never commit secrets** - Always use GitHub Secrets
2. **Rotate secrets regularly** - Update passwords and keys
3. **Use environment protection** - Require approvals for production
4. **Limit access** - Only grant necessary permissions
5. **Monitor workflows** - Review logs for suspicious activity
6. **Use branch protection** - Require PR reviews

## 📈 Optimization Tips

1. **Use build cache** - Docker builds use cache for faster builds
2. **Parallel jobs** - Tests run in parallel when possible
3. **Conditional deployment** - Only deploy on specific branches
4. **Artifact retention** - Keep artifacts for 7 days (configurable)

## 🔄 Updating Workflows

1. Edit workflow files in `.github/workflows/`
2. Test in feature branch
3. Create pull request
4. Review and merge
5. Workflows update automatically

## 📚 Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Build Action](https://github.com/docker/build-push-action)
- [AWS Actions](https://github.com/aws-actions)
- [Workflow Syntax](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions)

## ✅ Checklist

- [ ] All secrets configured
- [ ] Environments created
- [ ] SSH key added to Lightsail
- [ ] Workflows tested in feature branch
- [ ] Branch protection rules set
- [ ] Notifications configured
- [ ] Team members have access
- [ ] Documentation reviewed

---

**Need Help?** Check workflow logs or review [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md)

**Last Updated:** 2024-01-15  
**Version:** 1.0



