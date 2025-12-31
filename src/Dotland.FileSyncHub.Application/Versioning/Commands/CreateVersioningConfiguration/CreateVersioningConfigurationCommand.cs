using Dotland.FileSyncHub.Application.Versioning.DTOs;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.CreateVersioningConfiguration;

/// <summary>
/// Command to create versioning configuration for an organization.
/// </summary>
public class CreateVersioningConfigurationCommand : IRequest<OrganizationVersioningConfigurationDto>
{
    /// <summary>
    /// Creation DTO.
    /// </summary>
    public required CreateOrganizationVersioningConfigurationDto Dto { get; init; }
}
