using Dotland.FileSyncHub.Application.Common.Services;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentVersions;

/// <summary>
/// Handler for GetDocumentVersionsQuery.
/// </summary>
public class GetDocumentVersionsQueryHandler(IS3StorageService storageService)
    : IRequestHandler<GetDocumentVersionsQuery, GetDocumentVersionsResult>
{
    public Task<GetDocumentVersionsResult> Handle(GetDocumentVersionsQuery request, CancellationToken cancellationToken)
    {
        // Get all versions
        // var versions = await storageService.GetDocumentVersionsAsync(
        //     request.OrganizationId,
        //     request.Category,
        //     request.DocumentId,
        //     cancellationToken);
        //
        // // Check if versioning is enabled
        // var versioningEnabled = await storageService.IsVersioningEnabledAsync(
        //     request.OrganizationId,
        //     request.Category,
        //     cancellationToken);
        //
        // return new GetDocumentVersionsResult
        // {
        //     DocumentId = request.DocumentId,
        //     VersioningEnabled = versioningEnabled,
        //     Versions = versions,
        //     Count = versions.Count
        // };
        throw new NotImplementedException();
    }
}
