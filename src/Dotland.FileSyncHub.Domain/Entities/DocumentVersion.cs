using Dotland.FileSyncHub.Domain.Common;

namespace Dotland.FileSyncHub.Domain.Entities;

/// <summary>
/// Represents a version of a document stored in S3
/// </summary>
public class DocumentVersion : Entity
{
    public Guid DocumentId { get; private set; }
    public int VersionNumber { get; private set; }
    public string S3Key { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long FileSize { get; private set; }
    public string? Comment { get; private set; }
    public bool IsActive { get; private set; }

    public Document Document { get; private set; } = null!;

    private DocumentVersion() { }

    public static DocumentVersion Create(
        Guid documentId,
        int versionNumber,
        string s3Key,
        string fileName,
        string contentType,
        long fileSize,
        string? comment = null,
        string? createdBy = null)
    {
        return new DocumentVersion
        {
            DocumentId = documentId,
            VersionNumber = versionNumber,
            S3Key = s3Key,
            FileName = fileName,
            ContentType = contentType,
            FileSize = fileSize,
            Comment = comment,
            IsActive = true,
            CreatedBy = createdBy
        };
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdated();
    }
}
