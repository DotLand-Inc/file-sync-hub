using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDownloadUrl;

/// <summary>
/// Query to get a presigned download URL for a document.
/// </summary>
public class GetDownloadUrlQuery : IRequest<GetDownloadUrlResult>
{
    /// <summary>
    /// S3 key of the document.
    /// </summary>
    public required string S3Key { get; init; }

    /// <summary>
    /// URL expiration time in minutes.
    /// </summary>
    public int ExpirationMinutes { get; init; } = 60;
}
