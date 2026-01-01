using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Common.Models;
using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Documents.Commands.UploadDocumentVersion;

/// <summary>
/// Handler for UploadDocumentVersionCommand.
/// </summary>
public class UploadDocumentVersionCommandHandler(IS3StorageService storageService, IApplicationDbContext dbContext)
    : IRequestHandler<UploadDocumentVersionCommand, UploadResult>
{
    public async Task<UploadResult> Handle(UploadDocumentVersionCommand request, CancellationToken cancellationToken)
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
            .SingleOrDefaultAsync(e => e.Category == request.Category,
                cancellationToken);
        
        var document = await dbContext.Documents.SingleOrDefaultAsync(e =>
            e.Id == Guid.Parse(request.DocumentId),
            cancellationToken);
        
        if (document == null)
        {
            throw new NotFoundException("Documents", request.DocumentId);
        }


        using var stream = new MemoryStream(request.FileContent);
        
        var dto = new UploadS3FileDto()
        {
            FileStream = stream,
            Filename = request.FileName,
            OrganizationId = request.OrganizationId,
            Category = request.Category,
            ContentType = request.ContentType,
            DocumentId =  request.DocumentId,
            MaxVersions = categoryConfiguration?.MaxVersions ?? organisationConfig.DefaultMaxVersions,
            VersioningEnabled = categoryConfiguration?.VersioningEnabled ?? organisationConfig.DefaultVersioningEnabled,
            S3Key = document.S3Key
        };

        // Upload new version
        var result = await storageService.UploadFileAsync(
            dto,
            cancellationToken);

        if (result.Success)
        {
            var version = dbContext.DocumentVersions.Where(e => e.DocumentId == document.Id)
                .Max(e => e.VersionNumber);
            
            result.Version = version;
            
            var documentVersion = Domain.Entities.DocumentVersion.Create(
                document.Id,
                version + 1,
                result.S3VersionId,
                result.Filename,
                result.ContentType,
                result.SizeBytes,
                request.Description,
                "admin");
            
            await dbContext.DocumentVersions.AddAsync(documentVersion, cancellationToken);

            var history = Domain.Entities.DocumentStatusHistory.Create(
                document.Id,
                DocumentStatus.Reviewed,
                "Document Reviewed",
                "admin");
            
            await dbContext.DocumentStatusHistory.AddAsync(history, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        return result;
    }
}
