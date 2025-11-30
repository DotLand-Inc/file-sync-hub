using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dotland.FileSyncHub.Application.Versioning;
using Dotland.FileSyncHub.Application.Versioning.DTOs;
using Dotland.FileSyncHub.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Dotland.FileSyncHub.Web.Controllers;

/// <summary>
/// API controller for managing versioning configurations
/// </summary>
[ApiController]
[Route("api/versioning")]
public class VersioningController : ControllerBase
{
    private readonly IVersioningService _versioningService;

    public VersioningController(IVersioningService versioningService)
    {
        _versioningService = versioningService;
    }

    /// <summary>
    /// Get all versioning configurations
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrganizationVersioningConfigurationDto>>> GetAll(CancellationToken cancellationToken)
    {
        var configs = await _versioningService.GetAllConfigurationsAsync(cancellationToken);
        return Ok(configs);
    }

    /// <summary>
    /// Get versioning configuration for an organization
    /// </summary>
    [HttpGet("{organizationId}")]
    public async Task<ActionResult<OrganizationVersioningConfigurationDto>> GetByOrganization(string organizationId, CancellationToken cancellationToken)
    {
        var config = await _versioningService.GetOrganizationConfigurationAsync(organizationId, cancellationToken);

        if (config == null)
            return NotFound($"No versioning configuration found for organization {organizationId}");

        return Ok(config);
    }

    /// <summary>
    /// Create versioning configuration for an organization
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrganizationVersioningConfigurationDto>> Create([FromBody] CreateOrganizationVersioningConfigurationDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var config = await _versioningService.CreateOrganizationConfigurationAsync(dto, null, cancellationToken);
            return CreatedAtAction(nameof(GetByOrganization), new { organizationId = config.OrganizationId }, config);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Update versioning configuration for an organization
    /// </summary>
    [HttpPut("{organizationId}")]
    public async Task<ActionResult<OrganizationVersioningConfigurationDto>> Update(string organizationId, [FromBody] UpdateOrganizationVersioningConfigurationDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var config = await _versioningService.UpdateOrganizationConfigurationAsync(organizationId, dto, null, cancellationToken);
            return Ok(config);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Set category-specific versioning configuration
    /// </summary>
    [HttpPut("{organizationId}/categories")]
    public async Task<IActionResult> SetCategoryConfiguration(string organizationId, [FromBody] SetCategoryVersioningConfigurationDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _versioningService.SetCategoryConfigurationAsync(organizationId, dto, null, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Remove category-specific versioning configuration
    /// </summary>
    [HttpDelete("{organizationId}/categories/{category}")]
    public async Task<IActionResult> RemoveCategoryConfiguration(string organizationId, DocumentCategory category, CancellationToken cancellationToken)
    {
        try
        {
            await _versioningService.RemoveCategoryConfigurationAsync(organizationId, category, null, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Check if versioning is enabled for a specific organization and category
    /// </summary>
    [HttpGet("{organizationId}/categories/{category}/enabled")]
    public async Task<ActionResult<bool>> IsVersioningEnabled(string organizationId, DocumentCategory category, CancellationToken cancellationToken)
    {
        var isEnabled = await _versioningService.IsVersioningEnabledAsync(organizationId, category, cancellationToken);
        return Ok(new { isEnabled });
    }

    /// <summary>
    /// Get max versions for a specific organization and category
    /// </summary>
    [HttpGet("{organizationId}/categories/{category}/max-versions")]
    public async Task<ActionResult<int>> GetMaxVersions(string organizationId, DocumentCategory category, CancellationToken cancellationToken)
    {
        var maxVersions = await _versioningService.GetMaxVersionsAsync(organizationId, category, cancellationToken);
        return Ok(new { maxVersions });
    }

    /// <summary>
    /// Delete (deactivate) versioning configuration for an organization
    /// </summary>
    [HttpDelete("{organizationId}")]
    public async Task<IActionResult> Delete(string organizationId, CancellationToken cancellationToken)
    {
        try
        {
            await _versioningService.DeleteOrganizationConfigurationAsync(organizationId, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
