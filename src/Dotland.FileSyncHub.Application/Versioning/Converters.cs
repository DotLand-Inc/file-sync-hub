using Dotland.FileSyncHub.Application.Versioning.DTOs;
using Dotland.FileSyncHub.Domain.Entities;

namespace Dotland.FileSyncHub.Application.Versioning;

public static class Converters
{
    public static OrganizationVersioningConfigurationDto MapToOrganizationVersioningConfigurationDto(this OrganizationVersioningConfiguration config)
    {
        return new OrganizationVersioningConfigurationDto(
            config.Id,
            config.OrganizationId,
            config.DefaultVersioningEnabled,
            config.DefaultMaxVersions,
            config.IsActive,
            config.CategoryConfigurations.Select(c => new CategoryVersioningConfigurationDto(
                c.Id,
                c.Category,
                c.VersioningEnabled,
                c.MaxVersions
            )).ToList()
        );
    }
}