using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Versioning;
using Dotland.FileSyncHub.Application.Versioning.DTOs;
using MediatR;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.CreateVersioningConfiguration;

/// <summary>
/// Handler for CreateVersioningConfigurationCommand.
/// </summary>
public class CreateVersioningConfigurationCommandHandler : IRequestHandler<CreateVersioningConfigurationCommand, OrganizationVersioningConfigurationDto>
{
    private readonly IVersioningService _versioningService;

    public CreateVersioningConfigurationCommandHandler(IVersioningService versioningService)
    {
        _versioningService = versioningService;
    }

    public async Task<OrganizationVersioningConfigurationDto> Handle(CreateVersioningConfigurationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _versioningService.CreateOrganizationConfigurationAsync(request.Dto, null, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("Configuration", ex.Message) });
        }
    }
}
