using Dotland.FileSyncHub.Application.Versioning.DTOs;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.SetCategoryVersioning;

/// <summary>
/// Command to set category-specific versioning configuration.
/// </summary>
public class SetCategoryVersioningCommand : IRequest<Unit>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public required string OrganizationId { get; init; }

    /// <summary>
    /// Category configuration DTO.
    /// </summary>
    public required SetCategoryVersioningConfigurationDto Dto { get; init; }
}
