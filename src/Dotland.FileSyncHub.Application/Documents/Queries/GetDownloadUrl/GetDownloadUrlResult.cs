namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDownloadUrl;

/// <summary>
/// Result containing the presigned download URL.
/// </summary>
public class GetDownloadUrlResult
{
    /// <summary>
    /// Presigned download URL.
    /// </summary>
    public string DownloadUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL expiration time in minutes.
    /// </summary>
    public int ExpiresInMinutes { get; set; }
}
