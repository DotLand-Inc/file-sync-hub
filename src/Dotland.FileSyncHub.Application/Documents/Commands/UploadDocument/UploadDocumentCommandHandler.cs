using Dotland.FileSyncHub.Application.Common.Exceptions;
using Dotland.FileSyncHub.Application.Common.Models;
using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Documents.Commands.UploadDocument;

/// <summary>
/// Handler for UploadDocumentCommand.
/// </summary>
public class UploadDocumentCommandHandler(
    IS3StorageService storageService,
    IApplicationDbContext dbContext)
    : IRequestHandler<UploadDocumentCommand, UploadResult>
{
    public async Task<UploadResult> Handle(
        UploadDocumentCommand request,
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
            .SingleOrDefaultAsync(e => e.Category == request.Category,
                cancellationToken);
        
        using var stream = new MemoryStream(request.FileContent);
        
        var dto = new UploadS3FileDto()
        {
            FileStream = stream,
            Filename = request.FileName,
            OrganizationId = request.OrganizationId,
            Category = request.Category,
            ContentType = request.ContentType,
            MaxVersions = categoryConfiguration?.MaxVersions ?? organisationConfig.DefaultMaxVersions,
            VersioningEnabled = categoryConfiguration?.VersioningEnabled ?? organisationConfig.DefaultVersioningEnabled
        };
        
        var result = await storageService.UploadFileAsync(
            dto,
            cancellationToken);

        if (result.Success)
        {
            var document = Domain.Entities.Document.Create(
                dto.OrganizationId,
                result.Filename,
                result.Filename,
                result.ContentType,
                result.SizeBytes,
                dto.Category,
                result.S3Key,
                request.Description,
                "admin"
            );
            
            await dbContext.Documents.AddAsync(document, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var documentVersion = Domain.Entities.DocumentVersion.Create(
                document.Id,
                result.Version,
                result.S3Key,
                result.Filename,
                result.ContentType,
                result.SizeBytes,
                request.Description,
                "admin");
            
            await dbContext.DocumentVersions.AddAsync(documentVersion, cancellationToken);

            var history = Domain.Entities.DocumentStatusHistory.Create(
                document.Id,
                DocumentStatus.Published,
                "Document Published",
                "admin");
            
            await dbContext.DocumentStatusHistory.AddAsync(history, cancellationToken);
            
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}
