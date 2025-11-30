using FluentValidation;

namespace Dotland.FileSyncHub.Application.Documents.Commands.UploadDocument;

/// <summary>
/// Validator for UploadDocumentCommand.
/// </summary>
public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentCommandValidator()
    {
        RuleFor(v => v.FileContent)
            .NotEmpty()
            .WithMessage("File content is required.");

        RuleFor(v => v.FileName)
            .NotEmpty()
            .WithMessage("File name is required.")
            .MaximumLength(255)
            .WithMessage("File name must not exceed 255 characters.");

        RuleFor(v => v.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required.");

        RuleFor(v => v.FileContent)
            .Must(content => content.Length <= 100 * 1024 * 1024) // 100MB max
            .WithMessage("File size must not exceed 100MB.")
            .When(v => v.FileContent != null);
    }
}
