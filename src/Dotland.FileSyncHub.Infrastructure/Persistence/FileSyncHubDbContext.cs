using Dotland.FileSyncHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for FileSyncHub
/// </summary>
public class FileSyncHubDbContext : DbContext
{
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<DocumentStatusHistory> DocumentStatusHistory => Set<DocumentStatusHistory>();
    public DbSet<OrganizationVersioningConfiguration> OrganizationVersioningConfigurations => Set<OrganizationVersioningConfiguration>();
    public DbSet<CategoryVersioningConfiguration> CategoryVersioningConfigurations => Set<CategoryVersioningConfiguration>();

    public FileSyncHubDbContext(DbContextOptions<FileSyncHubDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileSyncHubDbContext).Assembly);
    }
}
