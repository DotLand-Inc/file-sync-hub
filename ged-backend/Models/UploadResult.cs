namespace GedBackend.Models;

/// <summary>
/// Result of a file upload operation.
/// </summary>
public class UploadResult
{
    /// <summary>
    /// Whether the upload was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Unique document identifier.
    /// </summary>
    public string DocumentId { get; set; } = string.Empty;

    /// <summary>
    /// S3 object key.
    /// </summary>
    public string S3Key { get; set; } = string.Empty;

    /// <summary>
    /// Original filename.
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long SizeBytes { get; set; }

    /// <summary>
    /// Content MIME type.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Document version number (1 for new documents, incremented for updates).
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Whether versioning is enabled for this document's category.
    /// </summary>
    public bool VersioningEnabled { get; set; }

    /// <summary>
    /// S3 version ID if S3 versioning is enabled on the bucket.
    /// </summary>
    public string? S3VersionId { get; set; }

    /// <summary>
    /// Error message if upload failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
