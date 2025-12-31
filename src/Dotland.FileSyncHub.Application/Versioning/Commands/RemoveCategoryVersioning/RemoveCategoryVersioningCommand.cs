using Dotland.FileSyncHub.Domain.Enums;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.RemoveCategoryVersioning;

/// <summary>
/// Command to remove category-specific versioning configuration.
/// </summary>
public class RemoveCategoryVersioningCommand : IRequest<Unit>
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
