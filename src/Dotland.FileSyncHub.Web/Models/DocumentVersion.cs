namespace Dotland.FileSyncHub.Web.Models;

/// <summary>
/// Represents a version of a document.
/// </summary>
public class DocumentVersion
{
    /// <summary>
    /// Version number (1, 2, 3, ...).
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// S3 object key for this version.
    /// </summary>
    public string S3Key { get; set; } = string.Empty;

    /// <summary>
    /// S3 version ID (if S3 versioning is enabled).
    /// </summary>
    public string? S3VersionId { get; set; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long SizeBytes { get; set; }

    /// <summary>
    /// Upload timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Whether this is the current/latest version.
    /// </summary>
    public bool IsCurrent { get; set; }
}
