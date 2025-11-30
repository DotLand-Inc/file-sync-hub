using Dotland.FileSyncHub.Domain.Enums;

namespace Dotland.FileSyncHub.Application.Common.Settings;

/// <summary>
/// Versioning configuration for a specific category within an organization.
/// </summary>
public class CategoryVersioningSettings
{
    /// <summary>
    /// Document category.
    /// </summary>
    public DocumentCategory Category { get; set; }

    /// <summary>
    /// Whether versioning is enabled for this category.
    /// </summary>
    public bool VersioningEnabled { get; set; } = false;

    /// <summary>
    /// Maximum number of versions to keep (0 = unlimited).
    /// </summary>
    public int MaxVersions { get; set; } = 0;
}