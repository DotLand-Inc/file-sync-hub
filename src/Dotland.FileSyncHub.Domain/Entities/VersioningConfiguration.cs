using Dotland.FileSyncHub.Domain.Common;
using Dotland.FileSyncHub.Domain.Enums;

namespace Dotland.FileSyncHub.Domain.Entities;

/// <summary>
/// Versioning configuration for an organization
/// </summary>
public class OrganizationVersioningConfiguration : Entity
{
    public string OrganizationId { get; private set; } = null!;
    public bool DefaultVersioningEnabled { get; set; }
    public int DefaultMaxVersions { get; set; }
    public bool IsActive { get; private set; }

    private readonly List<CategoryVersioningConfiguration> _categoryConfigurations = new();
    public IReadOnlyCollection<CategoryVersioningConfiguration> CategoryConfigurations => _categoryConfigurations.AsReadOnly();

    private OrganizationVersioningConfiguration() { }

    public static OrganizationVersioningConfiguration Create(
        string organizationId,
        bool defaultVersioningEnabled = false,
        int defaultMaxVersions = 0,
        string? createdBy = null)
    {
        return new OrganizationVersioningConfiguration
        {
            OrganizationId = organizationId,
            DefaultVersioningEnabled = defaultVersioningEnabled,
            DefaultMaxVersions = defaultMaxVersions,
            IsActive = true,
            CreatedBy = createdBy
        };
    }

    public void Update(bool defaultVersioningEnabled, int defaultMaxVersions, string? updatedBy = null)
    {
        DefaultVersioningEnabled = defaultVersioningEnabled;
        DefaultMaxVersions = defaultMaxVersions;
        SetUpdated(updatedBy);
    }

    public void Activate(string? updatedBy = null)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }

    public void Deactivate(string? updatedBy = null)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }

    public void AddCategoryConfiguration(CategoryVersioningConfiguration config)
    {
        var existing = _categoryConfigurations.FirstOrDefault(c => c.Category == config.Category);
        if (existing != null)
        {
            _categoryConfigurations.Remove(existing);
        }
        _categoryConfigurations.Add(config);
    }

    public void RemoveCategoryConfiguration(DocumentCategory category)
    {
        var config = _categoryConfigurations.FirstOrDefault(c => c.Category == category);
        if (config != null)
        {
            _categoryConfigurations.Remove(config);
        }
    }

    public bool IsVersioningEnabled(DocumentCategory category)
    {
        var config = _categoryConfigurations.FirstOrDefault(c => c.Category == category);
        return config?.VersioningEnabled ?? DefaultVersioningEnabled;
    }

    public int GetMaxVersions(DocumentCategory category)
    {
        var config = _categoryConfigurations.FirstOrDefault(c => c.Category == category);
        return config?.MaxVersions ?? DefaultMaxVersions;
    }
}

/// <summary>
/// Category-specific versioning configuration
/// </summary>
public class CategoryVersioningConfiguration : Entity
{
    public Guid OrganizationVersioningConfigurationId { get; private set; }
    public DocumentCategory Category { get; private set; }
    public bool VersioningEnabled { get; private set; }
    public int MaxVersions { get; private set; }

    public OrganizationVersioningConfiguration OrganizationConfiguration { get; private set; } = null!;

    private CategoryVersioningConfiguration() { }

    public static CategoryVersioningConfiguration Create(
        Guid organizationVersioningConfigurationId,
        DocumentCategory category,
        bool versioningEnabled,
        int maxVersions = 0,
        string? createdBy = null)
    {
        return new CategoryVersioningConfiguration
        {
            OrganizationVersioningConfigurationId = organizationVersioningConfigurationId,
            Category = category,
            VersioningEnabled = versioningEnabled,
            MaxVersions = maxVersions,
            CreatedBy = createdBy
        };
    }

    public void Update(bool versioningEnabled, int maxVersions, string? updatedBy = null)
    {
        VersioningEnabled = versioningEnabled;
        MaxVersions = maxVersions;
        SetUpdated(updatedBy);
    }
}
