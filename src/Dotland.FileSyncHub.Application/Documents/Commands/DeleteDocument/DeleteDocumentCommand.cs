using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Commands.DeleteDocument;

/// <summary>
/// Command to delete a document.
/// </summary>
public class DeleteDocumentCommand : IRequest<Unit>
{
    /// <summary>
    /// S3 key of the document to delete.
    /// </summary>
    public required string S3Key { get; init; }
}
