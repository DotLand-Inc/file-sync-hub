namespace Dotland.FileSyncHub.Application.Common.Settings;

/// <summary>
/// Configuration settings for AWS S3 storage.
/// </summary>
public class S3Settings
{
    public const string SectionName = "S3";

    /// <summary>
    /// AWS Region (e.g., "eu-west-1").
    /// </summary>
    public string Region { get; set; } = "eu-west-1";

    /// <summary>
    /// S3 bucket name for document storage.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Optional custom endpoint URL (for LocalStack or S3-compatible services).
    /// </summary>
    public string? ServiceUrl { get; set; }

    /// <summary>
    /// Maximum file size in MB.
    /// </summary>
    public int MaxFileSizeMb { get; set; } = 100;

    /// <summary>
    /// Allowed file extensions.
    /// </summary>
    public string[] AllowedExtensions { get; set; } =
    [
        ".pdf", ".doc", ".docx", ".xls", ".xlsx",
        ".ppt", ".pptx", ".txt", ".csv", ".json",
        ".png", ".jpg", ".jpeg", ".gif", ".bmp"
    ];

    /// <summary>
    /// Default versioning configuration for organizations not explicitly configured.
    /// </summary>
    public DefaultVersioningSettings DefaultVersioning { get; set; } = new();

    /// <summary>
    /// Organization-specific versioning configurations.
    /// </summary>
    public List<OrganizationVersioningSettings> OrganizationVersioning { get; set; } = [];

    /// <summary>
    /// Get versioning config for an organization.
    /// </summary>
    public OrganizationVersioningSettings GetOrganizationConfig(string organizationId)
    {
        var config = OrganizationVersioning.FirstOrDefault(o =>
            o.OrganizationId.Equals(organizationId, StringComparison.OrdinalIgnoreCase));
    
        if (config != null)
            return config;
    
        // Return default config
        return new OrganizationVersioningSettings
        {
            OrganizationId = organizationId,
            DefaultVersioningEnabled = DefaultVersioning.Enabled,
            CategoryConfigs = DefaultVersioning.CategoryDefaults
        };
    }
}


