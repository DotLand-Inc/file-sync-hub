namespace Dotland.FileSyncHub.Application.Documents.Queries.DocumentDirectDownload;

/// <summary>
/// Result containing document content and metadata for direct download.
/// </summary>
public class DocumentDirectDownloadResult
{
    /// <summary>
    /// File content as byte array.
    /// </summary>
    public byte[] Content { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Original filename.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Content type (MIME type).
    /// </summary>
    public string ContentType { get; set; } = "application/octet-stream";
}
