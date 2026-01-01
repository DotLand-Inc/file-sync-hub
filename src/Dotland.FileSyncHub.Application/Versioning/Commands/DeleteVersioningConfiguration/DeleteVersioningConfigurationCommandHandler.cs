using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Application.Versioning;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.DeleteVersioningConfiguration;

/// <summary>
/// Handler for DeleteVersioningConfigurationCommand.
/// </summary>
public class DeleteVersioningConfigurationCommandHandler(IVersioningService versioningService, IApplicationDbContext dbContext)
    : IRequestHandler<DeleteVersioningConfigurationCommand, Unit>
{
    public async Task<Unit> Handle(DeleteVersioningConfigurationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var config = await dbContext.OrganizationVersioningConfigurations
                .Include(c => c.CategoryConfigurations)
                .SingleOrDefaultAsync(c => c.OrganizationId == request.OrganizationId &&
                                           c.IsActive, cancellationToken);
            
            if (config == null)
                throw new InvalidOperationException(
                    $"Configuration not found for organization {request.OrganizationId}");
            
            dbContext.CategoryVersioningConfigurations.RemoveRange(config.CategoryConfigurations);
            dbContext.OrganizationVersioningConfigurations.Remove(config);
            
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("VersioningConfiguration", request.OrganizationId);
        }
    }
}
