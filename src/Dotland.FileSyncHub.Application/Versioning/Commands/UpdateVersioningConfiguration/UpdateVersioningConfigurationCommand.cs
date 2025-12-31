using Dotland.FileSyncHub.Application.Versioning.DTOs;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.UpdateVersioningConfiguration;

/// <summary>
/// Command to update versioning configuration for an organization.
/// </summary>
public class UpdateVersioningConfigurationCommand : IRequest<OrganizationVersioningConfigurationDto>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public required string OrganizationId { get; init; }

    /// <summary>
    /// Update DTO.
    /// </summary>
    public required UpdateOrganizationVersioningConfigurationDto Dto { get; init; }
}
