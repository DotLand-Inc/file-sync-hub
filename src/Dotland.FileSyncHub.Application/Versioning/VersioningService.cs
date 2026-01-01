using Dotland.FileSyncHub.Application.Versioning.DTOs;
using Dotland.FileSyncHub.Domain.Entities;
using Dotland.FileSyncHub.Domain.Enums;
using Dotland.FileSyncHub.Domain.Repositories;

namespace Dotland.FileSyncHub.Application.Versioning;

/// <summary>
/// Service implementation for managing versioning configurations
/// </summary>
public class VersioningService(IUnitOfWork unitOfWork) : IVersioningService
{
    public async Task<OrganizationVersioningConfigurationDto?> GetOrganizationConfigurationAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        var config = await unitOfWork.VersioningConfigurations.GetByOrganizationIdWithCategoriesAsync(organizationId, cancellationToken);

        if (config == null)
            return null;

        return MapToDto(config);
    }
    
    public async Task<bool> IsVersioningEnabledAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.VersioningConfigurations.IsVersioningEnabledAsync(organizationId, category, cancellationToken);
    }

    public async Task<int> GetMaxVersionsAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.VersioningConfigurations.GetMaxVersionsAsync(organizationId, category, cancellationToken);
    }

    private static OrganizationVersioningConfigurationDto MapToDto(OrganizationVersioningConfiguration config)
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
