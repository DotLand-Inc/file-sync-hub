using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Versioning;
using Dotland.FileSyncHub.Application.Versioning.DTOs;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.UpdateVersioningConfiguration;

/// <summary>
/// Handler for UpdateVersioningConfigurationCommand.
/// </summary>
public class UpdateVersioningConfigurationCommandHandler : IRequestHandler<UpdateVersioningConfigurationCommand, OrganizationVersioningConfigurationDto>
{
    private readonly IVersioningService _versioningService;

    public UpdateVersioningConfigurationCommandHandler(IVersioningService versioningService)
    {
        _versioningService = versioningService;
    }

    public async Task<OrganizationVersioningConfigurationDto> Handle(UpdateVersioningConfigurationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _versioningService.UpdateOrganizationConfigurationAsync(request.OrganizationId, request.Dto, null, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("VersioningConfiguration", request.OrganizationId);
        }
    }
}
