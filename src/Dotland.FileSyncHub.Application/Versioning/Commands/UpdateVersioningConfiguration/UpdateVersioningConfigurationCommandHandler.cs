using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Application.Versioning.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.UpdateVersioningConfiguration;

/// <summary>
/// Handler for UpdateVersioningConfigurationCommand.
/// </summary>
public class UpdateVersioningConfigurationCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<UpdateVersioningConfigurationCommand, OrganizationVersioningConfigurationDto>
{
    public async Task<OrganizationVersioningConfigurationDto> Handle(UpdateVersioningConfigurationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var config = await applicationDbContext.OrganizationVersioningConfigurations
                .SingleOrDefaultAsync(c => 
                        c.OrganizationId == request.OrganizationId &&
                        c.IsActive,
                    cancellationToken);
            

            if (config == null)
                throw new InvalidOperationException($"Configuration not found for organization {request.OrganizationId}");

            config.DefaultVersioningEnabled = request.Dto.DefaultVersioningEnabled;
            config.DefaultMaxVersions = request.Dto.DefaultMaxVersions;
            config.SetUpdated("admin");

            applicationDbContext.OrganizationVersioningConfigurations.Update(config);
            
            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return config.MapToOrganizationVersioningConfigurationDto();
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("VersioningConfiguration", request.OrganizationId);
        }
    }
}
