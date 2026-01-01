using Dotland.FileSyncHub.Domain.Common;
using Dotland.FileSyncHub.Domain.Enums;

namespace Dotland.FileSyncHub.Domain.Entities;

/// <summary>
/// Document entity representing a managed document in the system
/// </summary>
public class Document : Entity
{
    public string OrganizationId { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long FileSize { get; private set; }
    public DocumentCategory Category { get; private set; }
    public DocumentStatus Status { get; private set; }
    public int CurrentVersion { get; private set; }
    public string? S3Key { get; private set; }
    public string? WorkflowInstanceId { get; private set; }

    private readonly List<DocumentVersion> _versions = new();
    public IReadOnlyCollection<DocumentVersion> Versions => _versions.AsReadOnly();

    private readonly List<DocumentStatusHistory> _statusHistory = new();
    public IReadOnlyCollection<DocumentStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    private readonly List<DocumentRelation> _parentRelations = new();
    public IReadOnlyCollection<DocumentRelation> ParentRelations => _parentRelations.AsReadOnly();

    private readonly List<DocumentRelation> _childRelations = new();
    public IReadOnlyCollection<DocumentRelation> ChildRelations => _childRelations.AsReadOnly();

    private Document() { }

    public static Document Create(
        string organizationId,
        string title,
        string fileName,
        string contentType,
        long fileSize,
        DocumentCategory category,
        string s3Key,
        string? description = null,
        string? createdBy = null)
    {
        var document = new Document
        {
            OrganizationId = organizationId,
            Title = title,
            FileName = fileName,
            ContentType = contentType,
            FileSize = fileSize,
            Category = category,
            Description = description,
            Status = DocumentStatus.Draft,
            CurrentVersion = 1,
            CreatedBy = createdBy,
            S3Key = s3Key
        };

        return document;
    }

    public void SetS3Key(string s3Key)
    {
        S3Key = s3Key;
    }

    public void SetWorkflowInstanceId(string workflowInstanceId)
    {
        WorkflowInstanceId = workflowInstanceId;
    }

    public void UpdateStatus(DocumentStatus newStatus, string? comment = null, string? updatedBy = null)
    {
        if (Status == newStatus) return;

        var previousStatus = Status;
        Status = newStatus;
        SetUpdated(updatedBy);

        AddStatusHistory(newStatus, comment ?? $"Status changed from {previousStatus} to {newStatus}", updatedBy);
    }

    public void IncrementVersion()
    {
        CurrentVersion++;
        SetUpdated();
    }

    public void Update(string? title = null, string? description = null, DocumentCategory? category = null, string? updatedBy = null)
    {
        if (title != null) Title = title;
        if (description != null) Description = description;
        if (category.HasValue) Category = category.Value;
        SetUpdated(updatedBy);
    }

    public void AddVersion(DocumentVersion version)
    {
        _versions.Add(version);
    }

    private void AddStatusHistory(DocumentStatus status, string comment, string? changedBy)
    {
        _statusHistory.Add(DocumentStatusHistory.Create(Id, status, comment, changedBy));
    }
}
