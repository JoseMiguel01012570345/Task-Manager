using FluentValidation.Results;

namespace TaskManagement.Application.Exceptions;

public sealed class RequestValidationException : Exception
{
    public RequestValidationException(ValidationResult result)
        : base("The request is not valid.")
    {
        Errors = result.Errors
            .GroupBy(error => string.IsNullOrWhiteSpace(error.PropertyName) ? string.Empty : error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).Distinct().ToArray());
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
