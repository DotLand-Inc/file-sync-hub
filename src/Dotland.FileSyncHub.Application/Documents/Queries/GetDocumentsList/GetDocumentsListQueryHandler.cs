using Dotland.FileSyncHub.Application.Common.Services;
using Dotland.FileSyncHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentsList;

/// <summary>
/// Handler for GetDocumentsListQuery.
/// </summary>
public class GetDocumentsListQueryHandler : IRequestHandler<GetDocumentsListQuery, GetDocumentsListResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetDocumentsListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<GetDocumentsListResult> Handle(GetDocumentsListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Documents.AsQueryable();

        // Filter by organization
        query = query.Where(d => d.OrganizationId == request.OrganizationId);

        // RBAC Logic
        if (_currentUserService.IsAdmin)
        {
            // Admin sees all documents
        }
        else if (_currentUserService.IsHr)
        {
            // HR sees all HR docs AND their own docs
            // Note: If a doc is both HR and Own, it's included.
            // If a doc is NOT HR but Own, it's included.
            // If a doc is HR but NOT Own, it's included.
            var userId = _currentUserService.UserId;
            var userEmail = _currentUserService.Email;
            query = query.Where(d => d.Category == DocumentCategory.HumanResources || d.CreatedBy == userId || d.CreatedBy == userEmail);
        }
        else
        {
            // Regular user sees ONLY their own documents
            var userId = _currentUserService.UserId;
            var userEmail = _currentUserService.Email;
            query = query.Where(d => d.CreatedBy == userId || d.CreatedBy == userEmail);
        }

        // Filter by category if specified (applied on top of RBAC)
        if (request.Category.HasValue)
        {
            query = query.Where(d => d.Category == request.Category.Value);
        }

        // Project to DTO
        var documents = await query
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new DocumentDto
            {
                Id = d.Id,
                Name = d.Title,
                FileName = d.FileName,
                Description = d.Description,
                FileType = d.ContentType.Contains("pdf") ? "pdf" : 
                           d.ContentType.Contains("word") ? "docx" :
                           d.ContentType.Contains("sheet") ? "xlsx" :
                           d.ContentType.Contains("image") ? "png" : "file",
                FileSize = d.FileSize,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                CreatedBy = d.CreatedBy,
                Status = d.Status.ToString().ToLowerInvariant(),
                CategoryId = d.Category.ToString(),
                Version = d.CurrentVersion,
                Tags = new List<string>()
            })
            .ToListAsync(cancellationToken);

        return new GetDocumentsListResult
        {
            Documents = documents,
            Count = documents.Count
        };
    }
}
