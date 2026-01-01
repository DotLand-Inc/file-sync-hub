using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Application.Versioning.DTOs;
using Dotland.FileSyncHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetAllVersioningConfigurations;

/// <summary>
/// Handler for GetAllVersioningConfigurationsQuery.
/// </summary>
public class GetAllVersioningConfigurationsQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAllVersioningConfigurationsQuery, GetAllVersioningConfigurationsResult>
{
    public async Task<GetAllVersioningConfigurationsResult> Handle(GetAllVersioningConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var configs = await applicationDbContext.OrganizationVersioningConfigurations
            .Include(c => c.CategoryConfigurations)
            .Where(c => c.IsActive)
            .ToListAsync(cancellationToken);
        
        return new GetAllVersioningConfigurationsResult
        {
            Configurations = configs
                .Select(MapToDto)
                .ToList()
        };
    }
    
    private static OrganizationVersioningConfigurationDto MapToDto(OrganizationVersioningConfiguration config)
    {
        return new OrganizationVersioningConfigurationDto(
            config.Id,
            config.OrganizationId,
            config.DefaultVersioningEnabled,
            config.DefaultMaxVersions,
            config.IsActive,
            config.CategoryConfigurations.Select(c => new CategoryVersioningConfigurationDto(
                c.Id,
                c.Category,
                c.VersioningEnabled,
                c.MaxVersions
            )).ToList()
        );
    }
}
