using Dotland.FileSyncHub.Domain.Entities;

namespace Dotland.FileSyncHub.Domain.Repositories;

/// <summary>
/// Repository interface for DocumentVersion entity
/// </summary>
public interface IDocumentVersionRepository : IRepository<DocumentVersion>
{
    Task<IReadOnlyList<DocumentVersion>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<DocumentVersion?> GetLatestVersionAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<DocumentVersion?> GetByVersionNumberAsync(Guid documentId, int versionNumber, CancellationToken cancellationToken = default);
    Task<int> GetVersionCountAsync(Guid documentId, CancellationToken cancellationToken = default);
}
