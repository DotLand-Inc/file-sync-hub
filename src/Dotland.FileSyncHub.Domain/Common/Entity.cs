namespace Dotland.FileSyncHub.Domain.Common;

/// <summary>
/// Base entity class with common properties
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    protected void SetUpdated(string? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
