using Dotland.FileSyncHub.Application.Common.Models;
using Dotland.FileSyncHub.Domain.Enums;
using MediatR;

namespace Dotland.FileSyncHub.Application.Documents.Commands.UploadDocument;

/// <summary>
/// Command to upload a document to S3 storage.
/// </summary>
public class UploadDocumentCommand : IRequest<UploadResult>
{
    /// <summary>
    /// File content as byte array.
    /// </summary>
    public required byte[] FileContent { get; init; }

    /// <summary>
    /// Original filename.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public required string OrganizationId { get; init; }

    /// <summary>
    /// Document category.
    /// </summary>
    public DocumentCategory Category { get; init; } = DocumentCategory.Other;

    /// <summary>
    /// MIME content type.
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Optional description metadata.
    /// </summary>
    public string? Description { get; init; }
}
