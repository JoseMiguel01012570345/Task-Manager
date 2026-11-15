using Microsoft.AspNetCore.Diagnostics;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.API.Errors;

public sealed class ApiExceptionHandler(IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is RequestValidationException validation)
        {
            await Results.ValidationProblem(validation.Errors).ExecuteAsync(httpContext).ConfigureAwait(false);
            return true;
        }

        var (status, title, detail) = exception switch
        {
            UnauthorizedException unauthorized => (StatusCodes.Status401Unauthorized, "Unauthorized", unauthorized.Message),
            ForbiddenException forbidden => (StatusCodes.Status403Forbidden, "Forbidden", forbidden.Message),
            NotFoundException notFound => (StatusCodes.Status404NotFound, "Not Found", notFound.Message),
            ConcurrencyException conflict => (StatusCodes.Status409Conflict, "Conflict", conflict.Message),
            DomainException domain => (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", domain.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", UnexpectedDetail(exception)),
        };

        await Results.Problem(title: title, detail: detail, statusCode: status)
            .ExecuteAsync(httpContext)
            .ConfigureAwait(false);
        return true;
    }

    private string UnexpectedDetail(Exception exception) =>
        environment.IsDevelopment()
            ? exception.Message
            : "An unexpected error occurred.";
}
