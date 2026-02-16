namespace Dotland.FileSyncHub.Application.Auth.Models;

public class TenantDiscoveryDto
{
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}
