namespace Dotland.FileSyncHub.Web.Models;

/// <summary>
/// Versioning configuration for a specific category within an organization.
/// </summary>
public class CategoryVersioningConfig
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

/// <summary>
/// Versioning configuration for an organization.
/// </summary>
public class OrganizationVersioningConfig
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
    public List<CategoryVersioningConfig> CategoryConfigs { get; set; } = new();

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
