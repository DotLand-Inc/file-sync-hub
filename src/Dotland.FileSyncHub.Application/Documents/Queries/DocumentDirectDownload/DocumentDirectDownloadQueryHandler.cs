using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Common.Services;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.DocumentDirectDownload;

/// <summary>
/// Handler for DocumentDirectDownloadQuery.
/// </summary>
public class DocumentDirectDownloadQueryHandler : IRequestHandler<DocumentDirectDownloadQuery, DocumentDirectDownloadResult>
{
    private readonly IS3StorageService _storageService;

    public DocumentDirectDownloadQueryHandler(IS3StorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<DocumentDirectDownloadResult> Handle(DocumentDirectDownloadQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Download file content and metadata
            var (stream, metadata) = await _storageService.DownloadFileAsync(request.S3Key, cancellationToken);

            // Convert stream to byte array
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);
            var content = memoryStream.ToArray();

            // Extract original filename from metadata or use S3 key
            var filename = metadata.TryGetValue("x-amz-meta-original-filename", out var fn)
                ? fn
                : Path.GetFileName(request.S3Key);

            return new DocumentDirectDownloadResult
            {
                Content = content,
                FileName = filename,
                ContentType = "application/octet-stream"
            };
        }
        catch (FileNotFoundException)
        {
            throw new NotFoundException("File", request.S3Key);
        }
    }
}
