using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Compact;
using TaskManagement.API.Auth;
using TaskManagement.API.Observability;
using TaskManagement.API.Endpoints;
using TaskManagement.API.Errors;
using TaskManagement.API.Security;
using TaskManagement.Application;
using TaskManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "TaskManagement.API")
        .WriteTo.Console(new CompactJsonFormatter());

    var endpoint = context.Configuration["OpenTelemetry:OtlpEndpoint"];
    if (Uri.TryCreate(endpoint, UriKind.Absolute, out var otlp))
    {
        configuration.WriteTo.OpenTelemetry(options =>
        {
            options.Endpoint = otlp.AbsoluteUri;
            options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc;
            options.ResourceAttributes["service.name"] = TelemetryExtensions.ServiceName;
        });
    }
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = JsonRequestMiddleware.MaxBodyBytes;
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration,
    seedDevelopmentData: builder.Environment.IsDevelopment());
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
    };
});
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddTaskAuthentication(builder.Configuration, builder.Environment);
builder.Services.AddLocalCors();
builder.Services.AddTaskRateLimiting();
builder.Services.AddTaskTelemetry(builder.Configuration);
builder.Services.AddTaskHealthChecks();
builder.Services.AddOpenApi();

var app = builder.Build();

if (args.Contains("--migrate"))
{
    using var scope = app.Services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<TaskManagement.Infrastructure.Persistence.AppDbContext>();
    database.Database.Migrate();
    return;
}

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});
app.UseExceptionHandler();
app.UseSecurityHeaders();
app.UseCors(LocalCors.PolicyName);
app.UseRateLimiter();
app.UseJsonRequests();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.MapTaskHealthChecks();
app.MapTaskEndpoints();

app.Run();

public partial class Program
{
}
