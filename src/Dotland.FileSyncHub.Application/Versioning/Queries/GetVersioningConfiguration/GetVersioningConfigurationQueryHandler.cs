using Dotland.FileSyncHub.Application.Versioning;
using Dotland.FileSyncHub.Application.Versioning.DTOs;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetVersioningConfiguration;

/// <summary>
/// Handler for GetVersioningConfigurationQuery.
/// </summary>
public class GetVersioningConfigurationQueryHandler : IRequestHandler<GetVersioningConfigurationQuery, OrganizationVersioningConfigurationDto?>
{
    private readonly IVersioningService _versioningService;

    public GetVersioningConfigurationQueryHandler(IVersioningService versioningService)
    {
        _versioningService = versioningService;
    }

    public async Task<OrganizationVersioningConfigurationDto?> Handle(GetVersioningConfigurationQuery request, CancellationToken cancellationToken)
    {
        return await _versioningService.GetOrganizationConfigurationAsync(request.OrganizationId, cancellationToken);
    }
}
