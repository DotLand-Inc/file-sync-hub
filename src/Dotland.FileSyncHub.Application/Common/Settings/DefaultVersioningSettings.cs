namespace Dotland.FileSyncHub.Application.Common.Settings;

/// <summary>
/// Default versioning settings applied when organization-specific config is not found.
/// </summary>
public class DefaultVersioningSettings
{
    /// <summary>
    /// Whether versioning is enabled by default.
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Default category versioning configurations.
    /// </summary>
    public List<CategoryVersioningSettings> CategoryDefaults { get; set; } = [];
}