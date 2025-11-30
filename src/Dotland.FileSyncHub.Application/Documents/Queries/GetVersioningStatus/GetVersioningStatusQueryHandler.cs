using Dotland.FileSyncHub.Application.Common.Services;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetVersioningStatus;

/// <summary>
/// Handler for GetVersioningStatusQuery.
/// </summary>
public class GetVersioningStatusQueryHandler : IRequestHandler<GetVersioningStatusQuery, bool>
{
    private readonly IS3StorageService _storageService;

    public GetVersioningStatusQueryHandler(IS3StorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<bool> Handle(GetVersioningStatusQuery request, CancellationToken cancellationToken)
    {
        return await _storageService.IsVersioningEnabledAsync(
            request.OrganizationId,
            request.Category,
            cancellationToken);
    }
}
