using Dotland.FileSyncHub.Application.Versioning.DTOs;
using Dotland.FileSyncHub.Domain.Enums;

namespace Dotland.FileSyncHub.Application.Versioning;

/// <summary>
/// Service for managing versioning configurations
/// </summary>
public interface IVersioningService
{
    Task<OrganizationVersioningConfigurationDto?> GetOrganizationConfigurationAsync(string organizationId, CancellationToken cancellationToken = default);
    Task<bool> IsVersioningEnabledAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default);
    Task<int> GetMaxVersionsAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default);
}
