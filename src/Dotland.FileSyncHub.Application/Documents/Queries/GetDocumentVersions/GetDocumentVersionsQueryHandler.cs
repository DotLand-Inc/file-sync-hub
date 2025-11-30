using Dotland.FileSyncHub.Application.Common.Services;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentVersions;

/// <summary>
/// Handler for GetDocumentVersionsQuery.
/// </summary>
public class GetDocumentVersionsQueryHandler : IRequestHandler<GetDocumentVersionsQuery, GetDocumentVersionsResult>
{
    private readonly IS3StorageService _storageService;

    public GetDocumentVersionsQueryHandler(IS3StorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<GetDocumentVersionsResult> Handle(GetDocumentVersionsQuery request, CancellationToken cancellationToken)
    {
        // Get all versions
        var versions = await _storageService.GetDocumentVersionsAsync(
            request.OrganizationId,
            request.Category,
            request.DocumentId,
            cancellationToken);

        // Check if versioning is enabled
        var versioningEnabled = await _storageService.IsVersioningEnabledAsync(
            request.OrganizationId,
            request.Category,
            cancellationToken);

        return new GetDocumentVersionsResult
        {
            DocumentId = request.DocumentId,
            VersioningEnabled = versioningEnabled,
            Versions = versions,
            Count = versions.Count
        };
    }
}
