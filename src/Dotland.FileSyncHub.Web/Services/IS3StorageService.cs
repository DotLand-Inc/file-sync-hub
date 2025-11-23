using Dotland.FileSyncHub.Web.Models;

namespace Dotland.FileSyncHub.Web.Services;

/// <summary>
/// Interface for S3 storage operations.
/// </summary>
public interface IS3StorageService
{
    /// <summary>
    /// Upload a file to S3.
    /// </summary>
    /// <param name="fileStream">File content stream.</param>
    /// <param name="filename">Original filename.</param>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="category">Document category.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="metadata">Optional metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload result with details.</returns>
    Task<UploadResult> UploadFileAsync(
        Stream fileStream,
        string filename,
        string organizationId,
        DocumentCategory category = DocumentCategory.Other,
        string? contentType = null,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a file from S3.
    /// </summary>
    /// <param name="s3Key">S3 object key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File stream and metadata.</returns>
    Task<(Stream Content, Dictionary<string, string> Metadata)> DownloadFileAsync(
        string s3Key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file from S3.
    /// </summary>
    /// <param name="s3Key">S3 object key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> DeleteFileAsync(string s3Key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a presigned URL for file access.
    /// </summary>
    /// <param name="s3Key">S3 object key.</param>
    /// <param name="expirationMinutes">URL expiration time in minutes.</param>
    /// <param name="forUpload">If true, generate URL for upload.</param>
    /// <returns>Presigned URL.</returns>
    string GeneratePresignedUrl(string s3Key, int expirationMinutes = 60, bool forUpload = false);

    /// <summary>
    /// Check if a file exists in S3.
    /// </summary>
    /// <param name="s3Key">S3 object key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if file exists.</returns>
    Task<bool> FileExistsAsync(string s3Key, CancellationToken cancellationToken = default);

    /// <summary>
    /// List files for an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="category">Optional category filter.</param>
    /// <param name="maxKeys">Maximum number of keys to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of file information.</returns>
    Task<List<S3FileInfo>> ListFilesAsync(
        string organizationId,
        DocumentCategory? category = null,
        int maxKeys = 1000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Copy a file within S3.
    /// </summary>
    /// <param name="sourceKey">Source S3 key.</param>
    /// <param name="destKey">Destination S3 key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> CopyFileAsync(string sourceKey, string destKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if versioning is enabled for a category in an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="category">Document category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if versioning is enabled.</returns>
    Task<bool> IsVersioningEnabledAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all versions of a document.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="category">Document category.</param>
    /// <param name="documentId">Document identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of document versions.</returns>
    Task<List<DocumentVersion>> GetDocumentVersionsAsync(
        string organizationId,
        DocumentCategory category,
        string documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload a new version of an existing document.
    /// </summary>
    /// <param name="fileStream">File content stream.</param>
    /// <param name="filename">Original filename.</param>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="category">Document category.</param>
    /// <param name="documentId">Existing document ID.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="metadata">Optional metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload result with details.</returns>
    Task<UploadResult> UploadFileAsync(
        Stream fileStream,
        string filename,
        string organizationId,
        DocumentCategory category,
        string? documentId,
        string? contentType = null,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);
}
