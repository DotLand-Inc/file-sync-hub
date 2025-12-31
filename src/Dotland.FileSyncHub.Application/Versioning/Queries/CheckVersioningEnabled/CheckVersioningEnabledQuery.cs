using Dotland.FileSyncHub.Domain.Enums;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.CheckVersioningEnabled;

/// <summary>
/// Query to check if versioning is enabled for a specific organization and category.
/// </summary>
public class CheckVersioningEnabledQuery : IRequest<CheckVersioningEnabledResult>
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
