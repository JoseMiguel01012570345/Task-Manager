using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TaskManagement.API.Observability;

public static class TelemetryExtensions
{
    public const string ServiceName = "taskmanagement-api";

    public static IServiceCollection AddTaskTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var endpoint = configuration["OpenTelemetry:OtlpEndpoint"];
        Uri? otlpEndpoint = Uri.TryCreate(endpoint, UriKind.Absolute, out var parsed) ? parsed : null;

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(ServiceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = context => !context.Request.Path.StartsWithSegments("/health");
                    })
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                if (otlpEndpoint is not null)
                {
                    tracing.AddOtlpExporter(options => options.Endpoint = otlpEndpoint);
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (otlpEndpoint is not null)
                {
                    metrics.AddOtlpExporter(options => options.Endpoint = otlpEndpoint);
                }
            });

        return services;
    }
}
