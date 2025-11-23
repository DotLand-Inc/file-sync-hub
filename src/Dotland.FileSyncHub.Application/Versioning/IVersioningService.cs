using Dotland.FileSyncHub.Application.Versioning.DTOs;
using Dotland.FileSyncHub.Domain.Enums;

namespace Dotland.FileSyncHub.Application.Versioning;

/// <summary>
/// Service for managing versioning configurations
/// </summary>
public interface IVersioningService
{
    Task<OrganizationVersioningConfigurationDto?> GetOrganizationConfigurationAsync(string organizationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrganizationVersioningConfigurationDto>> GetAllConfigurationsAsync(CancellationToken cancellationToken = default);
    Task<OrganizationVersioningConfigurationDto> CreateOrganizationConfigurationAsync(CreateOrganizationVersioningConfigurationDto dto, string? createdBy = null, CancellationToken cancellationToken = default);
    Task<OrganizationVersioningConfigurationDto> UpdateOrganizationConfigurationAsync(string organizationId, UpdateOrganizationVersioningConfigurationDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);
    Task SetCategoryConfigurationAsync(string organizationId, SetCategoryVersioningConfigurationDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);
    Task RemoveCategoryConfigurationAsync(string organizationId, DocumentCategory category, string? updatedBy = null, CancellationToken cancellationToken = default);
    Task<bool> IsVersioningEnabledAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default);
    Task<int> GetMaxVersionsAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default);
    Task DeleteOrganizationConfigurationAsync(string organizationId, CancellationToken cancellationToken = default);
}
