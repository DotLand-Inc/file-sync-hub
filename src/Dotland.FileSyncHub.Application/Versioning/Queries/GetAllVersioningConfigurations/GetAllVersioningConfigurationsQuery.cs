using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetAllVersioningConfigurations;

/// <summary>
/// Query to get all versioning configurations.
/// </summary>
public class GetAllVersioningConfigurationsQuery : IRequest<GetAllVersioningConfigurationsResult>
{
}
