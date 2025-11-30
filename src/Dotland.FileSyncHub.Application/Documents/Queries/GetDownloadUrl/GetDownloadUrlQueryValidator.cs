using FluentValidation;

namespace Dotland.FileSyncHub.Application.Documents.Queries.GetDownloadUrl;

/// <summary>
/// Validator for GetDownloadUrlQuery.
/// </summary>
public class GetDownloadUrlQueryValidator : AbstractValidator<GetDownloadUrlQuery>
{
    public GetDownloadUrlQueryValidator()
    {
        RuleFor(v => v.S3Key)
            .NotEmpty()
            .WithMessage("S3 key is required.");

        RuleFor(v => v.ExpirationMinutes)
            .GreaterThan(0)
            .WithMessage("Expiration minutes must be greater than 0.")
            .LessThanOrEqualTo(10080) // 7 days max
            .WithMessage("Expiration minutes cannot exceed 7 days (10080 minutes).");
    }
}
