using Microsoft.AspNetCore.Diagnostics;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.API.Errors;

public sealed class ApiExceptionHandler : IExceptionHandler
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

        if (exception is NotFoundException notFound)
        {
            await Results.Problem(
                title: "Not Found",
                detail: notFound.Message,
                statusCode: StatusCodes.Status404NotFound).ExecuteAsync(httpContext).ConfigureAwait(false);
            return true;
        }

        if (exception is DomainException domain && exception is not ConcurrencyException)
        {
            await Results.Problem(
                title: "Unprocessable Entity",
                detail: domain.Message,
                statusCode: StatusCodes.Status422UnprocessableEntity).ExecuteAsync(httpContext).ConfigureAwait(false);
            return true;
        }

        if (exception is ConcurrencyException conflict)
        {
            await Results.Problem(
                title: "Conflict",
                detail: conflict.Message,
                statusCode: StatusCodes.Status409Conflict).ExecuteAsync(httpContext).ConfigureAwait(false);
            return true;
        }

        if (exception is ForbiddenException forbidden)
        {
            await Results.Problem(
                title: "Forbidden",
                detail: forbidden.Message,
                statusCode: StatusCodes.Status403Forbidden).ExecuteAsync(httpContext).ConfigureAwait(false);
            return true;
        }

        return false;
    }
}
