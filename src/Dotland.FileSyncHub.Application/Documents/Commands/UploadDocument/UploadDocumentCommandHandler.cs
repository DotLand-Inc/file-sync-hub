using Dotland.FileSyncHub.Application.Common.Models;
using Dotland.FileSyncHub.Application.Common.Services;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Commands.UploadDocument;

/// <summary>
/// Handler for UploadDocumentCommand.
/// </summary>
public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, UploadResult>
{
    private readonly IS3StorageService _storageService;

    public UploadDocumentCommandHandler(IS3StorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<UploadResult> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        // Prepare metadata
        var metadata = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            metadata["description"] = request.Description;
        }

        // Create stream from byte array
        using var stream = new MemoryStream(request.FileContent);

        // Upload file
        var result = await _storageService.UploadFileAsync(
            stream,
            request.FileName,
            request.OrganizationId,
            request.Category,
            request.ContentType,
            metadata,
            cancellationToken);

        return result;
    }
}
