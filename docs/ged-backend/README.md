# GED Backend Documentation

Document Management System (GED) Backend with AWS S3 Storage.

## Overview

The GED Backend provides a RESTful API for managing documents stored in AWS S3. It supports multi-organization architecture with structured file organization.

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                   ASP.NET Core Web API                   │
├─────────────────────────────────────────────────────────┤
│  Controllers/                                            │
│  └── DocumentsController.cs  # Document endpoints        │
├─────────────────────────────────────────────────────────┤
│  Services/                                               │
│  ├── IS3StorageService.cs    # Interface                 │
│  └── S3StorageService.cs     # S3 operations             │
├─────────────────────────────────────────────────────────┤
│  Models/                                                 │
│  ├── DocumentCategory.cs     # Category enum             │
│  ├── UploadResult.cs         # Upload result model       │
│  └── S3FileInfo.cs           # File info model           │
├─────────────────────────────────────────────────────────┤
│  Configuration/                                          │
│  └── S3Settings.cs           # S3 configuration          │
├─────────────────────────────────────────────────────────┤
│  Infrastructure                                          │
│  └── AWS S3 Bucket                                       │
└─────────────────────────────────────────────────────────┘
```

## Features

- File upload to AWS S3
- File download with presigned URLs
- Direct file download
- File deletion
- File listing by organization/category
- Multi-organization support
- Document categorization
- File validation (size, extension)
- Metadata storage

## Quick Start

```bash
# Navigate to the backend directory
cd ged-backend

# Restore dependencies
dotnet restore

# Run the development server
dotnet run

# Or with hot reload
dotnet watch run
```

## API Documentation

Once running, access the OpenAPI documentation at:
- OpenAPI: http://localhost:5000/openapi/v1.json

## Configuration

Configuration can be done via `appsettings.json` or **environment variables**.

### Using appsettings.json

```json
{
  "S3": {
    "Region": "eu-west-1",
    "BucketName": "your-bucket-name",
    "ServiceUrl": null,
    "MaxFileSizeMb": 100,
    "AllowedExtensions": [".pdf", ".doc", ".docx", ...]
  }
}
```

### Using Environment Variables

Environment variables **override** appsettings.json values.

```bash
# AWS Credentials (read automatically by AWS SDK)
export AWS_ACCESS_KEY_ID=your_access_key
export AWS_SECRET_ACCESS_KEY=your_secret_key

# S3 Configuration (use double underscore __ for nested config)
export S3__Region=eu-west-1
export S3__BucketName=your-ged-bucket
export S3__ServiceUrl=http://localhost:4566  # For LocalStack
export S3__MaxFileSizeMb=100
```

### Settings Reference

| Setting | Env Variable | Description | Default |
|---------|--------------|-------------|---------|
| `S3:Region` | `S3__Region` | AWS region | `eu-west-1` |
| `S3:BucketName` | `S3__BucketName` | S3 bucket name | - |
| `S3:ServiceUrl` | `S3__ServiceUrl` | Custom S3 endpoint | `null` |
| `S3:MaxFileSizeMb` | `S3__MaxFileSizeMb` | Max file size in MB | `100` |
| - | `AWS_ACCESS_KEY_ID` | AWS access key | - |
| - | `AWS_SECRET_ACCESS_KEY` | AWS secret key | - |

## Documentation Index

- [S3 Storage Service](s3-storage-service.md) - Core storage service documentation
- [S3 Bucket Organization](s3-bucket-organization.md) - File organization strategy
- [API Reference](api-reference.md) - API endpoints documentation

## Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Local Development with LocalStack

For local development without AWS costs:

```bash
# Start LocalStack
docker run -d -p 4566:4566 localstack/localstack

# Configure appsettings.Development.json
# Set S3:ServiceUrl to "http://localhost:4566"

# Create bucket
aws --endpoint-url=http://localhost:4566 s3 mb s3://ged-documents-dev
```

## AWS Credentials

For production, configure AWS credentials using:
- Environment variables (`AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`)
- AWS credentials file (`~/.aws/credentials`)
- IAM roles (recommended for EC2/ECS)
