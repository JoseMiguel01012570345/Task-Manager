using Microsoft.AspNetCore.Diagnostics;
using TaskManagement.Application.Exceptions;

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
