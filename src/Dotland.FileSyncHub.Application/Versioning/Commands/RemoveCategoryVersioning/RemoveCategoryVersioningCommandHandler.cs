using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Versioning;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.RemoveCategoryVersioning;

/// <summary>
/// Handler for RemoveCategoryVersioningCommand.
/// </summary>
public class RemoveCategoryVersioningCommandHandler : IRequestHandler<RemoveCategoryVersioningCommand, Unit>
{
    private readonly IVersioningService _versioningService;

    public RemoveCategoryVersioningCommandHandler(IVersioningService versioningService)
    {
        _versioningService = versioningService;
    }

    public async Task<Unit> Handle(RemoveCategoryVersioningCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _versioningService.RemoveCategoryConfigurationAsync(request.OrganizationId, request.Category, null, cancellationToken);
            return Unit.Value;
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("VersioningConfiguration", request.OrganizationId);
        }
    }
}
