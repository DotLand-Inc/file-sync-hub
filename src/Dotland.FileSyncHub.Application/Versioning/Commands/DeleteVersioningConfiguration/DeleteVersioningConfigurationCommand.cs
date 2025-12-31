using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.DeleteVersioningConfiguration;

/// <summary>
/// Command to delete (deactivate) versioning configuration for an organization.
/// </summary>
public class DeleteVersioningConfigurationCommand : IRequest<Unit>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public required string OrganizationId { get; init; }
}
