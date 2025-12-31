using FluentValidation;

namespace Dotland.FileSyncHub.Application.Versioning.Commands.RemoveCategoryVersioning;

/// <summary>
/// Validator for RemoveCategoryVersioningCommand.
/// </summary>
public class RemoveCategoryVersioningCommandValidator : AbstractValidator<RemoveCategoryVersioningCommand>
{
    public RemoveCategoryVersioningCommandValidator()
    {
        RuleFor(v => v.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required.");
    }
}
