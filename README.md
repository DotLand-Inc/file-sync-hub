# FileSyncHub

A comprehensive file synchronization and document management platform.

## Overview

FileSyncHub is a modular platform that provides:
- **GED Backend**: Document Management System with AWS S3 storage
- File synchronization capabilities
- Multi-organization support

## Project Structure

```
file-sync-hub/
├── ged-backend/
│   ├── Dotland.FileSyncHub.sln          # Solution file
│   └── src/
│       └── Dotland.FileSyncHub.Web/     # Web API project
│           ├── Controllers/
│           ├── Services/
│           ├── Models/
│           ├── Configuration/
│           └── Dotland.FileSyncHub.Web.csproj
├── docs/
│   └── ged-backend/
└── README.md
```

## Components

### GED Backend

Document Management System (GED) backend built with .NET 9 and AWS S3.

- **Technology**: .NET 9, ASP.NET Core Web API, AWS SDK
- **Storage**: AWS S3
- **Documentation**: [docs/ged-backend/](docs/ged-backend/README.md)

## Quick Start

### GED Backend

```bash
cd ged-backend

# Restore dependencies
dotnet restore

# Configure S3 settings
# Edit src/Dotland.FileSyncHub.Web/appsettings.Development.json

# Run the server
dotnet run --project src/Dotland.FileSyncHub.Web

# Or with hot reload
dotnet watch --project src/Dotland.FileSyncHub.Web
```

The API will be available at `http://localhost:5000`

### Local Development with LocalStack

```bash
# Start LocalStack for local S3
docker run -d -p 4566:4566 localstack/localstack

# Create bucket
aws --endpoint-url=http://localhost:4566 s3 mb s3://ged-documents-dev
```

## Documentation

Detailed documentation is available in the [docs/](docs/) directory:

- [GED Backend Documentation](docs/ged-backend/README.md)
  - [S3 Storage Service](docs/ged-backend/s3-storage-service.md)
  - [S3 Bucket Organization](docs/ged-backend/s3-bucket-organization.md)
  - [API Reference](docs/ged-backend/api-reference.md)

## License

MIT License
