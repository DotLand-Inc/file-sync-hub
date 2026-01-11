# GitHub Actions Workflows

This directory contains GitHub Actions workflows for CI/CD of the FileSyncHub .NET application.

## Workflows

### ci-build.yml
Continuous Integration workflow that runs on:
- Push to `develop` branch
- Pull requests to any branch
- Can be called by other workflows

**Features:**
- Builds the .NET solution
- Runs all tests
- Builds Docker image
- Optionally pushes to Docker Hub (when called from CD workflow)
- Uses GitHub Actions cache for Docker layers

**Required Secrets:**
- `DOCKER_PAT`: Docker Hub Personal Access Token (only needed when pushing images)

### cd-deploy.yml
Continuous Deployment workflow that runs on:
- Push to `main` branch

**Features:**
- Cleans up existing Docker Hub tags
- Calls CI workflow to build and push with `latest` tag
- Builds and pushes with version tag extracted from .csproj file

**Required Secrets:**
- `DOCKER_PAT`: Docker Hub Personal Access Token

## Environment Variables

The following environment variables are configured in the workflows:

- `DOCKER_USERNAME`: Docker Hub username (default: `hsaii`)
- `WEB_IMAGE`: Docker image name for the web API
- `TARGET_TAG`: Tag to use for deployments (default: `latest`)

## Docker Images

The workflows build and push the following Docker images:

- `hsaii/filesync-web:develop` - Built on every push to develop
- `hsaii/filesync-web:latest` - Built on every push to main
- `hsaii/filesync-web:<version>` - Built on every push to main with version from .csproj

## Setup Instructions

1. **Create Docker Hub Personal Access Token:**
   - Go to https://hub.docker.com/settings/security
   - Click "New Access Token"
   - Give it a name (e.g., "GitHub Actions")
   - Copy the token

2. **Add Secret to GitHub Repository:**
   - Go to your repository Settings
   - Navigate to Secrets and variables > Actions
   - Click "New repository secret"
   - Name: `DOCKER_PAT`
   - Value: Paste your Docker Hub token
   - Click "Add secret"

3. **Update Docker Username:**
   - Edit the workflows and replace `hsaii` with your Docker Hub username
   - Update in both `ci-build.yml` and `cd-deploy.yml`

4. **Customize Image Names:**
   - Edit `WEB_IMAGE` in `ci-build.yml` to match your desired image name
   - Update the repository name in `cd-deploy.yml` cleanup script

## Environment Variables for Docker Container

When running the Docker container, you should provide the following environment variables:

See `.env.docker.example` for a complete list of required environment variables.

**Critical Variables:**
- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string
- `AWS_ACCESS_KEY_ID`: AWS access key for S3
- `AWS_SECRET_ACCESS_KEY`: AWS secret key for S3
- `S3__BucketName`: S3 bucket name
- `S3__Region`: AWS region

## Usage

### Manual Trigger
Workflows run automatically on push/PR, but you can also trigger them manually from the Actions tab.

### Customizing Build
To customize the build process:
1. Edit the `dotnet build` and `dotnet test` steps in `ci-build.yml`
2. Add additional build arguments to the Docker build step
3. Add pre/post build steps as needed

### Version Management
The version is automatically extracted from `src/Dotland.FileSyncHub.Web/Dotland.FileSyncHub.Web.csproj`.

To update the version:
1. Edit the `.csproj` file
2. Update the `<Version>` tag
3. Commit and push to main

The CD workflow will automatically tag the Docker image with this version.
