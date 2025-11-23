using Dotland.FileSyncHub.Domain.Repositories;
using Dotland.FileSyncHub.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dotland.FileSyncHub.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for transaction management
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly FileSyncHubDbContext _context;
    private IDbContextTransaction? _transaction;

    private IDocumentRepository? _documents;
    private IDocumentVersionRepository? _documentVersions;

    public UnitOfWork(FileSyncHubDbContext context)
    {
        _context = context;
    }

    public IDocumentRepository Documents => _documents ??= new DocumentRepository(_context);
    public IDocumentVersionRepository DocumentVersions => _documentVersions ??= new DocumentVersionRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
