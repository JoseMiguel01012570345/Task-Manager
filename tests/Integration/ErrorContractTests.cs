using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using TaskManagement.API.Errors;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.IntegrationTests;

public class ErrorContractTests
{
    [Fact]
    public async Task Known_exceptions_use_the_documented_status()
    {
        var cases = new (Exception Exception, int Status)[]
        {
            (new UnauthorizedException(), StatusCodes.Status401Unauthorized),
            (new ForbiddenException("no"), StatusCodes.Status403Forbidden),
            (new NotFoundException(), StatusCodes.Status404NotFound),
            (new ConcurrencyException(), StatusCodes.Status409Conflict),
            (new DomainException("bad transition"), StatusCodes.Status422UnprocessableEntity),
        };

        foreach (var (exception, status) in cases)
        {
            var context = await HandleAsync(exception, development: false);
            Assert.Equal(status, context.Response.StatusCode);
            Assert.Contains("problem", context.Response.ContentType, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task Unexpected_exception_hides_internal_detail_outside_development()
    {
        var context = await HandleAsync(new InvalidOperationException("password=secret"), development: false);
        var body = await ReadBodyAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("stack", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("traceId", body, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<DefaultHttpContext> HandleAsync(Exception exception, bool development)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "trace-test";
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddProblemDetails();
        context.RequestServices = services.BuildServiceProvider();
        var handler = new ApiExceptionHandler(new TestHostEnvironment(development));
        var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);
        Assert.True(handled);
        return context;
    }

    private static async Task<string> ReadBodyAsync(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        return document.RootElement.GetRawText();
    }

    private sealed class TestHostEnvironment(bool development) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = development ? Environments.Development : Environments.Production;

        public string ApplicationName { get; set; } = "TaskManagement.API";

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
