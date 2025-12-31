using Dotland.FileSyncHub.Application.Versioning;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetAllVersioningConfigurations;

/// <summary>
/// Handler for GetAllVersioningConfigurationsQuery.
/// </summary>
public class GetAllVersioningConfigurationsQueryHandler : IRequestHandler<GetAllVersioningConfigurationsQuery, GetAllVersioningConfigurationsResult>
{
    private readonly IVersioningService _versioningService;

    public GetAllVersioningConfigurationsQueryHandler(IVersioningService versioningService)
    {
        _versioningService = versioningService;
    }

    public async Task<GetAllVersioningConfigurationsResult> Handle(GetAllVersioningConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var configurations = await _versioningService.GetAllConfigurationsAsync(cancellationToken);

        return new GetAllVersioningConfigurationsResult
        {
            Configurations = configurations
        };
    }
}
