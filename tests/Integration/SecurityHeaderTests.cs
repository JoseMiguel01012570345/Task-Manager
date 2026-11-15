using System.Net;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class SecurityHeaderTests(ClusterApi cluster)
{
    [Fact]
    public async Task Responses_include_the_api_security_headers()
    {
        var response = await cluster.Anonymous.GetAsync("health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("nosniff", Header(response, "X-Content-Type-Options"));
        Assert.Equal("no-referrer", Header(response, "Referrer-Policy"));
        Assert.Equal("default-src 'none'; frame-ancestors 'none'; base-uri 'none'", Header(response, "Content-Security-Policy"));
    }

    private static string Header(HttpResponseMessage response, string name)
    {
        return response.Headers.TryGetValues(name, out var values)
            ? string.Concat(values)
            : string.Empty;
    }
}
