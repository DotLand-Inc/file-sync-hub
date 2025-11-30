using Dotland.FileSyncHub.Application.Common.Models;
using Dotland.FileSyncHub.Application.Common.Services;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Commands.UploadDocumentVersion;

/// <summary>
/// Handler for UploadDocumentVersionCommand.
/// </summary>
public class UploadDocumentVersionCommandHandler : IRequestHandler<UploadDocumentVersionCommand, UploadResult>
{
    private readonly IS3StorageService _storageService;

    public UploadDocumentVersionCommandHandler(IS3StorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<UploadResult> Handle(UploadDocumentVersionCommand request, CancellationToken cancellationToken)
    {
        // Prepare metadata
        var metadata = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            metadata["description"] = request.Description;
        }

        // Create stream from byte array
        using var stream = new MemoryStream(request.FileContent);

        // Upload new version
        var result = await _storageService.UploadFileAsync(
            stream,
            request.FileName,
            request.OrganizationId,
            request.Category,
            request.DocumentId,
            request.ContentType,
            metadata,
            cancellationToken);

        return result;
    }
}
