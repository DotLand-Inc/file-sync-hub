using FluentValidation;

namespace Dotland.FileSyncHub.Application.Documents.Queries.DocumentDirectDownload;

/// <summary>
/// Validator for DocumentDirectDownloadQuery.
/// </summary>
public class DocumentDirectDownloadQueryValidator : AbstractValidator<DocumentDirectDownloadQuery>
{
    public DocumentDirectDownloadQueryValidator()
    {
        RuleFor(v => v.S3Key)
            .NotEmpty()
            .WithMessage("S3 key is required.");
    }
}
