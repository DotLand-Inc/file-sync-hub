using FluentValidation;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.DeleteVersioningConfiguration;

/// <summary>
/// Validator for DeleteVersioningConfigurationCommand.
/// </summary>
public class DeleteVersioningConfigurationCommandValidator : AbstractValidator<DeleteVersioningConfigurationCommand>
{
    public DeleteVersioningConfigurationCommandValidator()
    {
        RuleFor(v => v.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required.");
    }
}
