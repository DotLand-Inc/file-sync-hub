using Dotland.FileSyncHub.Application.Versioning.DTOs;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetAllVersioningConfigurations;

/// <summary>
/// Result containing all versioning configurations.
/// </summary>
public class GetAllVersioningConfigurationsResult
{
    /// <summary>
    /// List of all versioning configurations.
    /// </summary>
    public IReadOnlyList<OrganizationVersioningConfigurationDto> Configurations { get; set; } = new List<OrganizationVersioningConfigurationDto>();
}
