using System.Threading.RateLimiting;

namespace TaskManagement.API.Security;

public static class TaskRateLimiting
{
    public static IServiceCollection AddTaskRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, _) =>
            {
                if (!context.HttpContext.Response.HasStarted)
                {
                    context.HttpContext.Response.Headers.RetryAfter = "10";
                    await Results.Problem(
                        title: "Too Many Requests",
                        statusCode: StatusCodes.Status429TooManyRequests)
                        .ExecuteAsync(context.HttpContext)
                        .ConfigureAwait(false);
                }
            };

            options.AddPolicy("reads", httpContext => Window(httpContext, "read", permitLimit: 100, windowSeconds: 10));
            options.AddPolicy("writes", httpContext => Window(httpContext, "write", permitLimit: 30, windowSeconds: 10));
            options.AddPolicy("login", httpContext => Window(httpContext, "login", permitLimit: 10, windowSeconds: 60));
        });

        return services;
    }

    public static RateLimitPartition<string> Window(HttpContext httpContext, string operation, int permitLimit, int windowSeconds)
    {
        var user = httpContext.User.FindFirst("preferred_username")?.Value
            ?? httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(
            $"{operation}:{user}",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromSeconds(windowSeconds),
                QueueLimit = 0,
            });
    }
}
