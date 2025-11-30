using Dotland.FileSyncHub.Domain.Enums;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentVersions;

/// <summary>
/// Query to get all versions of a document.
/// </summary>
public class GetDocumentVersionsQuery : IRequest<GetDocumentVersionsResult>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public required string OrganizationId { get; init; }

    /// <summary>
    /// Document identifier.
    /// </summary>
    public required string DocumentId { get; init; }

    /// <summary>
    /// Document category.
    /// </summary>
    public required DocumentCategory Category { get; init; }
}
