using Dotland.FileSyncHub.Application.Common.Models;
using Dotland.FileSyncHub.Application.Documents.Commands.DeleteDocument;
using Dotland.FileSyncHub.Application.Documents.Queries.DocumentDirectDownload;
using Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentsList;
using Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentVersions;
using Dotland.FileSyncHub.Application.Documents.Queries.GetDownloadUrl;
using Dotland.FileSyncHub.Application.Documents.Queries.GetVersioningStatus;
using Dotland.FileSyncHub.Domain.Enums;
using Dotland.FileSyncHub.Web.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dotland.FileSyncHub.Web.Controllers;

/// <summary>
/// API controller for document management operations.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class DocumentsController(IMediator mediator) : ControllerBase
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

        return Ok( new { S3Key = result.S3Key, Payload = result } );
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
    [ProducesResponseType(typeof(GetDocumentVersionsResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDocumentVersionsAsync(
        string organizationId,
        string documentId,
        [FromQuery] DocumentCategory category,
        CancellationToken cancellationToken)
    {
        var query = new GetDocumentVersionsQuery
        {
            OrganizationId = organizationId,
            DocumentId = documentId,
            Category = category
        };

        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Check if versioning is enabled for a category.
    /// </summary>
    [HttpGet("{organizationId}/versioning/{category}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckVersioningAsync(
        string organizationId,
        DocumentCategory category,
        CancellationToken cancellationToken)
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
    [ProducesResponseType(typeof(GetDownloadUrlResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDownloadUrlAsync(
        [FromQuery] string s3Key,
        [FromQuery] int expirationMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDownloadUrlQuery
        {
            S3Key = s3Key,
            ExpirationMinutes = expirationMinutes
        };

        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Download a document directly.
    /// </summary>
    [HttpGet("download/direct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DocumentDirectDownloadAsync(
        [FromQuery] string s3Key,
        CancellationToken cancellationToken = default)
    {
        var query = new DocumentDirectDownloadQuery
        {
            S3Key = s3Key
        };

        var result = await mediator.Send(query, cancellationToken);

        return File(result.Content, result.ContentType, result.FileName);
    }

    /// <summary>
    /// Delete a document.
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteDocumentAsync(
        [FromQuery] string s3Key,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteDocumentCommand
        {
            S3Key = s3Key
        };

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// List documents for an organization.
    /// </summary>
    [HttpGet("{organizationId}/list")]
    [ProducesResponseType(typeof(GetDocumentsListResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDocumentsListAsync(
        string organizationId,
        [FromQuery] DocumentCategory? category = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDocumentsListQuery
        {
            OrganizationId = organizationId,
            Category = category
        };

        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }
}
