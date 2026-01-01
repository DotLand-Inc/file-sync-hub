using Dotland.FileSyncHub.Application.Documents.Commands.UploadDocumentVersion;
using Dotland.FileSyncHub.Domain.Enums;
using Dotland.FileSyncHub.Web.Extensions;

namespace Dotland.FileSyncHub.Web.Models.Requests;

/// <summary>
/// Request model for uploading a new version of a document.
/// </summary>
public class UploadDocumentVersionRequest
{
    public IFormFile File { get; set; } = null!;
    public string OrganizationId { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public DocumentCategory Category { get; set; }
    public string? Description { get; set; }

    /// <summary>
    /// Converts the request to an UploadDocumentVersionCommand.
    /// </summary>
    public async Task<UploadDocumentVersionCommand> ToCommandAsync(CancellationToken cancellationToken)
    {
        var fileContent = await File.ToByteArrayAsync(cancellationToken);

        return new UploadDocumentVersionCommand
        {
            FileContent = fileContent,
            FileName = File?.FileName ?? string.Empty,
            OrganizationId = OrganizationId,
            DocumentId = DocumentId,
            Category = Category,
            ContentType = File?.ContentType,
            Description = Description
        };
    }
}
