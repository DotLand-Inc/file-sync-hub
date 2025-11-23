namespace Dotland.FileSyncHub.Web.Models;

/// <summary>
/// Information about a file stored in S3.
/// </summary>
public class S3FileInfo
{
    /// <summary>
    /// S3 object key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Last modified timestamp.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// ETag (entity tag) for the object.
    /// </summary>
    public string ETag { get; set; } = string.Empty;
}
