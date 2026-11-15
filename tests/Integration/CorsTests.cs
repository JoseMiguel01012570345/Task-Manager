using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.API.Security;

namespace TaskManagement.IntegrationTests;

public class CorsTests
{
    [Theory]
    [InlineData("http://localhost:5173", true)]
    [InlineData("https://evil.example", false)]
    public async Task Only_the_local_web_origin_is_allowed(string origin, bool allowed)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLocalCors();
        var app = new ApplicationBuilder(services.BuildServiceProvider());
        app.UseCors(LocalCors.PolicyName);
        app.Run(context =>
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Options;
        context.Request.Headers["Origin"] = origin;
        context.Request.Headers["Access-Control-Request-Method"] = "GET";
        await app.Build()(context);

        var header = context.Response.Headers.AccessControlAllowOrigin.ToString();
        if (allowed)
        {
            Assert.Equal(origin, header);
        }
        else
        {
            Assert.True(string.IsNullOrEmpty(header));
        }
    }
}
