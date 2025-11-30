using FluentValidation;

namespace Dotland.FileSyncHub.Application.Documents.Commands.DeleteDocument;

/// <summary>
/// Validator for DeleteDocumentCommand.
/// </summary>
public class DeleteDocumentCommandValidator : AbstractValidator<DeleteDocumentCommand>
{
    public DeleteDocumentCommandValidator()
    {
        RuleFor(v => v.S3Key)
            .NotEmpty()
            .WithMessage("S3 key is required.");
    }
}
