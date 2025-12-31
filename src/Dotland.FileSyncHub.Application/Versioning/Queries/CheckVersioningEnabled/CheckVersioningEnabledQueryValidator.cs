using FluentValidation;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.CheckVersioningEnabled;

/// <summary>
/// Validator for CheckVersioningEnabledQuery.
/// </summary>
public class CheckVersioningEnabledQueryValidator : AbstractValidator<CheckVersioningEnabledQuery>
{
    public CheckVersioningEnabledQueryValidator()
    {
        RuleFor(v => v.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required.");
    }
}
