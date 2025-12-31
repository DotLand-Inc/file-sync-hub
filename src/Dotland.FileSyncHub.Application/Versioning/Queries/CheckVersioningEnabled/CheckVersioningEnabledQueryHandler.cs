using Dotland.FileSyncHub.Application.Versioning;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.CheckVersioningEnabled;

/// <summary>
/// Handler for CheckVersioningEnabledQuery.
/// </summary>
public class CheckVersioningEnabledQueryHandler : IRequestHandler<CheckVersioningEnabledQuery, CheckVersioningEnabledResult>
{
    private readonly IVersioningService _versioningService;

    public CheckVersioningEnabledQueryHandler(IVersioningService versioningService)
    {
        _versioningService = versioningService;
    }

    public async Task<CheckVersioningEnabledResult> Handle(CheckVersioningEnabledQuery request, CancellationToken cancellationToken)
    {
        var isEnabled = await _versioningService.IsVersioningEnabledAsync(request.OrganizationId, request.Category, cancellationToken);

        return new CheckVersioningEnabledResult
        {
            IsEnabled = isEnabled
        };
    }
}
