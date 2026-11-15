using System.Net;
using System.Text;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class JsonRequestTests(ClusterApi cluster)
{
    [Fact]
    public async Task Non_json_write_is_rejected_before_the_endpoint()
    {
        using var content = new StringContent("nope", Encoding.UTF8, "text/plain");
        var response = await cluster.Anonymous.PostAsync("api/v1/tasks", content);

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }
}
