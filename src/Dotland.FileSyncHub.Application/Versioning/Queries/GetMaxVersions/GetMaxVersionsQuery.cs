using Dotland.FileSyncHub.Domain.Enums;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetMaxVersions;

/// <summary>
/// Query to get max versions for a specific organization and category.
/// </summary>
public class GetMaxVersionsQuery : IRequest<GetMaxVersionsResult>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public required string OrganizationId { get; init; }

    /// <summary>
    /// Document category.
    /// </summary>
    public required DocumentCategory Category { get; init; }
}
