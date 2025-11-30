using Dotland.FileSyncHub.Application.Common.Models;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentsList;

/// <summary>
/// Result containing list of documents.
/// </summary>
public class GetDocumentsListResult
{
    /// <summary>
    /// List of files.
    /// </summary>
    public List<S3FileInfo> Files { get; set; } = new();

    /// <summary>
    /// Total number of files.
    /// </summary>
    public int Count { get; set; }
}
