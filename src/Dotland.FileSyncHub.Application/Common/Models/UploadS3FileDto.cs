using Dotland.FileSyncHub.Domain.Enums;

namespace Dotland.FileSyncHub.Application.Common.Models;

public class UploadS3FileDto
{
    public required Stream FileStream { get; set; }
    public required string Filename { get; set; }

    public required string OrganizationId { get; set; }
    public DocumentCategory Category { get; set; }
    public string? DocumentId { get; set; }
    public bool VersioningEnabled { get; set; }
    public int MaxVersions { get; set; }
    public string? S3Key { get; set; }
    public string? ContentType { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}