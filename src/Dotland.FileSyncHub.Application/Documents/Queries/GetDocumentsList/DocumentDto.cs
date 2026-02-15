using Dotland.FileSyncHub.Domain.Enums;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentsList;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public int Version { get; set; }
    public List<string> Tags { get; set; } = new();
}
