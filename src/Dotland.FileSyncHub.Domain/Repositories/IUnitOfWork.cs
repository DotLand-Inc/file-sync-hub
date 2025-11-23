namespace Dotland.FileSyncHub.Domain.Repositories;

/// <summary>
/// Unit of Work interface for transaction management
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IDocumentRepository Documents { get; }
    IDocumentVersionRepository DocumentVersions { get; }
    IVersioningConfigurationRepository VersioningConfigurations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
