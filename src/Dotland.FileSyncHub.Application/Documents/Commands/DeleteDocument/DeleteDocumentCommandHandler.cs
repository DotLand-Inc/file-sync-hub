using Dotland.FileSyncHub.Application.Common.Services;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Commands.DeleteDocument;

/// <summary>
/// Handler for DeleteDocumentCommand.
/// </summary>
public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Unit>
{
    private readonly IS3StorageService _storageService;

    public DeleteDocumentCommandHandler(IS3StorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        await _storageService.DeleteFileAsync(request.S3Key, cancellationToken);

        return Unit.Value;
    }
}
