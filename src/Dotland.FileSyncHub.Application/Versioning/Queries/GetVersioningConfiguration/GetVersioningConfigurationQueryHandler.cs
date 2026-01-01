using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Application.Versioning.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetVersioningConfiguration;

/// <summary>
/// Handler for GetVersioningConfigurationQuery.
/// </summary>
public class GetVersioningConfigurationQueryHandler(IVersioningService versioningService, IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetVersioningConfigurationQuery, OrganizationVersioningConfigurationDto?>
{
    public async Task<OrganizationVersioningConfigurationDto?> Handle(GetVersioningConfigurationQuery request, CancellationToken cancellationToken)
    {
        var config = await applicationDbContext.OrganizationVersioningConfigurations
            .Include(c => c.CategoryConfigurations)
            .FirstOrDefaultAsync(c => c.OrganizationId == request.OrganizationId && c.IsActive, cancellationToken);
        
        return config?.MapToOrganizationVersioningConfigurationDto();
    }
}
