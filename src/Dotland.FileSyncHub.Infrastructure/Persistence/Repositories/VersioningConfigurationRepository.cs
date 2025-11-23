using Dotland.FileSyncHub.Domain.Entities;
using Dotland.FileSyncHub.Domain.Enums;
using Dotland.FileSyncHub.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for versioning configuration
/// </summary>
public class VersioningConfigurationRepository : Repository<OrganizationVersioningConfiguration>, IVersioningConfigurationRepository
{
    public VersioningConfigurationRepository(FileSyncHubDbContext context) : base(context)
    {
    }

    public async Task<OrganizationVersioningConfiguration?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(c => c.OrganizationId == organizationId && c.IsActive, cancellationToken);
    }

    public async Task<OrganizationVersioningConfiguration?> GetByOrganizationIdWithCategoriesAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.CategoryConfigurations)
            .FirstOrDefaultAsync(c => c.OrganizationId == organizationId && c.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<OrganizationVersioningConfiguration>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.CategoryConfigurations)
            .Where(c => c.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsVersioningEnabledAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default)
    {
        var config = await GetByOrganizationIdWithCategoriesAsync(organizationId, cancellationToken);

        if (config == null)
            return false;

        return config.IsVersioningEnabled(category);
    }

    public async Task<int> GetMaxVersionsAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default)
    {
        var config = await GetByOrganizationIdWithCategoriesAsync(organizationId, cancellationToken);

        if (config == null)
            return 0;

        return config.GetMaxVersions(category);
    }
}
