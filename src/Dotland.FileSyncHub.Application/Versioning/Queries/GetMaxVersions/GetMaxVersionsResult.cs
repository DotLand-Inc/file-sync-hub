namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetMaxVersions;

/// <summary>
/// Result containing the maximum number of versions.
/// </summary>
public class GetMaxVersionsResult
{
    /// <summary>
    /// Maximum number of versions allowed.
    /// </summary>
    public int MaxVersions { get; set; }
}
