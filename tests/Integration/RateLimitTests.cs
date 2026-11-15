using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using TaskManagement.API.Security;

namespace TaskManagement.IntegrationTests;

public class RateLimitTests
{
    [Fact]
    public async Task Excess_requests_return_429_and_retry_after()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, _) =>
            {
                context.HttpContext.Response.Headers.RetryAfter = "10";
                await Results.Problem(statusCode: StatusCodes.Status429TooManyRequests).ExecuteAsync(context.HttpContext);
            };
            options.AddPolicy("tiny", httpContext => TaskRateLimiting.Window(httpContext, "tiny", permitLimit: 1, windowSeconds: 60));
        });

        var app = builder.Build();
        app.UseRateLimiter();
        app.MapGet("/tiny", () => Results.Ok()).RequireRateLimiting("tiny");
        await app.StartAsync();

        var client = app.GetTestClient();
        var first = await client.GetAsync("/tiny");
        var second = await client.GetAsync("/tiny");

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, second.StatusCode);
        Assert.Equal("10", second.Headers.RetryAfter?.Delta?.Seconds.ToString() ?? second.Headers.GetValues("Retry-After").Single());

        await app.StopAsync();
        await app.DisposeAsync();
    }
}
