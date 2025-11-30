using FluentValidation;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentVersions;

/// <summary>
/// Validator for GetDocumentVersionsQuery.
/// </summary>
public class GetDocumentVersionsQueryValidator : AbstractValidator<GetDocumentVersionsQuery>
{
    public GetDocumentVersionsQueryValidator()
    {
        RuleFor(v => v.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required.");

        RuleFor(v => v.DocumentId)
            .NotEmpty()
            .WithMessage("Document ID is required.");
    }
}
