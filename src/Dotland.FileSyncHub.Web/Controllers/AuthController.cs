using Dotland.FileSyncHub.Application.Auth.Models;
using Dotland.FileSyncHub.Application.Auth.Queries.GetTenantByEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dotland.FileSyncHub.Web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("discovery")]
    [AllowAnonymous]
    public async Task<ActionResult<TenantDiscoveryDto>> DiscoverTenant([FromBody] DiscoveryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
        {
            return BadRequest("Invalid email format.");
        }

        var result = await sender.Send(new GetTenantByEmailQuery { Email = request.Email });
        return Ok(result);
    }
}
