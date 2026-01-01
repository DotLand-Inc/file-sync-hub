using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.Model;
using Dotland.FileSyncHub.Application.Common.Models;
using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Application.Common.Settings;
using Dotland.FileSyncHub.Application.Versioning;
using Dotland.FileSyncHub.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DomainCategory = Dotland.FileSyncHub.Domain.Enums.DocumentCategory;

namespace Dotland.FileSyncHub.Infrastructure.ThirdParty.Storage;

/// <summary>
/// Service for managing file storage in AWS S3.
///
/// Bucket Organization:
/// bucket/
/// ├── organizations/
/// │   ├── {organization_id}/
/// │   │   └── {year}/{category}/{document_id}_{version}_{filename}
/// └── system/
///     └── (system files)
///
/// Versioning Configuration:
/// - First checks database configuration (IVersioningService)
/// - Falls back to appsettings.json configuration if no DB config exists
/// </summary>
public class S3StorageService(
    IAmazonS3 s3Client,
    IOptions<S3Settings> settings,
    IVersioningService versioningService,
    ILogger<S3StorageService> logger)
    : IS3StorageService
{
    private const string OrganizationsPrefix = "organizations";
    private readonly S3Settings _settings = settings.Value;
    
    /// <summary>
    /// Upload a file with optional document ID for versioning.
    /// </summary>
    public async Task<UploadResult> UploadFileAsync(
        UploadS3FileDto uploadS3FileDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Read stream to memory for validation and checksum
            using var memoryStream = new MemoryStream();
            await uploadS3FileDto.FileStream.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();

            // Validate file
            ValidateFile(uploadS3FileDto.Filename, fileBytes.Length);

            // Determine version number
            var version = 1;
            var isNewDocument = string.IsNullOrEmpty(uploadS3FileDto.DocumentId);

            if (isNewDocument)
            {
                uploadS3FileDto.DocumentId = Guid.NewGuid().ToString();
            }
            else if (uploadS3FileDto.VersioningEnabled)
            {
                if (uploadS3FileDto.DocumentId == null)
                {
                    return new UploadResult
                    {
                        Success = false,
                        Filename = uploadS3FileDto.Filename,
                        ErrorMessage = "S3 upload failed: Document id is required when uploading new version."
                    };
                }

                if (string.IsNullOrWhiteSpace(uploadS3FileDto.S3Key))
                {
                    return new UploadResult
                    {
                        Success = false,
                        Filename = uploadS3FileDto.Filename,
                        ErrorMessage = "S3 upload failed: s3Key is required when upload new version."
                    };
                }
                
                // Get current max version
                version = await GetNextVersionAsync(
                    uploadS3FileDto.S3Key,
                    cancellationToken);
            }
            
            if (uploadS3FileDto.DocumentId == null)
            {
                return new UploadResult
                {
                    Success = false,
                    Filename = uploadS3FileDto.Filename,
                    ErrorMessage = "S3 upload failed: Unknown error."
                };
            }

            // Generate S3 key with new structure
            var s3Key = GenerateS3Key(
                uploadS3FileDto.OrganizationId,
                uploadS3FileDto.Category,
                uploadS3FileDto.Filename,
                version);

            // Auto-detect content type if not provided
            uploadS3FileDto.ContentType ??= GetContentType(uploadS3FileDto.Filename);

            // Prepare metadata
            var s3Metadata = new Dictionary<string, string>
            {
                ["original-filename"] = uploadS3FileDto.Filename,
                ["organization-id"] = uploadS3FileDto.OrganizationId,
                ["category"] = uploadS3FileDto.Category.ToString().ToLowerInvariant(),
                ["document-id"] = uploadS3FileDto.DocumentId,
                ["version"] = version.ToString(),
                ["checksum"] = CalculateChecksum(fileBytes)
            };

            if (uploadS3FileDto.Metadata != null)
            {
                foreach (var kvp in uploadS3FileDto.Metadata)
                {
                    s3Metadata[kvp.Key] = kvp.Value;
                }
            }

            // Upload to S3
            memoryStream.Position = 0;
            var request = new PutObjectRequest()
            {
                BucketName = _settings.BucketName,
                Key = s3Key,
                InputStream = memoryStream,
                ContentType = uploadS3FileDto.ContentType,
            };

            foreach (var kvp in s3Metadata)
            {
                request.Metadata.Add(kvp.Key, kvp.Value);
            }

            var response = await s3Client.PutObjectAsync(request, cancellationToken);

            logger.LogInformation(
                "File uploaded: {S3Key}, Version: {Version}, Versioning: {Versioning}",
                s3Key,
                version,
                uploadS3FileDto.VersioningEnabled);

            // Clean up old versions if max versions is set
            if (uploadS3FileDto.VersioningEnabled &&
                !isNewDocument &&
                uploadS3FileDto.MaxVersions > 0 &&
                !string.IsNullOrWhiteSpace(uploadS3FileDto.S3Key))
            {
                await CleanupOldVersionsAsync(
                    uploadS3FileDto.S3Key,
                    uploadS3FileDto.MaxVersions,
                    cancellationToken);
            }

            return new UploadResult
            {
                Success = true,
                DocumentId = uploadS3FileDto.DocumentId,
                S3Key = s3Key,
                Filename = uploadS3FileDto.Filename,
                SizeBytes = fileBytes.Length,
                ContentType = uploadS3FileDto.ContentType,
                Version = version,
                VersioningEnabled = uploadS3FileDto.VersioningEnabled,
                S3VersionId = response.VersionId
            };
        }
        catch (AmazonS3Exception ex)
        {
            logger.LogError(ex, "S3 upload failed for file: {Filename}", uploadS3FileDto.Filename);
            return new UploadResult
            {
                Success = false,
                Filename = uploadS3FileDto.Filename,
                ErrorMessage = $"S3 upload failed: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Upload failed for file: {Filename}", uploadS3FileDto.Filename);
            throw;
        }
    }

    /// <summary>
    /// Get all versions of a document.
    /// </summary>
    public async Task<List<DocumentVersion>> GetDocumentVersionsAsync(
        string s3Key,
        CancellationToken cancellationToken = default)
    {
        // Also search previous years
        var versions = new List<DocumentVersion>();

        var response = await s3Client.ListVersionsAsync(new ListVersionsRequest()
        {
            BucketName = _settings.BucketName,
            Prefix = s3Key,
        }, cancellationToken);

        return response.Versions.OrderBy(e => e.LastModified)
            .Select((e, i) => new DocumentVersion()
            {
                S3Key = e.Key,
                SizeBytes = e.Size ?? 0,
                Version = i + 1,
                CreatedAt = e.LastModified ?? new DateTime(),
                IsCurrent = e.IsLatest ?? false,
                S3VersionId = e.VersionId
            }).ToList();
    }

    /// <inheritdoc />
    public async Task<(Stream Content, Dictionary<string, string> Metadata)> DownloadFileAsync(
        string s3Key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await s3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = s3Key
            }, cancellationToken);

            var metadata = new Dictionary<string, string>();
            foreach (var key in response.Metadata.Keys)
            {
                metadata[key] = response.Metadata[key];
            }

            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            return (memoryStream, metadata);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new FileNotFoundException($"File not found: {s3Key}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteFileAsync(string s3Key, CancellationToken cancellationToken = default)
    {
        try
        {
            await s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = s3Key
            }, cancellationToken);

            logger.LogInformation("File deleted: {S3Key}", s3Key);
            return true;
        }
        catch (AmazonS3Exception ex)
        {
            logger.LogError(ex, "Failed to delete file: {S3Key}", s3Key);
            throw;
        }
    }

    /// <inheritdoc />
    public string GeneratePresignedUrl(string s3Key, int expirationMinutes = 60, bool forUpload = false)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _settings.BucketName,
            Key = s3Key,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Verb = forUpload ? HttpVerb.PUT : HttpVerb.GET
        };

        return s3Client.GetPreSignedURL(request);
    }

    /// <inheritdoc />
    public async Task<bool> FileExistsAsync(string s3Key, CancellationToken cancellationToken = default)
    {
        try
        {
            await s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
            {
                BucketName = _settings.BucketName,
                Key = s3Key
            }, cancellationToken);

            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<List<S3FileInfo>> ListFilesAsync(
        string organizationId,
        DocumentCategory? category = null,
        int maxKeys = 1000,
        CancellationToken cancellationToken = default)
    {
        // New structure: organizations/{org}/{year}/{category}/
        var prefix = $"{OrganizationsPrefix}/{organizationId}/";

        var response = await s3Client.ListObjectsV2Async(new ListObjectsV2Request
        {
            BucketName = _settings.BucketName,
            Prefix = prefix,
            MaxKeys = maxKeys
        }, cancellationToken);

        var files = response.S3Objects.Select(obj => new S3FileInfo
        {
            Key = obj.Key,
            Size = obj.Size ?? 0,
            LastModified = obj.LastModified ?? DateTime.MinValue,
            ETag = obj.ETag?.Trim('"') ?? string.Empty
        });

        // Filter by category if specified
        if (category.HasValue)
        {
            var categoryStr = $"/{category.Value.ToString().ToLowerInvariant()}/";
            files = files.Where(f => f.Key.Contains(categoryStr));
        }

        return files.ToList();
    }

    /// <inheritdoc />
    public async Task<bool> CopyFileAsync(string sourceKey, string destKey, CancellationToken cancellationToken = default)
    {
        try
        {
            await s3Client.CopyObjectAsync(new CopyObjectRequest
            {
                SourceBucket = _settings.BucketName,
                SourceKey = sourceKey,
                DestinationBucket = _settings.BucketName,
                DestinationKey = destKey
            }, cancellationToken);

            logger.LogInformation("File copied from {Source} to {Dest}", sourceKey, destKey);
            return true;
        }
        catch (AmazonS3Exception ex)
        {
            logger.LogError(ex, "Failed to copy file from {Source} to {Dest}", sourceKey, destKey);
            throw;
        }
    }

    /// <summary>
    /// Check if versioning is enabled for a category in an organization.
    /// Uses database configuration first, then falls back to appsettings.
    /// </summary>
    public async Task<bool> IsVersioningEnabledAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default)
    {
        var (enabled, _) = await GetVersioningConfigAsync(organizationId, category, cancellationToken);
        return enabled;
    }

    /// <summary>
    /// Get versioning configuration from database first, fallback to appsettings.
    /// </summary>
    private async Task<(bool VersioningEnabled, int MaxVersions)> GetVersioningConfigAsync(
        string organizationId,
        DocumentCategory category,
        CancellationToken cancellationToken)
    {
        // Convert Web category to Domain category
        var domainCategory = MapToDomainCategory(category);

        // Try database configuration first
        var dbConfig = await versioningService.GetOrganizationConfigurationAsync(organizationId, cancellationToken);

        if (dbConfig != null)
        {
            var categoryConfig = dbConfig.CategoryConfigurations
                .FirstOrDefault(c => c.Category == domainCategory);

            if (categoryConfig != null)
            {
                return (categoryConfig.VersioningEnabled, categoryConfig.MaxVersions);
            }

            return (dbConfig.DefaultVersioningEnabled, dbConfig.DefaultMaxVersions);
        }

        // Fallback to appsettings configuration
        var orgConfig = _settings.GetOrganizationConfig(organizationId);
        return (orgConfig.IsVersioningEnabled(category), orgConfig.GetMaxVersions(category));
    }

    /// <summary>
    /// Map Web DocumentCategory to Domain DocumentCategory.
    /// </summary>
    private static DomainCategory MapToDomainCategory(DocumentCategory category)
    {
        return category switch
        {
            DocumentCategory.Invoices => DomainCategory.Invoices,
            DocumentCategory.Contracts => DomainCategory.Contracts,
            DocumentCategory.Reports => DomainCategory.Reports,
            DocumentCategory.Legal => DomainCategory.Legal,
            DocumentCategory.HumanResources => DomainCategory.HumanResources,
            DocumentCategory.Finance => DomainCategory.Finance,
            DocumentCategory.Technical => DomainCategory.Technical,
            DocumentCategory.General => DomainCategory.General,
            _ => DomainCategory.Other
        };
    }

    private async Task<int> GetNextVersionAsync(
        string s3Key,
        CancellationToken cancellationToken)
    {
        var versions = await GetDocumentVersionsAsync(s3Key, cancellationToken);
        return versions.Count > 0 ? versions.Max(v => v.Version) + 1 : 1;
    }

    private async Task CleanupOldVersionsAsync(
        string s3Key,
        int maxVersions,
        CancellationToken cancellationToken)
    {
        var versions = await GetDocumentVersionsAsync(s3Key, cancellationToken);

        if (versions.Count <= maxVersions)
            return;

        var versionsToDelete = versions
            .OrderByDescending(v => v.Version)
            .Skip(maxVersions)
            .ToList();

        foreach (var version in versionsToDelete)
        {
            try
            {
                await DeleteFileAsync(version.S3Key, cancellationToken);
                logger.LogInformation(
                    "Deleted old version {Version} of document {s3Key}",
                    version.Version, s3Key);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to delete old version {Version}", version.Version);
            }
        }
    }

    /// <summary>
    /// Generate S3 key with new organization structure.
    /// Format: organizations/{org_id}/{year}/{category}/{doc_id}_v{version}_{filename}
    /// </summary>
    private string GenerateS3Key(
        string organizationId,
        DocumentCategory category,
        string filename,
        int version)
    {
        var year = DateTime.UtcNow.ToString("yyyy");
        var safeFilename = SanitizeFilename(filename);
        var categoryFolder = category.ToString().ToLowerInvariant();

        return $"{OrganizationsPrefix}/{organizationId}/{year}/{categoryFolder}/v{version}_{safeFilename}";
    }

    private static string SanitizeFilename(string filename)
    {
        var name = Path.GetFileName(filename);
        name = Regex.Replace(name, @"[/\\:*?""<>|\s]", "_");

        if (name.Length > 200)
        {
            var ext = Path.GetExtension(name);
            name = name[..(200 - ext.Length)] + ext;
        }

        return name;
    }

    private void ValidateFile(string filename, long sizeBytes)
    {
        var ext = Path.GetExtension(filename).ToLowerInvariant();

        if (!_settings.AllowedExtensions.Contains(ext))
        {
            throw new InvalidOperationException(
                $"File extension '{ext}' not allowed. Allowed: {string.Join(", ", _settings.AllowedExtensions)}");
        }

        var maxSizeBytes = _settings.MaxFileSizeMb * 1024L * 1024L;
        if (sizeBytes > maxSizeBytes)
        {
            throw new InvalidOperationException(
                $"File size ({sizeBytes / 1024.0 / 1024.0:F2} MB) exceeds maximum allowed ({_settings.MaxFileSizeMb} MB)");
        }
    }

    private static string CalculateChecksum(byte[] content)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(content);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string GetContentType(string filename)
    {
        var ext = Path.GetExtension(filename).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".json" => "application/json",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }
}
