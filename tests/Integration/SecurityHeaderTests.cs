using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.API.Security;

namespace TaskManagement.IntegrationTests;

public class SecurityHeaderTests
{
    [Fact]
    public async Task Responses_include_the_api_security_headers()
    {
        var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());
        app.UseSecurityHeaders();
        app.Run(context =>
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return Task.CompletedTask;
        });
        var context = new DefaultHttpContext();
        await app.Build()(context);

        Assert.Equal("nosniff", context.Response.Headers["X-Content-Type-Options"]);
        Assert.Equal("no-referrer", context.Response.Headers["Referrer-Policy"]);
        Assert.Equal(SecurityHeadersMiddleware.ContentSecurityPolicy, context.Response.Headers["Content-Security-Policy"]);
    }
}
