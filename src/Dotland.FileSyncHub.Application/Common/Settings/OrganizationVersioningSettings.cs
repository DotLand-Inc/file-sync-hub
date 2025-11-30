using Dotland.FileSyncHub.Domain.Enums;

namespace Dotland.FileSyncHub.Application.Common.Settings;

/// <summary>
/// Versioning configuration for an organization.
/// </summary>
public class OrganizationVersioningSettings
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public string OrganizationId { get; set; } = string.Empty;

    /// <summary>
    /// Default versioning setting for categories not explicitly configured.
    /// </summary>
    public bool DefaultVersioningEnabled { get; set; } = false;

    /// <summary>
    /// Category-specific versioning configurations.
    /// </summary>
    public List<CategoryVersioningSettings> CategoryConfigs { get; set; } = new();

    /// <summary>
    /// Check if versioning is enabled for a specific category.
    /// </summary>
    public bool IsVersioningEnabled(DocumentCategory category)
    {
        var config = CategoryConfigs.FirstOrDefault(c => c.Category == category);
        return config?.VersioningEnabled ?? DefaultVersioningEnabled;
    }

    /// <summary>
    /// Get max versions for a specific category.
    /// </summary>
    public int GetMaxVersions(DocumentCategory category)
    {
        var config = CategoryConfigs.FirstOrDefault(c => c.Category == category);
        return config?.MaxVersions ?? 0;
    }
}