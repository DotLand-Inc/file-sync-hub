using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dotland.FileSyncHub.Application.Common.Models;
using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Application.Documents.Queries.GetVersioningStatus;
using Dotland.FileSyncHub.Domain.Enums;
using Dotland.FileSyncHub.Web.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dotland.FileSyncHub.Web.Controllers;

/// <summary>
/// API controller for document management operations.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class DocumentsController(
    IS3StorageService storageService,
    IMediator mediator,
    ILogger<DocumentsController> logger)
    : ControllerBase
{
    /// <summary>
    /// Upload a new document to S3.
    /// </summary>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(UploadResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync(
        [FromForm] UploadDocumentRequest request, CancellationToken cancellationToken)
    {
        var command = await request.ToCommandAsync(cancellationToken);
        var result = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetDownloadUrl), new { s3Key = result.S3Key }, result);
    }

    /// <summary>
    /// Upload a new version of an existing document.
    /// </summary>
    [HttpPut("upload/version")]
    [ProducesResponseType(typeof(UploadResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutAsync(
        [FromForm] UploadDocumentVersionRequest request,
        CancellationToken cancellationToken = default)
    {
        // Check if versioning is enabled for this category using CQRS query
        var versioningQuery = new GetVersioningStatusQuery
        {
            OrganizationId = request.OrganizationId,
            Category = request.Category
        };

        var isVersioningEnabled = await mediator.Send(versioningQuery, cancellationToken);

        if (!isVersioningEnabled)
        {
            return BadRequest(new { error = $"Versioning is not enabled for category '{request.Category}' in this organization" });
        }

        var command = await request.ToCommandAsync(cancellationToken);
        var result = await mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get all versions of a document.
    /// </summary>
    [HttpGet("{organizationId}/versions/{documentId}")]
    [ProducesResponseType(typeof(List<DocumentVersion>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVersions(
        string organizationId,
        string documentId,
        [FromQuery] DocumentCategory category,
        CancellationToken cancellationToken)
    {
        var versions = await storageService.GetDocumentVersionsAsync(
            organizationId, category, documentId, cancellationToken);

        var versioningQuery = new GetVersioningStatusQuery
        {
            OrganizationId = organizationId,
            Category = category
        };

        var versioningEnabled = await mediator.Send(versioningQuery, cancellationToken);

        return Ok(new
        {
            documentId,
            versioningEnabled,
            versions,
            count = versions.Count
        });
    }

    /// <summary>
    /// Check if versioning is enabled for a category.
    /// </summary>
    [HttpGet("{organizationId}/versioning/{category}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckVersioning(string organizationId, DocumentCategory category, CancellationToken cancellationToken = default)
    {
        var query = new GetVersioningStatusQuery
        {
            OrganizationId = organizationId,
            Category = category
        };

        var enabled = await mediator.Send(query, cancellationToken);

        return Ok(new { organizationId, category = category.ToString(), versioningEnabled = enabled });
    }

    /// <summary>
    /// Get a presigned download URL for a document.
    /// </summary>
    [HttpGet("download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDownloadUrl(
        [FromQuery] string s3Key,
        [FromQuery] int expirationMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(s3Key))
        {
            return BadRequest(new { error = "S3 key is required" });
        }

        var exists = await storageService.FileExistsAsync(s3Key, cancellationToken);
        if (!exists)
        {
            return NotFound(new { error = "File not found" });
        }

        var url = storageService.GeneratePresignedUrl(s3Key, expirationMinutes);
        return Ok(new { downloadUrl = url, expiresInMinutes = expirationMinutes });
    }

    /// <summary>
    /// Download a document directly.
    /// </summary>
    [HttpGet("download/direct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadDirect(
        [FromQuery] string s3Key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(s3Key))
        {
            return BadRequest(new { error = "S3 key is required" });
        }

        try
        {
            var (content, metadata) = await storageService.DownloadFileAsync(s3Key, cancellationToken);

            var filename = metadata.TryGetValue("x-amz-meta-original-filename", out var fn)
                ? fn
                : Path.GetFileName(s3Key);

            var contentType = "application/octet-stream";

            return File(content, contentType, filename);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { error = "File not found" });
        }
    }

    /// <summary>
    /// Delete a document.
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(
        [FromQuery] string s3Key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(s3Key))
        {
            return BadRequest(new { error = "S3 key is required" });
        }

        try
        {
            await storageService.DeleteFileAsync(s3Key, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delete failed for key: {S3Key}", s3Key);
            return StatusCode(500, new { error = "Delete failed" });
        }
    }

    /// <summary>
    /// List documents for an organization.
    /// </summary>
    [HttpGet("{organizationId}/list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        string organizationId,
        [FromQuery] DocumentCategory? category = null,
        CancellationToken cancellationToken = default)
    {
        var files = await storageService.ListFilesAsync(organizationId, category, cancellationToken: cancellationToken);
        return Ok(new { files, count = files.Count });
    }
}
