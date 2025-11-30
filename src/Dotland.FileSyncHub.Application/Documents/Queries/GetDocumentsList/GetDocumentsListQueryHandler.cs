using Dotland.FileSyncHub.Application.Common.Services;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentsList;

/// <summary>
/// Handler for GetDocumentsListQuery.
/// </summary>
public class GetDocumentsListQueryHandler : IRequestHandler<GetDocumentsListQuery, GetDocumentsListResult>
{
    private readonly IS3StorageService _storageService;

    public GetDocumentsListQueryHandler(IS3StorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<GetDocumentsListResult> Handle(GetDocumentsListQuery request, CancellationToken cancellationToken)
    {
        var files = await _storageService.ListFilesAsync(
            request.OrganizationId,
            request.Category,
            cancellationToken: cancellationToken);

        return new GetDocumentsListResult
        {
            Files = files,
            Count = files.Count
        };
    }
}
