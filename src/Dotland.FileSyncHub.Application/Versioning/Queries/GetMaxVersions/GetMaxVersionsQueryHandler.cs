using Dotland.FileSyncHub.Application.Versioning;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetMaxVersions;

/// <summary>
/// Handler for GetMaxVersionsQuery.
/// </summary>
public class GetMaxVersionsQueryHandler : IRequestHandler<GetMaxVersionsQuery, GetMaxVersionsResult>
{
    private readonly IVersioningService _versioningService;

    public GetMaxVersionsQueryHandler(IVersioningService versioningService)
    {
        _versioningService = versioningService;
    }

    public async Task<GetMaxVersionsResult> Handle(GetMaxVersionsQuery request, CancellationToken cancellationToken)
    {
        var maxVersions = await _versioningService.GetMaxVersionsAsync(request.OrganizationId, request.Category, cancellationToken);

        return new GetMaxVersionsResult
        {
            MaxVersions = maxVersions
        };
    }
}
