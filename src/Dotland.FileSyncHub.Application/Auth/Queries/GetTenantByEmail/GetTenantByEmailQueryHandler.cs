using Dotland.FileSyncHub.Application.Auth.Models;
using Dotland.FileSyncHub.Application.Common.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dotland.FileSyncHub.Application.Auth.Queries.GetTenantByEmail;

public class GetTenantByEmailQueryHandler : IRequestHandler<GetTenantByEmailQuery, TenantDiscoveryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public GetTenantByEmailQueryHandler(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<TenantDiscoveryDto> Handle(GetTenantByEmailQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
        {
            // Could throw exception or handle gracefully? 
            // For now let's assume valid email passed or controller validation?
            // Controller validated format.
        }

        var domain = request.Email.Split('@').Last().ToLowerInvariant();

        var tenants = await _context.Tenants.ToListAsync(cancellationToken);
        var tenant = tenants.FirstOrDefault(t => 
            t.AllowedDomains.Split(',', StringSplitOptions.RemoveEmptyEntries)
             .Select(d => d.Trim().ToLowerInvariant())
             .Contains(domain));

        if (tenant != null)
        {
            return new TenantDiscoveryDto
            {
                TenantId = tenant.Identifier,
                Name = tenant.Name,
                Authority = tenant.Authority,
                ClientId = tenant.ClientId,
                LogoUrl = tenant.LogoUrl
            };
        }

        // Return Default Config
        var defaultAuthority = _configuration["Keycloak:Authority"] ?? "https://login.dotland.fr/realms/dotsuite";
        var defaultClientId = _configuration["Keycloak:ClientId"] ?? "ged-frontend";

        return new TenantDiscoveryDto
        {
            TenantId = "default",
            Name = "Default Tenant",
            Authority = defaultAuthority,
            ClientId = defaultClientId,
            LogoUrl = null
        };
    }
}
