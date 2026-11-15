using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.API.Observability;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddTaskHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("postgres", tags: ["ready"]);

        return services;
    }

    public static WebApplication MapTaskHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = static _ => false
        }).AllowAnonymous();

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = static check => check.Tags.Contains("ready")
        }).AllowAnonymous();

        return app;
    }
}
