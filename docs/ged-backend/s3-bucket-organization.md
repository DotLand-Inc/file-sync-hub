# S3 Bucket Organization Strategy

This document describes the file organization structure for the GED system in AWS S3.

## Overview

The organization strategy is designed to:
- Support multi-tenant (multi-organization) architecture
- Enable efficient file retrieval and listing
- Facilitate access control at different levels
- Support document versioning per category/organization
- Enable easy archival and lifecycle management

## Bucket Structure

```
ged-documents-{environment}/
├── organizations/
│   ├── {organization_id}/
│   │   └── {year}/
│   │       ├── invoices/
│   │       │   └── {doc_id}_v{version}_{filename}
│   │       ├── contracts/
│   │       │   └── {doc_id}_v{version}_{filename}
│   │       ├── reports/
│   │       ├── correspondence/
│   │       ├── legal/
│   │       ├── hr/
│   │       ├── financial/
│   │       ├── technical/
│   │       └── other/
│   └── {another_org_id}/
│       └── ...
└── system/
    └── (system files)
```

## Key Format

```
organizations/{organization_id}/{year}/{category}/{document_id}_v{version}_{sanitized_filename}
```

### Components

| Component | Description | Example |
|-----------|-------------|---------|
| `organizations` | Root prefix for all org data | `organizations` |
| `organization_id` | Unique organization identifier | `org-abc123` |
| `year` | 4-digit year | `2025` |
| `category` | Document category folder | `contracts` |
| `document_id` | UUID for the document | `550e8400-e29b-41d4-a716-446655440000` |
| `version` | Version number | `v1`, `v2`, `v3` |
| `sanitized_filename` | Safe original filename | `annual_contract.pdf` |

### Example Keys

```
organizations/org-abc123/2025/contracts/550e8400_v1_annual_contract.pdf
organizations/org-abc123/2025/contracts/550e8400_v2_annual_contract.pdf
organizations/org-abc123/2025/invoices/660f9500_v1_invoice_001.pdf
organizations/org-xyz789/2025/hr/770a0600_v1_employee_handbook.docx
```

## Categories

| Category | Folder Name | Description |
|----------|-------------|-------------|
| Invoices | `invoices` | Invoices, bills, receipts |
| Contracts | `contracts` | Legal contracts, agreements |
| Reports | `reports` | Business reports, analytics |
| Correspondence | `correspondence` | Emails, letters, communications |
| Legal | `legal` | Legal documents, compliance |
| HR | `hr` | Human resources documents |
| Financial | `financial` | Financial statements, budgets |
| Technical | `technical` | Technical documentation, specs |
| Other | `other` | Uncategorized documents |

## Versioning

### Configuration

Versioning is configurable per category and per organization:

```json
{
  "S3": {
    "DefaultVersioning": {
      "Enabled": false,
      "CategoryDefaults": [
        { "Category": "Contracts", "VersioningEnabled": true, "MaxVersions": 10 },
        { "Category": "Legal", "VersioningEnabled": true, "MaxVersions": 0 },
        { "Category": "Invoices", "VersioningEnabled": false, "MaxVersions": 0 }
      ]
    },
    "OrganizationVersioning": [
      {
        "OrganizationId": "org-special",
        "DefaultVersioningEnabled": true,
        "CategoryConfigs": [
          { "Category": "Reports", "VersioningEnabled": true, "MaxVersions": 5 }
        ]
      }
    ]
  }
}
```

### Versioning Rules

| Setting | Description |
|---------|-------------|
| `VersioningEnabled` | Enable/disable versioning for category |
| `MaxVersions` | Maximum versions to keep (0 = unlimited) |

### Version Naming

When versioning is enabled:
```
{doc_id}_v1_{filename}  # First version
{doc_id}_v2_{filename}  # Second version
{doc_id}_v3_{filename}  # Third version
```

### Auto-Cleanup

When `MaxVersions` is set, old versions are automatically deleted when a new version is uploaded:
- If `MaxVersions = 5`, only the 5 most recent versions are kept
- If `MaxVersions = 0`, all versions are kept indefinitely

## Benefits of This Structure

### 1. Multi-Tenancy
- `organizations/` prefix clearly separates org data from system data
- Organization ID enables easy data isolation
- Simple IAM policy definitions per organization

### 2. Access Control
```json
{
  "Effect": "Allow",
  "Action": ["s3:GetObject", "s3:PutObject"],
  "Resource": "arn:aws:s3:::ged-documents-prod/organizations/org-abc123/*"
}
```

### 3. Yearly Organization
- Year as a prefix helps with lifecycle management
- Easy to archive or delete by year
- Prevents accumulation of files in single folder

### 4. Listing Efficiency
- List all org documents: `prefix=organizations/org-123/`
- List by year: `prefix=organizations/org-123/2025/`
- List by category: `prefix=organizations/org-123/2025/contracts/`

## Metadata Storage

Each file includes S3 metadata:

| Key | Description |
|-----|-------------|
| `original-filename` | Original uploaded filename |
| `organization-id` | Organization identifier |
| `category` | Document category |
| `document-id` | Document UUID |
| `version` | Version number |
| `checksum` | MD5 hash for integrity |

## API Endpoints

| Endpoint | Description |
|----------|-------------|
| `POST /upload` | Upload new document |
| `POST /upload/version` | Upload new version of existing document |
| `GET /{org}/versions/{docId}` | Get all versions of a document |
| `GET /{org}/versioning/{category}` | Check if versioning is enabled |

## Lifecycle Rules

| Rule | Prefix | Transition | Storage Class |
|------|--------|------------|---------------|
| Archive after 1 year | `organizations/*/2023/` | 365 days | GLACIER |
| Archive after 2 years | `organizations/*/2022/` | Immediate | DEEP_ARCHIVE |

## Security Best Practices

1. **Enable S3 versioning** at bucket level for additional protection
2. **Enable encryption** (SSE-S3 or SSE-KMS)
3. **Block public access** on the bucket
4. **Enable access logging** for audit trails
5. **Use presigned URLs** for client-side uploads/downloads
6. **Implement IAM policies** per organization prefix
