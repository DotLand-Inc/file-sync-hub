# S3 Storage Service

The `S3StorageService` is the core service for managing file operations with AWS S3.

## Location

`ged-backend/Services/S3StorageService.cs`

## Overview

This service provides a high-level interface for:
- Uploading files to S3
- Downloading files from S3
- Deleting files
- Generating presigned URLs
- Listing files
- File existence checks
- Copying files

## Interface: IS3StorageService

The service implements `IS3StorageService` for dependency injection.

### Dependency Injection

```csharp
// In Program.cs
builder.Services.AddScoped<IS3StorageService, S3StorageService>();

// In a controller or service
public class MyController : ControllerBase
{
    private readonly IS3StorageService _storageService;

    public MyController(IS3StorageService storageService)
    {
        _storageService = storageService;
    }
}
```

## Methods

### UploadFileAsync

Upload a file to S3 with automatic key generation.

```csharp
using GedBackend.Models;
using GedBackend.Services;

var result = await _storageService.UploadFileAsync(
    fileStream: stream,
    filename: "document.pdf",
    organizationId: "org-123",
    category: DocumentCategory.Contracts,
    contentType: "application/pdf",    // optional, auto-detected
    metadata: new Dictionary<string, string> { ["author"] = "John" }
);

Console.WriteLine(result.Success);      // true
Console.WriteLine(result.DocumentId);   // GUID
Console.WriteLine(result.S3Key);        // org-123/contracts/2024/11/guid_document.pdf
```

### DownloadFileAsync

Download a file from S3.

```csharp
var (content, metadata) = await _storageService.DownloadFileAsync(
    s3Key: "org-123/contracts/2024/11/doc.pdf"
);

// content is a Stream
// metadata is Dictionary<string, string>
```

### DeleteFileAsync

Delete a file from S3.

```csharp
bool success = await _storageService.DeleteFileAsync(
    s3Key: "org-123/contracts/2024/11/doc.pdf"
);
```

### GeneratePresignedUrl

Generate a presigned URL for secure file access.

```csharp
// For download (GET)
string downloadUrl = _storageService.GeneratePresignedUrl(
    s3Key: "org-123/contracts/2024/11/doc.pdf",
    expirationMinutes: 60
);

// For upload (PUT)
string uploadUrl = _storageService.GeneratePresignedUrl(
    s3Key: "org-123/contracts/2024/11/new-doc.pdf",
    expirationMinutes: 60,
    forUpload: true
);
```

### FileExistsAsync

Check if a file exists in S3.

```csharp
bool exists = await _storageService.FileExistsAsync(
    s3Key: "org-123/contracts/2024/11/doc.pdf"
);
```

### ListFilesAsync

List files for an organization.

```csharp
// List all files for an organization
var files = await _storageService.ListFilesAsync(organizationId: "org-123");

// List files in a specific category
var invoices = await _storageService.ListFilesAsync(
    organizationId: "org-123",
    category: DocumentCategory.Invoices
);

// Each file is an S3FileInfo with Key, Size, LastModified, ETag
```

### CopyFileAsync

Copy a file within S3.

```csharp
bool success = await _storageService.CopyFileAsync(
    sourceKey: "org-123/contracts/2024/11/doc.pdf",
    destKey: "org-123/archive/doc.pdf"
);
```

## Error Handling

The service throws exceptions on failure:

```csharp
try
{
    var result = await _storageService.UploadFileAsync(...);
}
catch (InvalidOperationException ex)
{
    // File validation failed (extension or size)
    Console.WriteLine($"Validation error: {ex.Message}");
}
catch (FileNotFoundException ex)
{
    // File not found in S3
    Console.WriteLine($"Not found: {ex.Message}");
}
catch (AmazonS3Exception ex)
{
    // AWS S3 error
    Console.WriteLine($"S3 error: {ex.Message}");
}
```

## File Validation

The service validates files before upload:

- **File extension**: Must be in the allowed extensions list
- **File size**: Must not exceed `MaxFileSizeMb`

Configure in `appsettings.json`:

```json
{
  "S3": {
    "MaxFileSizeMb": 100,
    "AllowedExtensions": [
      ".pdf", ".doc", ".docx", ".xls", ".xlsx",
      ".ppt", ".pptx", ".txt", ".csv", ".json",
      ".png", ".jpg", ".jpeg", ".gif", ".bmp"
    ]
  }
}
```

## S3 Key Generation

Keys are automatically generated following the bucket organization structure:

```
{organization_id}/{category}/{year}/{month}/{document_id}_{sanitized_filename}
```

Example: `org-123/contracts/2024/11/abc123_contract.pdf`
