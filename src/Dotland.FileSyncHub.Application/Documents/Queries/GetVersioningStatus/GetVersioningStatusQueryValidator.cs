using FluentValidation;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetVersioningStatus;

/// <summary>
/// Validator for GetVersioningStatusQuery.
/// </summary>
public class GetVersioningStatusQueryValidator : AbstractValidator<GetVersioningStatusQuery>
{
    public GetVersioningStatusQueryValidator()
    {
        RuleFor(v => v.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required.");
    }
}
