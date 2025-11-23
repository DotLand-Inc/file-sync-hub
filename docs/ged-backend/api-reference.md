# API Reference

GED Backend REST API documentation.

## Base URL

```
http://localhost:5000/api/v1
```

## Endpoints

### Health Check

#### GET /health

Check service health.

**Response:**
```json
{
  "status": "healthy",
  "service": "ged-backend"
}
```

---

### Documents

#### POST /api/v1/documents/upload

Upload a new document to the GED system.

**Request:**
- Content-Type: `multipart/form-data`

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `file` | File | Yes | File to upload |
| `organizationId` | string | Yes | Organization identifier |
| `category` | enum | No | Document category (default: `Other`) |
| `description` | string | No | Document description |

**Categories (DocumentCategory enum):**
- `Invoices`, `Contracts`, `Reports`, `Correspondence`
- `Legal`, `Hr`, `Financial`, `Technical`, `Other`

**Response:** `201 Created`
```json
{
  "success": true,
  "documentId": "550e8400-e29b-41d4-a716-446655440000",
  "s3Key": "organizations/org-123/2025/contracts/550e8400_v1_contract.pdf",
  "filename": "contract.pdf",
  "sizeBytes": 102400,
  "contentType": "application/pdf",
  "version": 1,
  "versioningEnabled": true,
  "s3VersionId": null,
  "errorMessage": null
}
```

**Example:**
```bash
curl -X POST "http://localhost:5000/api/v1/documents/upload" \
  -F "file=@contract.pdf" \
  -F "organizationId=org-123" \
  -F "category=Contracts" \
  -F "description=Annual contract 2025"
```

---

#### POST /api/v1/documents/upload/version

Upload a new version of an existing document (requires versioning enabled for the category).

**Request:**
- Content-Type: `multipart/form-data`

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `file` | File | Yes | File to upload |
| `organizationId` | string | Yes | Organization identifier |
| `documentId` | string | Yes | Existing document ID |
| `category` | enum | Yes | Document category |
| `description` | string | No | Version description |

**Response:** `201 Created`
```json
{
  "success": true,
  "documentId": "550e8400-e29b-41d4-a716-446655440000",
  "s3Key": "organizations/org-123/2025/contracts/550e8400_v2_contract.pdf",
  "filename": "contract.pdf",
  "sizeBytes": 105000,
  "contentType": "application/pdf",
  "version": 2,
  "versioningEnabled": true,
  "s3VersionId": null,
  "errorMessage": null
}
```

**Errors:**
- `400 Bad Request`: Versioning not enabled for this category

**Example:**
```bash
curl -X POST "http://localhost:5000/api/v1/documents/upload/version" \
  -F "file=@contract_v2.pdf" \
  -F "organizationId=org-123" \
  -F "documentId=550e8400-e29b-41d4-a716-446655440000" \
  -F "category=Contracts" \
  -F "description=Updated contract"
```

---

#### GET /api/v1/documents/{organizationId}/versions/{documentId}

Get all versions of a document.

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `organizationId` | string | Organization identifier |
| `documentId` | string | Document identifier |

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `category` | enum | Yes | Document category |

**Response:** `200 OK`
```json
{
  "documentId": "550e8400-e29b-41d4-a716-446655440000",
  "versioningEnabled": true,
  "versions": [
    {
      "version": 2,
      "s3Key": "organizations/org-123/2025/contracts/550e8400_v2_contract.pdf",
      "sizeBytes": 105000,
      "createdAt": "2025-01-15T14:30:00Z",
      "isCurrent": true
    },
    {
      "version": 1,
      "s3Key": "organizations/org-123/2025/contracts/550e8400_v1_contract.pdf",
      "sizeBytes": 102400,
      "createdAt": "2025-01-10T10:00:00Z",
      "isCurrent": false
    }
  ],
  "count": 2
}
```

**Example:**
```bash
curl "http://localhost:5000/api/v1/documents/org-123/versions/550e8400?category=Contracts"
```

---

#### GET /api/v1/documents/{organizationId}/versioning/{category}

Check if versioning is enabled for a category.

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `organizationId` | string | Organization identifier |
| `category` | enum | Document category |

**Response:** `200 OK`
```json
{
  "organizationId": "org-123",
  "category": "Contracts",
  "versioningEnabled": true
}
```

---

#### GET /api/v1/documents/download

Get a presigned download URL for a document.

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `s3Key` | string | Yes | S3 object key |
| `expirationMinutes` | int | No | URL expiration (default: 60) |

**Response:** `200 OK`
```json
{
  "downloadUrl": "https://bucket.s3.amazonaws.com/...",
  "expiresInMinutes": 60
}
```

---

#### GET /api/v1/documents/download/direct

Download a document directly (streams the file content).

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `s3Key` | string | Yes | S3 object key |

**Response:** File content with Content-Disposition header

---

#### DELETE /api/v1/documents

Delete a document from S3.

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `s3Key` | string | Yes | S3 object key |

**Response:** `204 No Content`

---

#### GET /api/v1/documents/{organizationId}/list

List documents for an organization.

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `organizationId` | string | Organization identifier |

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `category` | enum | No | Filter by category |

**Response:** `200 OK`
```json
{
  "files": [
    {
      "key": "organizations/org-123/2025/contracts/550e8400_v1_doc.pdf",
      "size": 102400,
      "lastModified": "2025-01-10T10:00:00Z",
      "eTag": "abc123..."
    }
  ],
  "count": 1
}
```

---

## Error Responses

```json
{
  "error": "Error message description"
}
```

## Versioning Configuration

Versioning can be configured per category and per organization in `appsettings.json`:

```json
{
  "S3": {
    "DefaultVersioning": {
      "Enabled": false,
      "CategoryDefaults": [
        { "Category": "Contracts", "VersioningEnabled": true, "MaxVersions": 10 },
        { "Category": "Legal", "VersioningEnabled": true, "MaxVersions": 0 }
      ]
    }
  }
}
```

## OpenAPI

Access the OpenAPI specification at:
- `http://localhost:5000/openapi/v1.json`
