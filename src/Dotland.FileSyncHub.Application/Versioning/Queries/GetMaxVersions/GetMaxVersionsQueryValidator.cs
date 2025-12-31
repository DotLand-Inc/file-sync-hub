using FluentValidation;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetMaxVersions;

/// <summary>
/// Validator for GetMaxVersionsQuery.
/// </summary>
public class GetMaxVersionsQueryValidator : AbstractValidator<GetMaxVersionsQuery>
{
    public GetMaxVersionsQueryValidator()
    {
        RuleFor(v => v.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required.");
    }
}
