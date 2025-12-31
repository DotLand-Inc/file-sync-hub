using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Versioning;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.SetCategoryVersioning;

/// <summary>
/// Handler for SetCategoryVersioningCommand.
/// </summary>
public class SetCategoryVersioningCommandHandler : IRequestHandler<SetCategoryVersioningCommand, Unit>
{
    private readonly IVersioningService _versioningService;

    public SetCategoryVersioningCommandHandler(IVersioningService versioningService)
    {
        _versioningService = versioningService;
    }

    public async Task<Unit> Handle(SetCategoryVersioningCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _versioningService.SetCategoryConfigurationAsync(request.OrganizationId, request.Dto, null, cancellationToken);
            return Unit.Value;
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("VersioningConfiguration", request.OrganizationId);
        }
    }
}
