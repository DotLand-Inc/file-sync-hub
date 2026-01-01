using Dotland.FileSyncHub.Application.Versioning.Commands.CreateVersioningConfiguration;
using Dotland.FileSyncHub.Application.Versioning.Commands.DeleteVersioningConfiguration;
using Dotland.FileSyncHub.Application.Versioning.Commands.RemoveCategoryVersioning;
using Dotland.FileSyncHub.Application.Versioning.Commands.SetCategoryVersioning;
using Dotland.FileSyncHub.Application.Versioning.Commands.UpdateVersioningConfiguration;
using Dotland.FileSyncHub.Application.Versioning.DTOs;
using Dotland.FileSyncHub.Application.Versioning.Queries.CheckVersioningEnabled;
using Dotland.FileSyncHub.Application.Versioning.Queries.GetAllVersioningConfigurations;
using Dotland.FileSyncHub.Application.Versioning.Queries.GetMaxVersions;
using Dotland.FileSyncHub.Application.Versioning.Queries.GetVersioningConfiguration;
using Dotland.FileSyncHub.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dotland.FileSyncHub.Web.Controllers;

/// <summary>
/// API controller for managing versioning configurations
/// </summary>
[ApiController]
[Route("api/versioning")]
public class VersioningController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all versioning configurations
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GetAllVersioningConfigurationsResult>> GetAllVersioningConfigurationsAsync(
        CancellationToken cancellationToken)
    {
        var query = new GetAllVersioningConfigurationsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get versioning configuration for an organization
    /// </summary>
    [HttpGet("{organizationId}")]
    public async Task<ActionResult<OrganizationVersioningConfigurationDto>> GetVersioningConfigurationAsync(
        string organizationId, CancellationToken cancellationToken)
    {
        var query = new GetVersioningConfigurationQuery
        {
            OrganizationId = organizationId
        };

        var config = await mediator.Send(query, cancellationToken);

        if (config == null)
            return NotFound($"No versioning configuration found for organization {organizationId}");

        return Ok(config);
    }

    /// <summary>
    /// Create versioning configuration for an organization
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrganizationVersioningConfigurationDto>> PostAsync(
        [FromBody] CreateOrganizationVersioningConfigurationDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateVersioningConfigurationCommand
        {
            Dto = dto
        };

        var config = await mediator.Send(command, cancellationToken);
        return Ok(new { OrganizationId = config.OrganizationId, OriginalConfig = config });
    }

    /// <summary>
    /// Update versioning configuration for an organization
    /// </summary>
    [HttpPut("{organizationId}")]
    public async Task<ActionResult<OrganizationVersioningConfigurationDto>> UpdateVersioningConfigurationAsync(
        string organizationId,
        [FromBody] UpdateOrganizationVersioningConfigurationDto dto,
        CancellationToken cancellationToken)
    {
        var command = new UpdateVersioningConfigurationCommand
        {
            OrganizationId = organizationId,
            Dto = dto
        };

        var config = await mediator.Send(command, cancellationToken);
        return Ok(config);
    }

    /// <summary>
    /// Set category-specific versioning configuration
    /// </summary>
    [HttpPut("{organizationId}/categories")]
    public async Task<IActionResult> SetCategoryVersioningAsync(
        string organizationId,
        [FromBody] SetCategoryVersioningConfigurationDto dto,
        CancellationToken cancellationToken)
    {
        var command = new SetCategoryVersioningCommand
        {
            OrganizationId = organizationId,
            Dto = dto
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Remove category-specific versioning configuration
    /// </summary>
    [HttpDelete("{organizationId}/categories/{category}")]
    public async Task<IActionResult> RemoveCategoryVersioningAsync(
        string organizationId,
        DocumentCategory category,
        CancellationToken cancellationToken)
    {
        var command = new RemoveCategoryVersioningCommand
        {
            OrganizationId = organizationId,
            Category = category
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete (deactivate) versioning configuration for an organization
    /// </summary>
    [HttpDelete("{organizationId}")]
    public async Task<IActionResult> DeleteVersioningConfigurationAsync(string organizationId, CancellationToken cancellationToken)
    {
        var command = new DeleteVersioningConfigurationCommand
        {
            OrganizationId = organizationId
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
