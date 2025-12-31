using FluentValidation.Results;

namespace Dotland.FileSyncHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException()
    : Exception("One or more validation failures have occurred.")
{
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
}
