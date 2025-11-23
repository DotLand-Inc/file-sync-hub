using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.Model;
using GedBackend.Configuration;
using GedBackend.Models;
using Microsoft.Extensions.Options;

namespace GedBackend.Services;

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
/// </summary>
public class S3StorageService : IS3StorageService
{
    private const string OrganizationsPrefix = "organizations";
    private const string SystemPrefix = "system";

    private readonly IAmazonS3 _s3Client;
    private readonly S3Settings _settings;
    private readonly ILogger<S3StorageService> _logger;

    public S3StorageService(
        IAmazonS3 s3Client,
        IOptions<S3Settings> settings,
        ILogger<S3StorageService> logger)
    {
        _s3Client = s3Client;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<UploadResult> UploadFileAsync(
        Stream fileStream,
        string filename,
        string organizationId,
        DocumentCategory category = DocumentCategory.Other,
        string? contentType = null,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        return await UploadFileAsync(
            fileStream, filename, organizationId, category,
            documentId: null, contentType, metadata, cancellationToken);
    }

    /// <summary>
    /// Upload a file with optional document ID for versioning.
    /// </summary>
    public async Task<UploadResult> UploadFileAsync(
        Stream fileStream,
        string filename,
        string organizationId,
        DocumentCategory category,
        string? documentId,
        string? contentType = null,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Read stream to memory for validation and checksum
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();

            // Validate file
            ValidateFile(filename, fileBytes.Length);

            // Get versioning config
            var orgConfig = _settings.GetOrganizationConfig(organizationId);
            var versioningEnabled = orgConfig.IsVersioningEnabled(category);

            // Determine version number
            var version = 1;
            var isNewDocument = string.IsNullOrEmpty(documentId);

            if (isNewDocument)
            {
                documentId = Guid.NewGuid().ToString();
            }
            else if (versioningEnabled)
            {
                // Get current max version
                version = await GetNextVersionAsync(organizationId, category, documentId!, cancellationToken);
            }

            // Generate S3 key with new structure
            var s3Key = GenerateS3Key(organizationId, category, filename, documentId!, version);

            // Auto-detect content type if not provided
            contentType ??= GetContentType(filename);

            // Prepare metadata
            var s3Metadata = new Dictionary<string, string>
            {
                ["original-filename"] = filename,
                ["organization-id"] = organizationId,
                ["category"] = category.ToString().ToLowerInvariant(),
                ["document-id"] = documentId!,
                ["version"] = version.ToString(),
                ["checksum"] = CalculateChecksum(fileBytes)
            };

            if (metadata != null)
            {
                foreach (var kvp in metadata)
                {
                    s3Metadata[kvp.Key] = kvp.Value;
                }
            }

            // Upload to S3
            memoryStream.Position = 0;
            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = s3Key,
                InputStream = memoryStream,
                ContentType = contentType
            };

            foreach (var kvp in s3Metadata)
            {
                request.Metadata.Add(kvp.Key, kvp.Value);
            }

            var response = await _s3Client.PutObjectAsync(request, cancellationToken);

            _logger.LogInformation(
                "File uploaded: {S3Key}, Version: {Version}, Versioning: {Versioning}",
                s3Key, version, versioningEnabled);

            // Clean up old versions if max versions is set
            if (versioningEnabled && !isNewDocument)
            {
                var maxVersions = orgConfig.GetMaxVersions(category);
                if (maxVersions > 0)
                {
                    await CleanupOldVersionsAsync(organizationId, category, documentId!, maxVersions, cancellationToken);
                }
            }

            return new UploadResult
            {
                Success = true,
                DocumentId = documentId!,
                S3Key = s3Key,
                Filename = filename,
                SizeBytes = fileBytes.Length,
                ContentType = contentType,
                Version = version,
                VersioningEnabled = versioningEnabled,
                S3VersionId = response.VersionId
            };
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 upload failed for file: {Filename}", filename);
            return new UploadResult
            {
                Success = false,
                Filename = filename,
                ErrorMessage = $"S3 upload failed: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed for file: {Filename}", filename);
            throw;
        }
    }

    /// <summary>
    /// Get all versions of a document.
    /// </summary>
    public async Task<List<DocumentVersion>> GetDocumentVersionsAsync(
        string organizationId,
        DocumentCategory category,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        var prefix = $"{OrganizationsPrefix}/{organizationId}/{DateTime.UtcNow.Year}/{category.ToString().ToLowerInvariant()}/{documentId}_";

        // Also search previous years
        var versions = new List<DocumentVersion>();
        var currentYear = DateTime.UtcNow.Year;

        for (var year = currentYear; year >= currentYear - 5; year--)
        {
            var yearPrefix = $"{OrganizationsPrefix}/{organizationId}/{year}/{category.ToString().ToLowerInvariant()}/{documentId}_";

            var response = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = _settings.BucketName,
                Prefix = yearPrefix
            }, cancellationToken);

            foreach (var obj in response.S3Objects)
            {
                var versionMatch = Regex.Match(obj.Key, @"_v(\d+)_");
                if (versionMatch.Success && int.TryParse(versionMatch.Groups[1].Value, out var ver))
                {
                    versions.Add(new DocumentVersion
                    {
                        Version = ver,
                        S3Key = obj.Key,
                        SizeBytes = obj.Size ?? 0,
                        CreatedAt = obj.LastModified ?? DateTime.MinValue,
                        IsCurrent = false
                    });
                }
            }
        }

        // Mark the highest version as current
        if (versions.Count > 0)
        {
            var maxVersion = versions.Max(v => v.Version);
            var current = versions.First(v => v.Version == maxVersion);
            current.IsCurrent = true;
        }

        return versions.OrderByDescending(v => v.Version).ToList();
    }

    /// <inheritdoc />
    public async Task<(Stream Content, Dictionary<string, string> Metadata)> DownloadFileAsync(
        string s3Key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _s3Client.GetObjectAsync(new GetObjectRequest
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
            await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = s3Key
            }, cancellationToken);

            _logger.LogInformation("File deleted: {S3Key}", s3Key);
            return true;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {S3Key}", s3Key);
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

        return _s3Client.GetPreSignedURL(request);
    }

    /// <inheritdoc />
    public async Task<bool> FileExistsAsync(string s3Key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
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

        var response = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
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
            await _s3Client.CopyObjectAsync(new CopyObjectRequest
            {
                SourceBucket = _settings.BucketName,
                SourceKey = sourceKey,
                DestinationBucket = _settings.BucketName,
                DestinationKey = destKey
            }, cancellationToken);

            _logger.LogInformation("File copied from {Source} to {Dest}", sourceKey, destKey);
            return true;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to copy file from {Source} to {Dest}", sourceKey, destKey);
            throw;
        }
    }

    /// <summary>
    /// Check if versioning is enabled for a category in an organization.
    /// </summary>
    public bool IsVersioningEnabled(string organizationId, DocumentCategory category)
    {
        var orgConfig = _settings.GetOrganizationConfig(organizationId);
        return orgConfig.IsVersioningEnabled(category);
    }

    private async Task<int> GetNextVersionAsync(
        string organizationId,
        DocumentCategory category,
        string documentId,
        CancellationToken cancellationToken)
    {
        var versions = await GetDocumentVersionsAsync(organizationId, category, documentId, cancellationToken);
        return versions.Count > 0 ? versions.Max(v => v.Version) + 1 : 1;
    }

    private async Task CleanupOldVersionsAsync(
        string organizationId,
        DocumentCategory category,
        string documentId,
        int maxVersions,
        CancellationToken cancellationToken)
    {
        var versions = await GetDocumentVersionsAsync(organizationId, category, documentId, cancellationToken);

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
                _logger.LogInformation(
                    "Deleted old version {Version} of document {DocumentId}",
                    version.Version, documentId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete old version {Version}", version.Version);
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
        string documentId,
        int version)
    {
        var year = DateTime.UtcNow.ToString("yyyy");
        var safeFilename = SanitizeFilename(filename);
        var categoryFolder = category.ToString().ToLowerInvariant();

        return $"{OrganizationsPrefix}/{organizationId}/{year}/{categoryFolder}/{documentId}_v{version}_{safeFilename}";
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
