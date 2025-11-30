using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Common.Services;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDownloadUrl;

/// <summary>
/// Handler for GetDownloadUrlQuery.
/// </summary>
public class GetDownloadUrlQueryHandler : IRequestHandler<GetDownloadUrlQuery, GetDownloadUrlResult>
{
    private readonly IS3StorageService _storageService;

    public GetDownloadUrlQueryHandler(IS3StorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<GetDownloadUrlResult> Handle(GetDownloadUrlQuery request, CancellationToken cancellationToken)
    {
        // Check if file exists
        var exists = await _storageService.FileExistsAsync(request.S3Key, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("File", request.S3Key);
        }

        // Generate presigned URL
        var url = _storageService.GeneratePresignedUrl(request.S3Key, request.ExpirationMinutes);

        return new GetDownloadUrlResult
        {
            DownloadUrl = url,
            ExpiresInMinutes = request.ExpirationMinutes
        };
    }
}
