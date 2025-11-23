using Dotland.FileSyncHub.Domain.Entities;
using Dotland.FileSyncHub.Domain.Enums;

namespace Dotland.FileSyncHub.Domain.Repositories;

/// <summary>
/// Repository interface for Document entity
/// </summary>
public interface IDocumentRepository : IRepository<Document>
{
    Task<Document?> GetByIdWithVersionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Document?> GetByIdWithHistoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Document?> GetByIdFullAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByOrganizationAsync(string organizationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByOrganizationAndCategoryAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByStatusAsync(DocumentStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByWorkflowInstanceIdAsync(string workflowInstanceId, CancellationToken cancellationToken = default);
}
