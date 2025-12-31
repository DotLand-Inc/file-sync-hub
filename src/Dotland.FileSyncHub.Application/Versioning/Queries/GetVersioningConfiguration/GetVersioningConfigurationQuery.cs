using Dotland.FileSyncHub.Application.Versioning.DTOs;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetVersioningConfiguration;

/// <summary>
/// Query to get versioning configuration for an organization.
/// </summary>
public class GetVersioningConfigurationQuery : IRequest<OrganizationVersioningConfigurationDto?>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public required string OrganizationId { get; init; }
}
