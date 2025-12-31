using FluentValidation;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.CreateVersioningConfiguration;

/// <summary>
/// Validator for CreateVersioningConfigurationCommand.
/// </summary>
public class CreateVersioningConfigurationCommandValidator : AbstractValidator<CreateVersioningConfigurationCommand>
{
    public CreateVersioningConfigurationCommandValidator()
    {
        RuleFor(v => v.Dto)
            .NotNull()
            .WithMessage("Configuration data is required.");

        RuleFor(v => v.Dto.OrganizationId)
            .NotEmpty()
            .When(v => v.Dto != null)
            .WithMessage("Organization ID is required.");
    }
}
