using FluentValidation;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.SetCategoryVersioning;

/// <summary>
/// Validator for SetCategoryVersioningCommand.
/// </summary>
public class SetCategoryVersioningCommandValidator : AbstractValidator<SetCategoryVersioningCommand>
{
    public SetCategoryVersioningCommandValidator()
    {
        RuleFor(v => v.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required.");

        RuleFor(v => v.Dto)
            .NotNull()
            .WithMessage("Configuration data is required.");
    }
}
