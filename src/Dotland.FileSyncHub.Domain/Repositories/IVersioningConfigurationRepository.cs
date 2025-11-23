using Dotland.FileSyncHub.Domain.Entities;
using Dotland.FileSyncHub.Domain.Enums;

namespace Dotland.FileSyncHub.Domain.Repositories;

/// <summary>
/// Repository interface for versioning configuration
/// </summary>
public interface IVersioningConfigurationRepository : IRepository<OrganizationVersioningConfiguration>
{
    Task<OrganizationVersioningConfiguration?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken = default);
    Task<OrganizationVersioningConfiguration?> GetByOrganizationIdWithCategoriesAsync(string organizationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrganizationVersioningConfiguration>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<bool> IsVersioningEnabledAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default);
    Task<int> GetMaxVersionsAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default);
}
