using Dotland.FileSyncHub.Domain.Enums;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetVersioningStatus;

/// <summary>
/// Query to check if versioning is enabled for a category in an organization.
/// </summary>
public class GetVersioningStatusQuery : IRequest<bool>
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
