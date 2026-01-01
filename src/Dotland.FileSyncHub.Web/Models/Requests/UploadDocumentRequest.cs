using Dotland.FileSyncHub.Application.Documents.Commands.UploadDocument;
using Dotland.FileSyncHub.Domain.Enums;
using Dotland.FileSyncHub.Web.Extensions;

namespace Dotland.FileSyncHub.Web.Models.Requests;

/// <summary>
/// Request model for uploading a document.
/// </summary>
public class UploadDocumentRequest
{
    public IFormFile File { get; set; } = null!;
    public string OrganizationId { get; set; } = string.Empty;
    public DocumentCategory Category { get; set; } = DocumentCategory.Other;
    public string? Description { get; set; }

    /// <summary>
    /// Converts the request to an UploadDocumentCommand.
    /// </summary>
    public async Task<UploadDocumentCommand> ToCommandAsync(CancellationToken cancellationToken)
    {
        var fileContent = await File.ToByteArrayAsync(cancellationToken);

        return new UploadDocumentCommand
        {
            FileContent = fileContent,
            FileName = File?.FileName ?? string.Empty,
            OrganizationId = OrganizationId,
            Category = Category,
            ContentType = File?.ContentType,
            Description = Description
        };
    }
}

