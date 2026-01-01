using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Application.Versioning.DTOs;
using Dotland.FileSyncHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.CreateVersioningConfiguration;

/// <summary>
/// Handler for CreateVersioningConfigurationCommand.
/// </summary>
public class CreateVersioningConfigurationCommandHandler(IApplicationDbContext  dbContext)
    : IRequestHandler<CreateVersioningConfigurationCommand, OrganizationVersioningConfigurationDto>
{
    public async Task<OrganizationVersioningConfigurationDto> Handle(CreateVersioningConfigurationCommand request, CancellationToken cancellationToken)
    { 
        try
        {
            var existingConfig = await dbContext.OrganizationVersioningConfigurations
                    .SingleOrDefaultAsync(c => 
                        c.OrganizationId == request.Dto.OrganizationId &&
                        c.IsActive,
                        cancellationToken);
            
            if (existingConfig != null)
                throw new InvalidOperationException(
                    $"Configuration already exists for organization {request.Dto.OrganizationId}");
            
            var config = OrganizationVersioningConfiguration.Create(
                request.Dto.OrganizationId,
                request.Dto.DefaultVersioningEnabled,
                request.Dto.DefaultMaxVersions,
                "admin"
            );
            
            await dbContext.OrganizationVersioningConfigurations.AddAsync(config, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);


            return config.MapToOrganizationVersioningConfigurationDto();
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure("Configuration", ex.Message)]);
        }
    }
}
