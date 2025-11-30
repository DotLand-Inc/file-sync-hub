using Dotland.FileSyncHub.Application.Common.Models;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentVersions;

/// <summary>
/// Result containing document versions information.
/// </summary>
public class GetDocumentVersionsResult
{
    /// <summary>
    /// Document identifier.
    /// </summary>
    public string DocumentId { get; set; } = string.Empty;

    /// <summary>
    /// Whether versioning is enabled for this category.
    /// </summary>
    public bool VersioningEnabled { get; set; }

    /// <summary>
    /// List of all versions.
    /// </summary>
    public List<DocumentVersion> Versions { get; set; } = new();

    /// <summary>
    /// Total number of versions.
    /// </summary>
    public int Count { get; set; }
}
