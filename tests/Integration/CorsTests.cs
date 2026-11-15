using System.Net;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class CorsTests(ClusterApi cluster)
{
    [Theory]
    [InlineData("http://localhost:5173", true)]
    [InlineData("https://evil.example", false)]
    public async Task Only_the_local_web_origin_is_allowed(string origin, bool allowed)
    {
        using var request = new HttpRequestMessage(HttpMethod.Options, "api/v1/tasks");
        request.Headers.TryAddWithoutValidation("Origin", origin);
        request.Headers.TryAddWithoutValidation("Access-Control-Request-Method", "GET");

        var response = await cluster.Anonymous.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var header = response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values)
            ? string.Concat(values)
            : string.Empty;
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
