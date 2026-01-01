using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for FileSyncHub
/// </summary>
public class FileSyncHubDbContext(DbContextOptions<FileSyncHubDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<DocumentStatusHistory> DocumentStatusHistory => Set<DocumentStatusHistory>();
    public DbSet<OrganizationVersioningConfiguration> OrganizationVersioningConfigurations => Set<OrganizationVersioningConfiguration>();
    public DbSet<CategoryVersioningConfiguration> CategoryVersioningConfigurations => Set<CategoryVersioningConfiguration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileSyncHubDbContext).Assembly);
    }
}
