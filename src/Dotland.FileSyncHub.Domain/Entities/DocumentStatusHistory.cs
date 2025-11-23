using Dotland.FileSyncHub.Domain.Common;
using Dotland.FileSyncHub.Domain.Enums;

namespace Dotland.FileSyncHub.Domain.Entities;

/// <summary>
/// Tracks the history of document status changes
/// </summary>
public class DocumentStatusHistory : Entity
{
    public Guid DocumentId { get; private set; }
    public DocumentStatus Status { get; private set; }
    public string Comment { get; private set; } = null!;
    public string? ChangedBy { get; private set; }

    public Document Document { get; private set; } = null!;

    private DocumentStatusHistory() { }

    public static DocumentStatusHistory Create(
        Guid documentId,
        DocumentStatus status,
        string comment,
        string? changedBy = null)
    {
        return new DocumentStatusHistory
        {
            DocumentId = documentId,
            Status = status,
            Comment = comment,
            ChangedBy = changedBy,
            CreatedBy = changedBy
        };
    }
}
