using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.DocumentDirectDownload;

/// <summary>
/// Query to download a document directly.
/// </summary>
public class DocumentDirectDownloadQuery : IRequest<DocumentDirectDownloadResult>
{
    /// <summary>
    /// S3 key of the document.
    /// </summary>
    public required string S3Key { get; init; }
}
