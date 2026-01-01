using Dotland.FileSyncHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Common.Services;

public interface IApplicationDbContext
{
    DbSet<OrganizationVersioningConfiguration> OrganizationVersioningConfigurations { get; }
    DbSet<CategoryVersioningConfiguration> CategoryVersioningConfigurations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}