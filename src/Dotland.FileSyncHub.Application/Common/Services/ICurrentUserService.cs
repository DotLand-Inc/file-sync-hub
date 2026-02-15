namespace Dotland.FileSyncHub.Application.Common.Services;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? OrganizationId { get; }
    // IEnumerable<string> Roles { get; } // Simple role list if needed
    bool IsAdmin { get; }
    bool IsHr { get; }
}
