using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Application.Versioning;
using Dotland.FileSyncHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.SetCategoryVersioning;

/// <summary>
/// Handler for SetCategoryVersioningCommand.
/// </summary>
public class SetCategoryVersioningCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<SetCategoryVersioningCommand, Unit>
{
    public async Task<Unit> Handle(
        SetCategoryVersioningCommand request,
        CancellationToken cancellationToken)
    {
        var organisationConfig = await dbContext
            .OrganizationVersioningConfigurations
            .SingleOrDefaultAsync(
                e =>
                    e.OrganizationId  == request.OrganizationId,
                cancellationToken);
        
        if (organisationConfig == null)
        {
            throw new NotFoundException("VersioningConfiguration", request.OrganizationId);
        }
        
        var categoryConfiguration = await dbContext.CategoryVersioningConfigurations
            .SingleOrDefaultAsync(e => e.Category == request.Dto.Category &&
                                       e.OrganizationVersioningConfigurationId == organisationConfig.Id,
                cancellationToken);

        if (categoryConfiguration == null)
        {
            categoryConfiguration = CategoryVersioningConfiguration.Create(
                organisationConfig.Id,
                request.Dto.Category,
                request.Dto.VersioningEnabled,
                request.Dto.MaxVersions,
                "admin");
            dbContext.CategoryVersioningConfigurations.Add(categoryConfiguration);
        }
        else
        {
            categoryConfiguration.Update(request.Dto.VersioningEnabled,
                request.Dto.MaxVersions,
                "admin");
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}
