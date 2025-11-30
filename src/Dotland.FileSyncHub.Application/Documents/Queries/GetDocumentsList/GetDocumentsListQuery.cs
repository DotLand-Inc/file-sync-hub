using Dotland.FileSyncHub.Domain.Enums;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentsList;

/// <summary>
/// Query to get list of documents for an organization.
/// </summary>
public class GetDocumentsListQuery : IRequest<GetDocumentsListResult>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public required string OrganizationId { get; init; }

    /// <summary>
    /// Optional document category filter.
    /// </summary>
    public DocumentCategory? Category { get; init; }
}
