using FluentValidation;

namespace Dotland.FileSyncHub.Application.Versioning.Queries.GetVersioningConfiguration;

/// <summary>
/// Validator for GetVersioningConfigurationQuery.
/// </summary>
public class GetVersioningConfigurationQueryValidator : AbstractValidator<GetVersioningConfigurationQuery>
{
    public GetVersioningConfigurationQueryValidator()
    {
        RuleFor(v => v.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required.");
    }
}
