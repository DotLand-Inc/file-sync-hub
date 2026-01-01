using Dotland.FileSyncHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Common.Services;

public interface IApplicationDbContext
{
    DbSet<OrganizationVersioningConfiguration> OrganizationVersioningConfigurations { get; }
    DbSet<CategoryVersioningConfiguration> CategoryVersioningConfigurations { get; }
    DbSet<Document> Documents { get; }
    DbSet<DocumentVersion> DocumentVersions { get; }
    DbSet<DocumentStatusHistory> DocumentStatusHistory { get; }
    DbSet<DocumentRelation> DocumentRelations { get;  }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}