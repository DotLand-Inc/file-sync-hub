using System.Security.Claims;
using System.Text.Json;
using Dotland.FileSyncHub.Application.Common.Services;

namespace Dotland.FileSyncHub.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                             _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? 
                            _httpContextAccessor.HttpContext?.User?.FindFirstValue("email");

    // Assuming organization ID might be in the claims or URL, for now returning null or implementing if known claim exists
    // If org ID is passed as route param, use that in Controller.
    public string? OrganizationId => null; 

    public bool IsAdmin => HasRole("edm_admin");
    public bool IsHr => HasRole("edm_hr");

    private bool HasRole(string role)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null) return false;

        // 1. Check strict Realm Access or Resource Access roles from Keycloak
        if (user.HasClaim(c => c.Type == "resource_access"))
        {
            var resourceAccessClaim = user.FindFirst("resource_access")?.Value;
            if (!string.IsNullOrEmpty(resourceAccessClaim))
            {
                try 
                {
                    using var doc = JsonDocument.Parse(resourceAccessClaim);
                    if (doc.RootElement.TryGetProperty("ged-frontend", out var clientApp))
                    {
                        if (clientApp.TryGetProperty("roles", out var rolesElement))
                        {
                            foreach (var r in rolesElement.EnumerateArray())
                            {
                                if (r.GetString() == role) return true;
                            }
                        }
                    }
                }
                catch 
                {
                    // Log error parsing JSON if needed
                }
            }
        }
        
        // 2. Check Realm Access
         if (user.HasClaim(c => c.Type == "realm_access"))
        {
            var realmAccessClaim = user.FindFirst("realm_access")?.Value;
             if (!string.IsNullOrEmpty(realmAccessClaim))
            {
                 try 
                {
                    using var doc = JsonDocument.Parse(realmAccessClaim);
                    if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                    {
                        foreach (var r in rolesElement.EnumerateArray())
                        {
                            if (r.GetString() == role) return true;
                        }
                    }
                }
                catch 
                {
                    // Log error
                }
            }
        }

        // 3. Check standard ClaimTypes.Role (if mapped by middleware)
        return user.IsInRole(role);
    }
}
