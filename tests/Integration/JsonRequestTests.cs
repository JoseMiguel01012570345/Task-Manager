using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.API.Security;

namespace TaskManagement.IntegrationTests;

public class JsonRequestTests
{
    [Fact]
    public async Task Non_json_write_is_rejected_before_the_endpoint()
    {
        var called = false;
        var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());
        app.UseJsonRequests();
        app.Run(_ =>
        {
            called = true;
            return Task.CompletedTask;
        });
        var pipeline = app.Build();
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.ContentType = "text/plain";

        await pipeline(context);

        Assert.False(called);
        Assert.Equal(StatusCodes.Status415UnsupportedMediaType, context.Response.StatusCode);
    }
}
