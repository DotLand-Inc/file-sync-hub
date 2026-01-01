using Dotland.FileSyncHub.Domain.Common;

namespace Dotland.FileSyncHub.Domain.Entities;

/// <summary>
/// Represents a relationship between two documents where the source document has the target document as an ancestor
/// </summary>
public class DocumentRelation : Entity
{
    public Guid SourceDocumentId { get; private set; }
    public Guid TargetDocumentId { get; private set; }
    public string? Description { get; private set; }

    public Document SourceDocument { get; private set; } = null!;
    public Document TargetDocument { get; private set; } = null!;

    private DocumentRelation() { }

    public static DocumentRelation Create(
        Guid sourceDocumentId,
        Guid targetDocumentId,
        string? description = null,
        string? createdBy = null)
    {
        return new DocumentRelation
        {
            SourceDocumentId = sourceDocumentId,
            TargetDocumentId = targetDocumentId,
            Description = description,
            CreatedBy = createdBy
        };
    }

    public void UpdateDescription(string? description, string? updatedBy = null)
    {
        Description = description;
        SetUpdated(updatedBy);
    }
}
