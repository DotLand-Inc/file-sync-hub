using Dotland.FileSyncHub.Domain.Entities;
using Dotland.FileSyncHub.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for DocumentVersion entity
/// </summary>
public class DocumentVersionRepository : Repository<DocumentVersion>, IDocumentVersionRepository
{
    public DocumentVersionRepository(FileSyncHubDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<DocumentVersion>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<DocumentVersion?> GetLatestVersionAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(v => v.DocumentId == documentId && v.IsActive)
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DocumentVersion?> GetByVersionNumberAsync(Guid documentId, int versionNumber, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(v => v.DocumentId == documentId && v.VersionNumber == versionNumber, cancellationToken);
    }

    public async Task<int> GetVersionCountAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(v => v.DocumentId == documentId, cancellationToken);
    }
}
