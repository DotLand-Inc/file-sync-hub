using Dotland.FileSyncHub.Application.Auth.Models;
using MediatR;

namespace Dotland.FileSyncHub.Application.Auth.Queries.GetTenantByEmail;

public class GetTenantByEmailQuery : IRequest<TenantDiscoveryDto>
{
    public string Email { get; set; } = string.Empty;
}
