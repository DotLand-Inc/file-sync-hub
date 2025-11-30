using FluentValidation;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDocumentsList;

/// <summary>
/// Validator for GetDocumentsListQuery.
/// </summary>
public class GetDocumentsListQueryValidator : AbstractValidator<GetDocumentsListQuery>
{
    public GetDocumentsListQueryValidator()
    {
        RuleFor(v => v.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required.");
    }
}
