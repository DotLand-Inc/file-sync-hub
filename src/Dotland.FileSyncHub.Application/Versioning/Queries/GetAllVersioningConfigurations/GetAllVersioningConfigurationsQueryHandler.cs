using Dotland.FileSyncHub.Application.Common.Services;
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
                .Select(Converters.MapToOrganizationVersioningConfigurationDto)
                .ToList()
        };
    }
    
    
}
