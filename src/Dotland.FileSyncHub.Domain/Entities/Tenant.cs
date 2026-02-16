using Dotland.FileSyncHub.Domain.Common;

namespace Dotland.FileSyncHub.Domain.Entities;

public class Tenant : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty; // e.g., "dotland", "acme"
    public string Authority { get; set; } = string.Empty; // Keycloak Realm URL
    public string ClientId { get; set; } = string.Empty;
    public string AllowedDomains { get; set; } = string.Empty; // Comma-separated list of domains: "dotland.fr,acme.com"

    // Optional: branding
    public string? LogoUrl { get; set; }
}
