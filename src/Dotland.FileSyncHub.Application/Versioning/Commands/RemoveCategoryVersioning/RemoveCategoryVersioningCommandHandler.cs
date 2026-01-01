using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Common.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.RemoveCategoryVersioning;

/// <summary>
/// Handler for RemoveCategoryVersioningCommand.
/// </summary>
public class RemoveCategoryVersioningCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<RemoveCategoryVersioningCommand, Unit>
{
    public async Task<Unit> Handle(RemoveCategoryVersioningCommand request, CancellationToken cancellationToken)
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

            var category = config.CategoryConfigurations.SingleOrDefault(e => 
                e.Category == request.Category);

            if (category != null)
            {
                dbContext.CategoryVersioningConfigurations.Remove(category);
                config.SetUpdated("admin");
                
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            
            return Unit.Value;
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("VersioningConfiguration", request.OrganizationId);
        }
    }
}
