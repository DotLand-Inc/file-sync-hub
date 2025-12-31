using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Versioning;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.DeleteVersioningConfiguration;

/// <summary>
/// Handler for DeleteVersioningConfigurationCommand.
/// </summary>
public class DeleteVersioningConfigurationCommandHandler : IRequestHandler<DeleteVersioningConfigurationCommand, Unit>
{
    private readonly IVersioningService _versioningService;

    public DeleteVersioningConfigurationCommandHandler(IVersioningService versioningService)
    {
        _versioningService = versioningService;
    }

    public async Task<Unit> Handle(DeleteVersioningConfigurationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _versioningService.DeleteOrganizationConfigurationAsync(request.OrganizationId, cancellationToken);
            return Unit.Value;
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("VersioningConfiguration", request.OrganizationId);
        }
    }
}
