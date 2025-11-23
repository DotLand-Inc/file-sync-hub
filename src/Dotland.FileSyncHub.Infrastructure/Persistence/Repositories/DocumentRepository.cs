using Dotland.FileSyncHub.Domain.Entities;
using Dotland.FileSyncHub.Domain.Enums;
using Dotland.FileSyncHub.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Document entity
/// </summary>
public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(FileSyncHubDbContext context) : base(context)
    {
    }

    public async Task<Document?> GetByIdWithVersionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Versions)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Document?> GetByIdWithHistoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.StatusHistory)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Document?> GetByIdFullAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Versions)
            .Include(d => d.StatusHistory)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByOrganizationAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(d => d.OrganizationId == organizationId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByOrganizationAndCategoryAsync(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(d => d.OrganizationId == organizationId && d.Category == category)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByStatusAsync(DocumentStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(d => d.Status == status)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByWorkflowInstanceIdAsync(string workflowInstanceId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(d => d.WorkflowInstanceId == workflowInstanceId)
            .ToListAsync(cancellationToken);
    }
}
