using FluentValidation;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.UpdateVersioningConfiguration;

/// <summary>
/// Validator for UpdateVersioningConfigurationCommand.
/// </summary>
public class UpdateVersioningConfigurationCommandValidator : AbstractValidator<UpdateVersioningConfigurationCommand>
{
    public UpdateVersioningConfigurationCommandValidator()
    {
        RuleFor(v => v.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required.");

        RuleFor(v => v.Dto)
            .NotNull()
            .WithMessage("Configuration data is required.");
    }
}
