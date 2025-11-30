using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dotland.FileSyncHub.Application.Common.Models;
using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dotland.FileSyncHub.Web.Controllers;

/// <summary>
/// API controller for document management operations.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class DocumentsController(IS3StorageService storageService, ILogger<DocumentsController> logger)
    : ControllerBase
{
    private readonly IS3StorageService _storageService = storageService;

    /// <summary>
    /// Upload a new document to S3.
    /// </summary>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(UploadResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromForm] string organizationId,
        [FromForm] DocumentCategory category = DocumentCategory.Other,
        [FromForm] string? description = null,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "No file provided" });
        }

        if (string.IsNullOrWhiteSpace(organizationId))
        {
            return BadRequest(new { error = "Organization ID is required" });
        }

        try
        {
            var metadata = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(description))
            {
                metadata["description"] = description;
            }

            using var stream = file.OpenReadStream();
            var result = await _storageService.UploadFileAsync(
                stream,
                file.FileName,
                organizationId,
                category,
                file.ContentType,
                metadata,
                cancellationToken);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return CreatedAtAction(nameof(GetDownloadUrl), new { s3Key = result.S3Key }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Upload failed for file: {Filename}", file.FileName);
            return StatusCode(500, new { error = "Upload failed" });
        }
    }

    /// <summary>
    /// Upload a new version of an existing document.
    /// </summary>
    [HttpPost("upload/version")]
    [ProducesResponseType(typeof(UploadResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadNewVersion(
        IFormFile file,
        [FromForm] string organizationId,
        [FromForm] string documentId,
        [FromForm] DocumentCategory category,
        [FromForm] string? description = null,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "No file provided" });
        }

        if (string.IsNullOrWhiteSpace(organizationId) || string.IsNullOrWhiteSpace(documentId))
        {
            return BadRequest(new { error = "Organization ID and Document ID are required" });
        }

        // Check if versioning is enabled for this category
        if (!await _storageService.IsVersioningEnabledAsync(organizationId, category, cancellationToken))
        {
            return BadRequest(new { error = $"Versioning is not enabled for category '{category}' in this organization" });
        }

        try
        {
            var metadata = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(description))
            {
                metadata["description"] = description;
            }

            using var stream = file.OpenReadStream();
            var result = await _storageService.UploadFileAsync(
                stream,
                file.FileName,
                organizationId,
                category,
                documentId,
                file.ContentType,
                metadata,
                cancellationToken);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return CreatedAtAction(nameof(GetDownloadUrl), new { s3Key = result.S3Key }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Version upload failed for document: {DocumentId}", documentId);
            return StatusCode(500, new { error = "Upload failed" });
        }
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
        CancellationToken cancellationToken = default)
    {
        var versions = await _storageService.GetDocumentVersionsAsync(
            organizationId, category, documentId, cancellationToken);

        return Ok(new
        {
            documentId,
            versioningEnabled = await _storageService.IsVersioningEnabledAsync(organizationId, category, cancellationToken),
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
        var enabled = await _storageService.IsVersioningEnabledAsync(organizationId, category, cancellationToken);
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

        var exists = await _storageService.FileExistsAsync(s3Key, cancellationToken);
        if (!exists)
        {
            return NotFound(new { error = "File not found" });
        }

        var url = _storageService.GeneratePresignedUrl(s3Key, expirationMinutes);
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
            var (content, metadata) = await _storageService.DownloadFileAsync(s3Key, cancellationToken);

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
            await _storageService.DeleteFileAsync(s3Key, cancellationToken);
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
        var files = await _storageService.ListFilesAsync(organizationId, category, cancellationToken: cancellationToken);
        return Ok(new { files, count = files.Count });
    }
}
